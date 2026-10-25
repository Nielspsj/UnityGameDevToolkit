// =====================================================================
//  EndlessRunnerGenerator.cs  (Unity 6, works with Built-in, URP and HDRP)
//  Written so that beginners can read it from top to bottom.
//
//  HOW TO SET UP (30 seconds):
//   1. Put EndlessRunnerGenerator.cs AND SimpleAutoRun.cs in your Assets folder.
//   2. In the Hierarchy: right-click > Create Empty. Name it "Generator".
//   3. Select "Generator" > Add Component > Endless Runner Generator.
//   4. (Optional) Drag your player from the Hierarchy into the "Player" slot.
//      Leave it empty and a capsule player is created for you.
//   5. Press Play.  A/D or Left/Right = change lane.  Space/W/Up = jump.
//
//  OPTIONAL: To make the "Obstacle" and "Coin" tags work, go to
//  Edit > Project Settings > Tags and Layers > Tags > "+" and add them.
//  (Everything works without them.)
//
//  USING YOUR OWN CHUNK PREFABS (optional):
//   Sort your prefabs into the Easy / Medium / Hard lists. A prefab's pivot
//   must sit at the START of the chunk (the near edge), and it should be as
//   long as "Chunk Length" along the Z axis.
//   If ALL lists are empty, simple cube chunks are generated automatically.
//
//  THE BIG IDEA:
//   Imagine a conveyor belt of floor tiles (chunks) in front of the player.
//   When a tile is far behind the player, we switch it off and later put it
//   in front again. We never destroy and re-create tiles, so it stays smooth.
// =====================================================================
using System.Collections.Generic;
using UnityEngine;

public class EndlessRunnerGenerator1 : MonoBehaviour
{
    // -----------------------------------------------------------------
    //  SETTINGS  (everything "public" shows up in the Inspector)
    //  [Header("...")] only adds a label above a group in the Inspector.
    // -----------------------------------------------------------------

    [Header("Player (leave empty to auto-create a capsule)")]
    public Transform player;
    public bool addAutoRunToPlayer = true;

    [Header("Optional: your own chunk prefabs, sorted by difficulty")]
    // FormerlySerializedAs keeps prefabs you assigned in an older version.
    [UnityEngine.Serialization.FormerlySerializedAs("chunkPrefabs")]
    public GameObject[] easyChunks;
    public GameObject[] mediumChunks;
    public GameObject[] hardChunks;
    public GameObject safeChunk;          // optional calm chunk for the very start

    [Header("Tier unlocking (counted in chunks spawned)")]
    public int mediumUnlockChunk = 10;
    public int hardUnlockChunk = 30;
    public float harderTierBias = 0.1f;   // higher = harder chunks appear more often over time

    [Header("Chunks")]
    public float chunkLength = 20f;
    public int chunksAlive = 5;

    [Header("Lanes")]
    public int lanes = 3;
    public float laneWidth = 2.5f;

    [Header("Generated chunks only")]
    public int obstacleRows = 4;          // how many rows of obstacles inside one chunk

    [Header("Difficulty of generated chunks (0 = empty, 1 = every lane blocked)")]
    public int emptyStartChunks = 2;
    [Range(0f, 1f)] public float startDensity = 0.15f;
    [Range(0f, 1f)] public float maxDensity = 0.6f;
    public float densityIncreasePerChunk = 0.01f;

    [Header("Extras")]
    public bool spawnCoins = false;

    [Header("Randomness")]
    public bool useSeed = false;
    public int seed = 12345;

    // -----------------------------------------------------------------
    //  PRIVATE DATA  (the script's memory; not shown in the Inspector)
    // -----------------------------------------------------------------

    // A small "box" that remembers everything about one chunk.
    class Chunk
    {
        public GameObject go;               // the chunk object in the scene
        public int source;                  // which prefab it came from (0 for generated)
        public GameObject[,] obstacles;     // obstacles[row, lane]  (generated chunks only)
        public GameObject[,] coins;         // coins[row, lane]      (generated chunks only)
    }

    List<Chunk> active = new List<Chunk>();     // chunks in the world right now, oldest first
    List<Chunk> sleeping = new List<Chunk>();   // switched-off chunks waiting to be reused

    List<GameObject> prefabs = new List<GameObject>();  // all your prefabs in one list
    List<int> easyIndexes = new List<int>();     // positions in 'prefabs' that are Easy
    List<int> mediumIndexes = new List<int>();   // ...that are Medium
    List<int> hardIndexes = new List<int>();     // ...that are Hard
    int safeIndex = -1;                          // position of the safe chunk (-1 = none)

    System.Random rng;          // our random number generator
    float nextZ;                // Z position where the next chunk will be placed
    int spawned;                // how many chunks we have placed so far
    float startPlayerZ;         // where the player began (used for the warning below)
    bool warnedStuck = false;
    bool warnedTag = false;

    Material floorMat;
    Material obstacleMat;
    Material coinMat;

    // -----------------------------------------------------------------
    //  Start() runs ONCE when you press Play
    // -----------------------------------------------------------------
    void Start()
    {
        // Make sure the settings make sense.
        if (lanes < 1) lanes = 1;
        if (chunksAlive < 3) chunksAlive = 3;

        // Create the random number generator.
        if (useSeed == true)
        {
            rng = new System.Random(seed);   // same seed = same level every time
        }
        else
        {
            rng = new System.Random();       // different level every time
        }

        BuildTierLists();

        // Create three colored materials for the generated cubes.
        floorMat = MakeMaterial(new Color(0.35f, 0.38f, 0.45f));
        obstacleMat = MakeMaterial(new Color(0.9f, 0.25f, 0.25f));
        coinMat = MakeMaterial(new Color(1f, 0.85f, 0.1f));

        // No player assigned? Make a capsule.
        if (player == null)
        {
            GameObject capsule = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            capsule.name = "Player (auto)";
            capsule.transform.position = new Vector3(0f, 1f, 0f);
            player = capsule.transform;
        }

        // Give the player the simple run/jump script if it has no such script yet.
        if (addAutoRunToPlayer == true)
        {
            if (player.GetComponent<SimpleAutoRun>() == null)
            {
                player.gameObject.AddComponent<SimpleAutoRun>();
            }
        }

        // If the Player slot contains a prefab from the Project window, warn.
        if (player.gameObject.scene.IsValid() == false)
        {
            Debug.LogError("The Player slot holds a prefab from the Project window. Drag the player from the Hierarchy (the copy in your scene) instead.");
        }

        startPlayerZ = player.position.z;

        // Start the first chunk one chunk-length behind the player.
        nextZ = Mathf.Floor(player.position.z / chunkLength) * chunkLength - chunkLength;

        // Fill the world in front of the player.
        float fillUntil = player.position.z + chunksAlive * chunkLength;
        while (nextZ < fillUntil)
        {
            SpawnChunk();
        }
    }

    // -----------------------------------------------------------------
    //  Update() runs EVERY frame
    // -----------------------------------------------------------------
    void Update()
    {
        // Warn if the player never moves forward (then no new chunks can appear).
        if (warnedStuck == false && Time.timeSinceLevelLoad > 3f)
        {
            float distanceRun = player.position.z - startPlayerZ;
            if (distanceRun < 1f)
            {
                warnedStuck = true;
                Debug.LogWarning("The player has not moved forward along +Z, so no new chunks spawn. Make sure your player runs toward +Z. If you use your own movement script, untick 'Add Auto Run To Player' on the generator.");
            }
        }

        // 1) Spawn new chunks in front of the player.
        float fillUntil = player.position.z + chunksAlive * chunkLength;
        while (nextZ < fillUntil)
        {
            SpawnChunk();
        }

        // 2) Put chunks that are far behind the player to sleep.
        while (active.Count > 0)
        {
            Chunk oldest = active[0];
            float oldestEndZ = oldest.go.transform.position.z + chunkLength;
            bool farBehind = oldestEndZ < player.position.z - chunkLength;

            if (farBehind == false)
            {
                break;   // the oldest chunk is still close, so all others are too
            }

            active.RemoveAt(0);
            oldest.go.SetActive(false);
            sleeping.Add(oldest);
        }
    }

    // -----------------------------------------------------------------
    //  Place one new chunk at nextZ
    // -----------------------------------------------------------------
    void SpawnChunk()
    {
        // Decide which prefab to use (or 0 for a generated chunk).
        int source = PickSource();

        // Reuse a sleeping chunk if one exists, otherwise build a new one.
        Chunk chunk = TakeSleepingChunk(source);
        if (chunk == null)
        {
            chunk = CreateChunk(source);
        }

        chunk.go.transform.position = new Vector3(0f, 0f, nextZ);
        chunk.go.SetActive(true);

        // Generated chunks get random obstacles. Prefabs are left as they are.
        if (prefabs.Count == 0)
        {
            float density = 0f;
            if (spawned >= emptyStartChunks)
            {
                int chunksSinceStart = spawned - emptyStartChunks;
                density = startDensity + chunksSinceStart * densityIncreasePerChunk;
                if (density > maxDensity)
                {
                    density = maxDensity;
                }
            }
            FillChunkWithObstacles(chunk, density);
        }

        active.Add(chunk);
        nextZ = nextZ + chunkLength;
        spawned = spawned + 1;
    }

    // Looks through the sleeping chunks for one of the right kind.
    // Returns null if there is none.
    Chunk TakeSleepingChunk(int source)
    {
        for (int i = 0; i < sleeping.Count; i++)
        {
            if (sleeping[i].source == source)
            {
                Chunk found = sleeping[i];
                sleeping.RemoveAt(i);
                return found;
            }
        }
        return null;
    }

    // -----------------------------------------------------------------
    //  Difficulty tiers (only used when you assigned your own prefabs)
    // -----------------------------------------------------------------

    // Puts all prefabs into one list and remembers which are Easy/Medium/Hard.
    void BuildTierLists()
    {
        AddPrefabsToTier(easyChunks, easyIndexes);
        AddPrefabsToTier(mediumChunks, mediumIndexes);
        AddPrefabsToTier(hardChunks, hardIndexes);

        if (safeChunk != null)
        {
            safeIndex = prefabs.Count;
            prefabs.Add(safeChunk);
        }
    }

    void AddPrefabsToTier(GameObject[] chunkArray, List<int> tierIndexes)
    {
        if (chunkArray == null)
        {
            return;
        }
        for (int i = 0; i < chunkArray.Length; i++)
        {
            if (chunkArray[i] != null)
            {
                tierIndexes.Add(prefabs.Count);   // remember the position in the list
                prefabs.Add(chunkArray[i]);
            }
        }
    }

    // Decides which prefab comes next. Returns an index into 'prefabs',
    // or 0 when we are making generated chunks.
    int PickSource()
    {
        // No prefabs at all: we build generated chunks.
        if (prefabs.Count == 0)
        {
            return 0;
        }

        // The very first chunks use the safe chunk, if you gave us one.
        if (spawned < emptyStartChunks && safeIndex >= 0)
        {
            return safeIndex;
        }

        // Each tier gets a "weight". Bigger weight = more likely to be picked.
        // A weight of 0 means the tier is empty or not unlocked yet.
        float easyWeight = 0f;
        float mediumWeight = 0f;
        float hardWeight = 0f;

        if (easyIndexes.Count > 0)
        {
            easyWeight = 1f;
        }

        if (mediumIndexes.Count > 0 && spawned >= mediumUnlockChunk)
        {
            int chunksSinceUnlock = spawned - mediumUnlockChunk;
            mediumWeight = 1f + chunksSinceUnlock * harderTierBias;
        }

        if (hardIndexes.Count > 0 && spawned >= hardUnlockChunk)
        {
            int chunksSinceUnlock = spawned - hardUnlockChunk;
            hardWeight = 1f + chunksSinceUnlock * harderTierBias;
        }

        float totalWeight = easyWeight + mediumWeight + hardWeight;

        // Nothing is unlocked yet? Use the easiest tier that has chunks.
        if (totalWeight <= 0f)
        {
            if (easyIndexes.Count > 0) return PickFromTier(easyIndexes);
            if (mediumIndexes.Count > 0) return PickFromTier(mediumIndexes);
            if (hardIndexes.Count > 0) return PickFromTier(hardIndexes);
            return safeIndex;
        }

        // Roll a dice between 0 and the total weight, then see where it lands.
        // Example: weights Easy 1, Medium 3 -> total 4.
        // A roll under 1 means Easy, anything above means Medium.
        float roll = (float)rng.NextDouble() * totalWeight;

        if (roll < easyWeight)
        {
            return PickFromTier(easyIndexes);
        }
        roll = roll - easyWeight;

        if (roll < mediumWeight)
        {
            return PickFromTier(mediumIndexes);
        }

        if (hardWeight > 0f)
        {
            return PickFromTier(hardIndexes);
        }
        if (mediumWeight > 0f)
        {
            return PickFromTier(mediumIndexes);
        }
        return PickFromTier(easyIndexes);
    }

    // Picks a random prefab from one tier.
    int PickFromTier(List<int> tierIndexes)
    {
        int pick = rng.Next(tierIndexes.Count);
        return tierIndexes[pick];
    }

    // -----------------------------------------------------------------
    //  Building a chunk (only runs when no sleeping chunk can be reused)
    // -----------------------------------------------------------------
    Chunk CreateChunk(int source)
    {
        Chunk chunk = new Chunk();
        chunk.source = source;

        // Case 1: your own prefab. Just copy it.
        if (prefabs.Count > 0)
        {
            chunk.go = Instantiate(prefabs[source], transform);
            return chunk;
        }

        // Case 2: build a chunk out of cubes and spheres.
        chunk.go = new GameObject("Chunk");
        chunk.go.transform.SetParent(transform);

        // The floor: one flat, wide cube.
        float floorWidth = lanes * laneWidth + 2f;
        GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
        floor.name = "Floor";
        floor.transform.SetParent(chunk.go.transform, false);
        floor.transform.localPosition = new Vector3(0f, -0.5f, chunkLength / 2f);
        floor.transform.localScale = new Vector3(floorWidth, 1f, chunkLength);
        floor.GetComponent<Renderer>().sharedMaterial = floorMat;

        // Make EVERY possible obstacle and coin once. Later we only switch
        // them on and off, which is much cheaper than creating new ones.
        chunk.obstacles = new GameObject[obstacleRows, lanes];
        chunk.coins = new GameObject[obstacleRows, lanes];
        float rowSpacing = chunkLength / obstacleRows;

        for (int row = 0; row < obstacleRows; row++)
        {
            for (int lane = 0; lane < lanes; lane++)
            {
                float x = LaneX(lane);
                float z = (row + 0.5f) * rowSpacing;

                // Obstacle: a red cube.
                GameObject obstacle = GameObject.CreatePrimitive(PrimitiveType.Cube);
                obstacle.name = "Obstacle";
                obstacle.transform.SetParent(chunk.go.transform, false);
                obstacle.transform.localPosition = new Vector3(x, 0.5f, z);
                obstacle.transform.localScale = new Vector3(laneWidth * 0.8f, 1f, 1f);
                obstacle.GetComponent<Renderer>().sharedMaterial = obstacleMat;
                TrySetTag(obstacle, "Obstacle");
                chunk.obstacles[row, lane] = obstacle;

                // Coin: a small yellow sphere you can walk through (isTrigger).
                GameObject coin = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                coin.name = "Coin";
                coin.transform.SetParent(chunk.go.transform, false);
                coin.transform.localPosition = new Vector3(x, 1f, z);
                coin.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                coin.GetComponent<Collider>().isTrigger = true;
                coin.GetComponent<Renderer>().sharedMaterial = coinMat;
                TrySetTag(coin, "Coin");
                chunk.coins[row, lane] = coin;
            }
        }

        return chunk;
    }

    // Randomly switches obstacles and coins on or off.
    // 'density' is the chance (0 to 1) that a lane in a row is blocked.
    void FillChunkWithObstacles(Chunk chunk, float density)
    {
        for (int row = 0; row < obstacleRows; row++)
        {
            // Step 1: roll the dice for every lane in this row.
            bool[] blocked = new bool[lanes];
            int blockedCount = 0;

            for (int lane = 0; lane < lanes; lane++)
            {
                double dice = rng.NextDouble();    // a number between 0 and 1
                if (dice < density)
                {
                    blocked[lane] = true;
                    blockedCount = blockedCount + 1;
                }
            }

            // Step 2: safety rule. Never block ALL lanes, or the player is trapped.
            if (blockedCount == lanes && lanes > 1)
            {
                int freeLane = rng.Next(lanes);
                blocked[freeLane] = false;
            }

            // Step 3: switch obstacles and coins on or off.
            for (int lane = 0; lane < lanes; lane++)
            {
                chunk.obstacles[row, lane].SetActive(blocked[lane]);

                // Coins only appear in free lanes, and only 40% of the time.
                bool showCoin = false;
                if (spawnCoins == true && blocked[lane] == false)
                {
                    double coinDice = rng.NextDouble();
                    if (coinDice < 0.4)
                    {
                        showCoin = true;
                    }
                }
                chunk.coins[row, lane].SetActive(showCoin);
            }
        }
    }

    // -----------------------------------------------------------------
    //  Small helper functions
    // -----------------------------------------------------------------

    // Turns a lane number into an X position. With 3 lanes of width 2.5:
    // lane 0 -> -2.5,  lane 1 -> 0,  lane 2 -> +2.5
    // (SimpleAutoRun uses this too, so it is "public".)
    public float LaneX(int lane)
    {
        float middleLane = (lanes - 1) / 2f;
        return (lane - middleLane) * laneWidth;
    }

    // Makes a colored material that works in Built-in, URP and HDRP.
    Material MakeMaterial(Color color)
    {
        Shader shader = null;

        // If the project uses URP or HDRP, ask it for its own default shader.
        UnityEngine.Rendering.RenderPipelineAsset pipeline = UnityEngine.Rendering.GraphicsSettings.currentRenderPipeline;
        if (pipeline != null)
        {
            shader = pipeline.defaultShader;
        }

        // Otherwise use the Built-in shader.
        if (shader == null)
        {
            shader = Shader.Find("Standard");
        }
        if (shader == null)
        {
            shader = Shader.Find("Sprites/Default");
        }

        Material material = new Material(shader);
        material.color = color;
        return material;
    }

    // Tries to give an object a tag. If the tag does not exist in your project,
    // Unity throws an error. "try/catch" catches it so the game keeps running.
    void TrySetTag(GameObject target, string tagName)
    {
        try
        {
            target.tag = tagName;
        }
        catch (UnityException)
        {
            if (warnedTag == false)
            {
                warnedTag = true;
                Debug.Log("Tip: add the tags \"Obstacle\" and \"Coin\" in Project Settings > Tags and Layers if you want to use them in your own code.");
            }
        }
    }
}
