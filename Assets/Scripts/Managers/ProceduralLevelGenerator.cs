using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WeightedTile
{
    public GameObject prefab;
    [Range(1, 100)]
    public int weight = 1;
}

public class ProceduralLevelGenerator : MonoBehaviour
{
    [Header("References")]
    [Tooltip("The player or camera used to determine spawn position.")]
    public Transform player;

    [Tooltip("Parent object to organize spawned tiles in the hierarchy.")]
    public Transform tileParent;

    [Header("Tile Prefabs")]
    [Tooltip("Optional: Specific tile to use at the start of the level.")]
    public GameObject startTile;

    [Tooltip("List of random weighted tiles to spawn after the start tile.")]
    public List<WeightedTile> weightedTiles = new List<WeightedTile>();

    [Tooltip("Optional: Specific tile to use at the end of the level.")]
    public GameObject endTile;

    [Header("Level Settings")]
    [Tooltip("If true, the level will generate infinitely.")]
    public bool endlessMode = true;

    [Tooltip("Number of random tiles to spawn before the end tile (if not endless).")]
    public int levelTileCount = 10;

    [Tooltip("How many tiles to keep active at once (for endless mode).")]
    public int maxActiveTiles = 5;

    [Tooltip("Distance between consecutive tiles.")]
    public float tileLength = 20f;

    [Tooltip("How far ahead of the player to spawn new tiles.")]
    public float spawnDistance = 40f;

    private List<GameObject> activeTiles = new List<GameObject>();
    private float nextSpawnZ = 0f;
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

        // Spawn new tiles when the player is close enough
        if (endlessMode && player.position.z + spawnDistance > nextSpawnZ - (tileLength * (maxActiveTiles - 1)))
        {
            SpawnTile(GetWeightedRandomTile());
            CleanupOldTiles();
        }
        else if (!endlessMode && tilesSpawned < levelTileCount)
        {
            if (player.position.z + spawnDistance > nextSpawnZ - (tileLength * 2))
            {
                SpawnTile(GetWeightedRandomTile());
            }
        }
        else if (!endlessMode && !levelEnded && tilesSpawned >= levelTileCount)
        {
            // Spawn end tile only once
            if (endTile != null)
            {
                SpawnTile(endTile);
            }
            levelEnded = true;
        }
    }

    void GenerateInitialTiles()
    {
        tilesSpawned = 0;
        levelEnded = false;
        nextSpawnZ = 0f;

        activeTiles.Clear();

        // Start tile
        if (startTile != null)
            SpawnTile(startTile, true);
        else
            SpawnTile(GetWeightedRandomTile(), true);

        // Initial random tiles
        int initialTiles = endlessMode ? maxActiveTiles - 1 : Mathf.Min(levelTileCount, maxActiveTiles - 1);
        for (int i = 0; i < initialTiles; i++)
        {
            SpawnTile(GetWeightedRandomTile());
        }
    }

    void SpawnTile(GameObject prefab, bool isFirstTile = false)
    {
        if (prefab == null)
        {
            Debug.LogWarning("ProceduralLevelGenerator: Tried to spawn a null prefab!");
            return;
        }

        Vector3 spawnPosition = isFirstTile ? Vector3.zero : new Vector3(0, 0, nextSpawnZ);

        GameObject newTile = Instantiate(prefab, spawnPosition, Quaternion.identity, tileParent);
        activeTiles.Add(newTile);

        nextSpawnZ += tileLength;
        tilesSpawned++;
    }

    void CleanupOldTiles()
    {
        while (endlessMode && activeTiles.Count > maxActiveTiles)
        {
            GameObject oldTile = activeTiles[0];
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
        GenerateInitialTiles();
    }

    GameObject GetWeightedRandomTile()
    {
        if (weightedTiles.Count == 0)
        {
            Debug.LogWarning("ProceduralLevelGenerator: No weighted tiles assigned!");
            return null;
        }

        int totalWeight = 0;
        foreach (var tile in weightedTiles)
        {
            totalWeight += Mathf.Max(1, tile.weight);
        }

        int randomValue = Random.Range(0, totalWeight);
        int cumulative = 0;

        foreach (var tile in weightedTiles)
        {
            cumulative += Mathf.Max(1, tile.weight);
            if (randomValue < cumulative)
                return tile.prefab;
        }

        return weightedTiles[0].prefab;
    }
}
