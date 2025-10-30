using System.Collections.Generic;
using UnityEngine;

public enum TileDirection
{
    Forward,
    Left,
    Right
}

[System.Serializable]
public class WeightedDirectionTile
{
    public GameObject prefab;
    [Range(1, 100)]
    public int weight = 1;
    public TileDirection direction = TileDirection.Forward;
}

public class ProceduralLvLGenTurns : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public Transform tileParent;

    [Header("Tile Prefabs")]
    public GameObject startTile;
    public List<WeightedDirectionTile> weightedTiles = new List<WeightedDirectionTile>();
    public GameObject endTile;

    [Header("Level Settings")]
    public bool endlessMode = true;
    public int levelTileCount = 10;
    public int maxActiveTiles = 5;
    public float tileLength = 20f;
    public float spawnDistance = 40f;

    private List<GameObject> activeTiles = new List<GameObject>();
    private HashSet<Vector3> occupiedPositions = new HashSet<Vector3>();
    private Vector3 currentDirection = Vector3.forward;
    private Vector3 currentPosition = Vector3.zero;
    private int tilesSpawned = 0;
    private bool levelEnded = false;

    void Start()
    {
        if (player == null)
        {
            Debug.LogWarning("ProceduralLevelGenerator: No player assigned!");
            return;
        }

        GenerateInitialTiles();
    }

    void Update()
    {
        if (player == null || levelEnded) return;

        float distanceAhead = Vector3.Dot(currentDirection, player.position - currentPosition);

        if (endlessMode && distanceAhead + spawnDistance > tileLength * (maxActiveTiles - 1))
        {
            var (prefab, direction) = GetWeightedRandomTileWithDirection();
            SpawnTile(prefab, direction);
            CleanupOldTiles();
        }
        else if (!endlessMode && tilesSpawned < levelTileCount)
        {
            if (distanceAhead + spawnDistance > tileLength * 2)
            {
                var (prefab, direction) = GetWeightedRandomTileWithDirection();
                SpawnTile(prefab, direction);
            }
        }
        else if (!endlessMode && !levelEnded && tilesSpawned >= levelTileCount)
        {
            if (endTile != null)
            {
                SpawnTile(endTile, TileDirection.Forward);
            }
            levelEnded = true;
        }
    }

    void GenerateInitialTiles()
    {
        tilesSpawned = 0;
        levelEnded = false;
        currentDirection = Vector3.forward;
        currentPosition = Vector3.zero;
        activeTiles.Clear();
        occupiedPositions.Clear();

        if (startTile != null)
            SpawnTile(startTile, TileDirection.Forward, true);
        else
        {
            var (prefab, direction) = GetWeightedRandomTileWithDirection();
            SpawnTile(prefab, direction, true);
        }

        int initialTiles = endlessMode ? maxActiveTiles - 1 : Mathf.Min(levelTileCount, maxActiveTiles - 1);
        for (int i = 0; i < initialTiles; i++)
        {
            var (prefab, direction) = GetWeightedRandomTileWithDirection();
            SpawnTile(prefab, direction);
        }
    }

    void SpawnTile(GameObject prefab, TileDirection direction, bool isFirstTile = false)
    {
        if (prefab == null)
        {
            Debug.LogWarning("ProceduralLevelGenerator: Tried to spawn a null prefab!");
            return;
        }

        if (occupiedPositions.Contains(currentPosition))
        {
            Debug.LogWarning("ProceduralLevelGenerator: Overlap detected at " + currentPosition + ", skipping tile spawn.");
            return;
        }

        Quaternion rotation = Quaternion.LookRotation(currentDirection);
        GameObject newTile = Instantiate(prefab, currentPosition, rotation, tileParent);
        activeTiles.Add(newTile);
        occupiedPositions.Add(currentPosition);

        // Update direction
        switch (direction)
        {
            case TileDirection.Left:
                currentDirection = Quaternion.Euler(0, -90, 0) * currentDirection;
                break;
            case TileDirection.Right:
                currentDirection = Quaternion.Euler(0, 90, 0) * currentDirection;
                break;
        }

        currentPosition += currentDirection * tileLength;
        tilesSpawned++;
    }

    void CleanupOldTiles()
    {
        while (endlessMode && activeTiles.Count > maxActiveTiles)
        {
            GameObject oldTile = activeTiles[0];
            occupiedPositions.Remove(oldTile.transform.position);
            activeTiles.RemoveAt(0);
            Destroy(oldTile);
        }
    }

    public void ResetLevel()
    {
        foreach (var tile in activeTiles)
        {
            Destroy(tile);
        }
        activeTiles.Clear();
        occupiedPositions.Clear();
        GenerateInitialTiles();
    }

    (GameObject, TileDirection) GetWeightedRandomTileWithDirection()
    {
        List<(GameObject, TileDirection, int)> validTiles = new List<(GameObject, TileDirection, int)>();

        foreach (var tile in weightedTiles)
        {
            Vector3 testDirection = currentDirection;
            switch (tile.direction)
            {
                case TileDirection.Left:
                    testDirection = Quaternion.Euler(0, -90, 0) * testDirection;
                    break;
                case TileDirection.Right:
                    testDirection = Quaternion.Euler(0, 90, 0) * testDirection;
                    break;
            }

            Vector3 testPosition = currentPosition + testDirection * tileLength;
            if (!occupiedPositions.Contains(testPosition))
            {
                validTiles.Add((tile.prefab, tile.direction, Mathf.Max(1, tile.weight)));
            }
        }

        if (validTiles.Count == 0)
        {
            Debug.LogWarning("ProceduralLevelGenerator: No valid tiles found, fallback to forward.");
            return (null, TileDirection.Forward);
        }

        int totalWeight = 0;
        foreach (var tile in validTiles)
        {
            totalWeight += tile.Item3;
        }

        int randomValue = Random.Range(0, totalWeight);
        int cumulative = 0;

        foreach (var tile in validTiles)
        {
            cumulative += tile.Item3;
            if (randomValue < cumulative)
                return (tile.Item1, tile.Item2);
        }

        return (validTiles[0].Item1, validTiles[0].Item2);
    }
}