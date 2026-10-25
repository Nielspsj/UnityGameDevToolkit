// =====================================================================
//  EndlessRunnerGenerator.cs  (Unity 6, Built-in Render Pipeline)
//
//  HOW TO SET UP (30 seconds):
//   1. Put EndlessRunnerGenerator.cs AND SimpleAutoRun.cs in your Assets folder.
//   2. In the Hierarchy: right-click > Create Empty. Name it "Generator".
//   3. Select "Generator" > Add Component > Endless Runner Generator.
//   4. (Optional) Drag your player object into the "Player" slot.
//      Leave it empty and a capsule player is created for you.
//   5. Press Play.  A/D or Left/Right = change lane.  Space/W/Up = jump.
//
//  OPTIONAL: To make "Obstacle" and "Coin" tags work, go to
//  Edit > Project Settings > Tags and Layers > Tags > "+" and add them.
//  (Everything works without them. Tags are only for your own game code.)
//
//  USING YOUR OWN CHUNK PREFABS (optional):
//   Drag prefabs into "Chunk Prefabs". Each prefab's pivot must sit at the
//   START of the chunk (the near edge), and it should be as long as
//   "Chunk Length" along the Z axis. If the list is empty, simple cube
//   chunks are generated automatically.
// =====================================================================
using System.Collections.Generic;
using UnityEngine;

public class EndlessRunnerGenerator : MonoBehaviour
{
    [Header("Player (leave empty to auto-create a capsule)")]
    public Transform player;
    public bool addAutoRunToPlayer = true;

    [Header("Optional: your own chunk prefabs")]
    public GameObject[] chunkPrefabs;

    [Header("Chunks")]
    public float chunkLength = 20f;
    public int chunksAlive = 5;

    [Header("Lanes")]
    public int lanes = 3;
    public float laneWidth = 2.5f;

    [Header("Difficulty (0 = no obstacles, 1 = all lanes blocked)")]
    public int emptyStartChunks = 2;
    [Range(0f, 1f)] public float startDensity = 0.15f;
    [Range(0f, 1f)] public float maxDensity = 0.6f;
    public float densityIncreasePerChunk = 0.01f;

    [Header("Extras")]
    public bool spawnCoins = false;

    [Header("Randomness")]
    public bool useSeed = false;
    public int seed = 12345;

    const int RowsPerChunk = 4;

    class Chunk
    {
        public GameObject go;
        public int source;
        public GameObject[] obstacles;
        public GameObject[] coins;
    }

    readonly List<Chunk> active = new List<Chunk>();
    Queue<Chunk>[] pools;
    readonly List<GameObject> prefabs = new List<GameObject>();
    System.Random rng;
    float nextZ;
    int spawned;
    Material floorMat, obstacleMat, coinMat;
    bool warnedTag;

    void Start()
    {
        lanes = Mathf.Max(1, lanes);
        chunksAlive = Mathf.Max(3, chunksAlive);
        rng = useSeed ? new System.Random(seed) : new System.Random();

        foreach (var p in chunkPrefabs) if (p != null) prefabs.Add(p);
        pools = new Queue<Chunk>[Mathf.Max(1, prefabs.Count)];
        for (int i = 0; i < pools.Length; i++) pools[i] = new Queue<Chunk>();

        floorMat = MakeMat(new Color(0.35f, 0.38f, 0.45f));
        obstacleMat = MakeMat(new Color(0.9f, 0.25f, 0.25f));
        coinMat = MakeMat(new Color(1f, 0.85f, 0.1f));

        if (player == null)
        {
            var p = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            p.name = "Player (auto)";
            p.transform.position = new Vector3(0f, 1f, 0f);
            player = p.transform;
        }
        if (addAutoRunToPlayer && player.GetComponent<SimpleAutoRun>() == null)
            player.gameObject.AddComponent<SimpleAutoRun>();

        nextZ = Mathf.Floor(player.position.z / chunkLength) * chunkLength - chunkLength;
        while (nextZ < player.position.z + chunksAlive * chunkLength) SpawnChunk();
    }

    void Update()
    {
        // Spawn new chunks ahead of the player
        while (nextZ < player.position.z + chunksAlive * chunkLength) SpawnChunk();

        // Recycle chunks that are far behind the player
        while (active.Count > 0 &&
               active[0].go.transform.position.z + chunkLength < player.position.z - chunkLength)
        {
            var old = active[0];
            active.RemoveAt(0);
            old.go.SetActive(false);
            pools[old.source].Enqueue(old);
        }
    }

    void SpawnChunk()
    {
        int src = prefabs.Count > 0 ? rng.Next(prefabs.Count) : 0;
        Chunk c = pools[src].Count > 0 ? pools[src].Dequeue() : CreateChunk(src);

        c.go.transform.position = new Vector3(0f, 0f, nextZ);
        c.go.SetActive(true);

        if (prefabs.Count == 0)
        {
            float density = spawned < emptyStartChunks
                ? 0f
                : Mathf.Min(maxDensity, startDensity + (spawned - emptyStartChunks) * densityIncreasePerChunk);
            Populate(c, density);
        }

        active.Add(c);
        nextZ += chunkLength;
        spawned++;
    }

    Chunk CreateChunk(int src)
    {
        var c = new Chunk { source = src };

        if (prefabs.Count > 0)
        {
            c.go = Instantiate(prefabs[src], transform);
            return c;
        }

        // ---- Build a simple cube chunk: floor + obstacle slots + coin slots ----
        c.go = new GameObject("Chunk");
        c.go.transform.SetParent(transform);

        float width = lanes * laneWidth + 2f;
        var floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
        floor.name = "Floor";
        floor.transform.SetParent(c.go.transform, false);
        floor.transform.localPosition = new Vector3(0f, -0.5f, chunkLength / 2f);
        floor.transform.localScale = new Vector3(width, 1f, chunkLength);
        floor.GetComponent<Renderer>().sharedMaterial = floorMat;

        int slots = RowsPerChunk * lanes;
        c.obstacles = new GameObject[slots];
        c.coins = new GameObject[slots];
        float rowSpacing = chunkLength / RowsPerChunk;

        for (int row = 0; row < RowsPerChunk; row++)
        {
            for (int lane = 0; lane < lanes; lane++)
            {
                int i = row * lanes + lane;
                var pos = new Vector3(LaneX(lane), 0f, (row + 0.5f) * rowSpacing);

                var o = GameObject.CreatePrimitive(PrimitiveType.Cube);
                o.name = "Obstacle";
                o.transform.SetParent(c.go.transform, false);
                o.transform.localPosition = pos + Vector3.up * 0.5f;
                o.transform.localScale = new Vector3(laneWidth * 0.8f, 1f, 1f);
                o.GetComponent<Collider>().isTrigger = true;
                o.GetComponent<Renderer>().sharedMaterial = obstacleMat;
                TrySetTag(o, "Obstacle");
                c.obstacles[i] = o;

                var coin = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                coin.name = "Coin";
                coin.transform.SetParent(c.go.transform, false);
                coin.transform.localPosition = pos + Vector3.up * 1f;
                coin.transform.localScale = Vector3.one * 0.5f;
                coin.GetComponent<Collider>().isTrigger = true;
                coin.GetComponent<Renderer>().sharedMaterial = coinMat;
                TrySetTag(coin, "Coin");
                c.coins[i] = coin;
            }
        }
        return c;
    }

    // Randomly turn obstacle/coin slots on or off. Always leaves one lane free per row.
    void Populate(Chunk c, float density)
    {
        for (int row = 0; row < RowsPerChunk; row++)
        {
            var blocked = new bool[lanes];
            int blockedCount = 0;
            for (int lane = 0; lane < lanes; lane++)
            {
                blocked[lane] = rng.NextDouble() < density;
                if (blocked[lane]) blockedCount++;
            }
            if (blockedCount == lanes && lanes > 1) blocked[rng.Next(lanes)] = false;

            for (int lane = 0; lane < lanes; lane++)
            {
                int i = row * lanes + lane;
                c.obstacles[i].SetActive(blocked[lane]);
                c.coins[i].SetActive(spawnCoins && !blocked[lane] && rng.NextDouble() < 0.4);
            }
        }
    }

    public float LaneX(int lane) { return (lane - (lanes - 1) / 2f) * laneWidth; }

    Material MakeMat(Color color)
    {
        // Ask the project's render pipeline (URP/HDRP) for its own default shader.
        // If the project uses Built-in, there is no pipeline asset, so use "Standard".
        Shader s = null;
        var pipeline = UnityEngine.Rendering.GraphicsSettings.currentRenderPipeline;
        if (pipeline != null) s = pipeline.defaultShader;
        if (s == null) s = Shader.Find("Standard");
        if (s == null) s = Shader.Find("Sprites/Default");
        var m = new Material(s);
        m.color = color;
        return m;
    }

    void TrySetTag(GameObject go, string tagName)
    {
        try { go.tag = tagName; }
        catch (UnityException)
        {
            if (!warnedTag)
            {
                warnedTag = true;
                Debug.Log("Tip: add the tags \"Obstacle\" and \"Coin\" in Project Settings > Tags and Layers if you want to use them in your own code.");
            }
        }
    }
}
