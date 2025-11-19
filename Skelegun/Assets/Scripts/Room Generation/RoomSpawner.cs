using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.AI;
using NavMeshPlus.Components;
using System.Linq;

public class RoomSpawner : MonoBehaviour  //This is the new spawn system that should allow any size room, the old script is now "OldRoomGen", need to combine the two 
{

    [Header("Config")]
    public RoomPool pool;
    public int targetRooms = 10;
    public bool seeded = true;
    public int seedOverride = 12345;
    public bool showGizmos = true;
    public NavMeshSurface surface;
    private RoomManager entryRoom;


    // generation state:
    System.Random rnd;
    Dictionary<Vector2Int, RoomInstance> placedByCell = new Dictionary<Vector2Int, RoomInstance>();
    List<RoomInstance> placedRooms = new List<RoomInstance>();


    // helper - cardinal offsets in grid cells (one cell = pool.gridCellSize)
    readonly Dictionary<RoomDirection, Vector2Int> dirOffsets = new Dictionary<RoomDirection, Vector2Int> {
        {RoomDirection.North, new Vector2Int(0,1)},
        {RoomDirection.South, new Vector2Int(0,-1)},
        {RoomDirection.East, new Vector2Int(1,0)},
        {RoomDirection.West, new Vector2Int(-1,0)},
    };

    void Start()
    {
        if (pool == null)
        {
            Debug.LogError("RoomGenerator: pool is null");
            return;
        }
        rnd = seeded ? new System.Random(seedOverride) : new System.Random(System.Environment.TickCount ^ pool.seed);
        targetRooms = Mathf.Max(3, Mathf.Min(targetRooms, pool.roomsPerLevel));
        Generate();
        PlacePlayer();
        BakeMesh();
    }

    public void Generate()
    {

        ClearPrevious();
        // Place spawn at origin (grid 0,0)
        var spawnData = pool.roomPrefabs.FirstOrDefault(d => d.roomType == RoomType.Spawn);
        if (spawnData == null)

        {
            Debug.LogError("No spawn room in pool (RoomData with RoomType.Spawn required).");
            return;
        }


        PlaceInitialRoom(spawnData, Vector2Int.zero);

        // BFS-like frontier: each placed room exposes its exits for attempts to attach rooms
        Queue<RoomInstance> frontier = new Queue<RoomInstance>(placedRooms);
        int attempts = 0;
        while (placedRooms.Count < targetRooms && frontier.Count > 0 && attempts < targetRooms * 20)

        {
            attempts++;
            var current = frontier.Dequeue();

            // Determine how many exits to attempt:
            int exitCount = DecideExitCount(current.man);
            var directions = System.Enum.GetValues(typeof(RoomDirection)).Cast<RoomDirection>().OrderBy(x => rnd.Next()).Take(exitCount);

            foreach (var dir in directions)
            {
                // calculate target grid position for new room (place adjacent so exits align cell-to-cell)
                Vector2Int offset = dirOffsets[dir];
                // new room will be placed so that the adjacent cell(s) line up.
                // attempt to pick a prefab and place it so one of its exits faces back to current.
                if (placedRooms.Count >= targetRooms) break;

                // Try to pick a room that can be placed here respecting minDistance and not overlapping
                RoomInstance placed = TryPlaceRoomAtExit(current, dir);
                if (placed != null)
                {
                    frontier.Enqueue(placed);
                }
            }
        }

        // ensure special rooms (shop, boss) are present — if not, try to place them now
        EnsureSpecialRoom(RoomType.Shop);
        EnsureSpecialRoom(RoomType.Boss);

        // After placements, create corridors where exits face each other and no explicit door connected yet
        CreateCorridors();

        Debug.Log($"Room generation complete: placed {placedRooms.Count}/{targetRooms} rooms.");
    }

    void ClearPrevious()
    {
        foreach (var r in placedRooms)
        {
            if (r.go != null) Destroy(r.go);
        }
        placedRooms.Clear();
        placedByCell.Clear();
    }

    void PlaceInitialRoom(RoomManager data, Vector2Int gridOrigin)
    {
        var rt = data.prefab.GetComponent<RoomManager>();
        if (rt == null)
        {
            Debug.LogError($"Spawn prefab missing RoomTemplate: {data.prefab.name}");
            return;
        }

        var go = Instantiate(data.prefab, GridToWorld(gridOrigin, rt), Quaternion.identity, transform);
        var occ = ComputeOccupiedRect(gridOrigin, rt.gridSize);
        var inst = new RoomInstance(go, rt, gridOrigin, occ);
        RegisterRoomInstance(inst);
    }

    Vector3 GridToWorld(Vector2Int gridOrigin, RoomManager rt)
    {
        // convert grid cell origin to world position (bottom-left)
        Vector2 cell = new Vector2(pool.gridCellSize.x, pool.gridCellSize.y);
        return new Vector3(gridOrigin.x * cell.x, gridOrigin.y * cell.y, 0f);
    }

    RectInt ComputeOccupiedRect(Vector2Int gridOrigin, Vector2Int gridSize)
    {
        return new RectInt(gridOrigin.x, gridOrigin.y, gridSize.x, gridSize.y);
    }

    void RegisterRoomInstance(RoomInstance inst)
    {
        // fill occupied cells map
        for (int x = inst.occupied.x; x < inst.occupied.x + inst.occupied.width; x++)
        {
            for (int y = inst.occupied.y; y < inst.occupied.y + inst.occupied.height; y++)
            {
                var key = new Vector2Int(x, y);
                if (!placedByCell.ContainsKey(key)) placedByCell[key] = inst;
            }
        }
        placedRooms.Add(inst);
    }

    int DecideExitCount(RoomManager manRay)
    {
        if (manRay.forceExitCount) return manRay.forcedExitCount;
        int roll = rnd.Next(100);
        if (roll < 10) return 1;
        if (roll < 60) return 2;
        if (roll < 90) return 3;
        return 4;
    }

    RoomInstance TryPlaceRoomAtExit(RoomInstance current, RoomDirection fromDir)
    {
        // direction for new room relative to current
        var opposite = Opposite(fromDir);
        // We will attempt variants of candidate prefabs
        var candidates = pool.roomPrefabs.OrderBy(x => rnd.Next()).ToList();
        foreach (var candidate in candidates)
        {
            // skip if candidate is spawn
            if (candidate.roomType == RoomType.Spawn) continue;

            // Enforce minDistance if candidate requires it - compute Manhattan distance of candidate's putative grid position later
            // find the candidate's template
            if (candidate.prefab == null) continue;
            var rt = candidate.prefab.GetComponent<RoomManager>();
            if (rt == null) continue;

            // compute target grid origin for candidate such that the candidate's opposite exit lines up with current's exit.
            // We'll compute by picking grid cell adjacent in that direction and offsetting candidate so its adjacent side lines with current.
            // For simplicity: place candidate so that one of its occupied cells touches the adjacent cell in that direction.
            Vector2Int candidateGridOrigin = GetAdjacentGridPositionForCandidate(current, fromDir, rt.gridSize);
            var candidateRect = ComputeOccupiedRect(candidateGridOrigin, rt.gridSize);

            // check overlap
            if (DoesOverlap(candidateRect)) continue;

            // enforce candidate min distance
            int manhattan = Mathf.Abs(candidateGridOrigin.x) + Mathf.Abs(candidateGridOrigin.y);
            if (manhattan < candidate.minDistanceFromSpawn) continue;
            if (manhattan < rt.preferredMinDistanceFromSpawn) continue;

            // instantiate
            var worldPos = GridToWorld(candidateGridOrigin, rt);
            var go = Instantiate(candidate.prefab, worldPos, Quaternion.identity, transform);
            var inst = new RoomInstance(go, rt, candidateGridOrigin, candidateRect);
            RegisterRoomInstance(inst);
            return inst;
        }

        // if none fit, return null
        return null;
    }

    Vector2Int GetAdjacentGridPositionForCandidate(RoomInstance current, RoomDirection fromDir, Vector2Int candidateSize)
    {
        // place candidate so that its side adjacent to current lines up.
        // Example: current at (cx,cy) with size cw,ch. For East placement, candidateGridOrigin.x = current.occupied.x + current.occupied.width
        switch (fromDir)
        {
            case RoomDirection.North:
                return new Vector2Int(current.occupied.x, current.occupied.y + current.occupied.height);
            case RoomDirection.South:
                return new Vector2Int(current.occupied.x, current.occupied.y - candidateSize.y);
            case RoomDirection.East:
                return new Vector2Int(current.occupied.x + current.occupied.width, current.occupied.y);
            case RoomDirection.West:
                return new Vector2Int(current.occupied.x - candidateSize.x, current.occupied.y);
            default:
                return current.occupied.position;
        }
    }

    bool DoesOverlap(RectInt rect)
    {
        for (int x = rect.x; x < rect.x + rect.width; x++)
        {
            for (int y = rect.y; y < rect.y + rect.height; y++)
            {
                if (placedByCell.ContainsKey(new Vector2Int(x, y))) return true;
            }
        }
        return false;
    }

    RoomDirection Opposite(RoomDirection d)
    {
        switch (d)
        {
            case RoomDirection.North: return RoomDirection.South;
            case RoomDirection.South: return RoomDirection.North;
            case RoomDirection.East: return RoomDirection.West;
            case RoomDirection.West: return RoomDirection.East;
        }
        return RoomDirection.North;
    }

    void EnsureSpecialRoom(RoomType type)
    {
        if (placedRooms.Any(r => r.man.roomType == type)) return;
        // Try to place by finding candidate grid positions that are not overlapping and satisfy distance constraints
        var candidates = placedRooms.OrderBy(r => rnd.Next()).ToList();
        foreach (var spot in candidates)
        {
            // try each direction around spot
            foreach (RoomDirection dir in System.Enum.GetValues(typeof(RoomDirection)))
            {
                // pick data for type
                var data = pool.roomPrefabs.FirstOrDefault(d => d.roomType == type);
                if (data == null) continue;
                var rt = data.prefab.GetComponent<RoomManager>();
                Vector2Int pos = GetAdjacentGridPositionForCandidate(spot, dir, rt.gridSize);
                var rect = ComputeOccupiedRect(pos, rt.gridSize);
                if (DoesOverlap(rect)) continue;
                // distance check
                int manhattan = Mathf.Abs(pos.x) + Mathf.Abs(pos.y);
                if (manhattan < data.minDistanceFromSpawn) continue;
                // place it
                var world = GridToWorld(pos, rt);
                var go = Instantiate(data.prefab, world, Quaternion.identity, transform);
                var inst = new RoomInstance(go, rt, pos, rect);
                RegisterRoomInstance(inst);
                Debug.Log($"Placed special room {type} at {pos}");
                return;
            }
        }
        Debug.LogWarning($"Could not place special room {type} - generation continuing without it.");
    }

    void CreateCorridors()
    {
        if (pool.corridorPrefab == null) return;
        // look for adjacent placed cells that are exits and not yet connected: for simplicity, we will create corridors between exit anchors of rooms that occupy adjacent grid cells and whose templates have matching exits.
        foreach (var r in placedRooms)
        {
            foreach (RoomDirection dir in System.Enum.GetValues(typeof(RoomDirection)))
            {
                var anchor = r.man.GetExit(dir);
                if (anchor == null) continue; // no exit
                // find neighbor cell in that dir
                Vector2Int neighborPos = r.occupied.position + dirOffsets[dir] * 1; // one cell away
                // find room occupying that neighbor cell
                if (placedByCell.TryGetValue(neighborPos, out RoomInstance neighbor) && neighbor != r)
                {
                    // check neighbor has opposite exit
                    var opp = neighbor.man.GetExit(Opposite(dir));
                    if (opp == null) continue;
                    // compute world positions and whether corridor already exists (we can detect by checking distance of existing corridors or by attaching components; for simplicity create one only when r.gridPos < neighbor.gridPos to avoid double creation)
                    if (r.gridPos.x > neighbor.gridPos.x || (r.gridPos.x == neighbor.gridPos.x && r.gridPos.y > neighbor.gridPos.y)) continue;
                    Vector3 a = anchor.position;
                    Vector3 b = opp.position;
                    SpawnCorridorBetween(a, b);
                }
                else
                {
                    // neighbor missing: that's a dead-end. optionally spawn a short dead-end corridor or leave as is.
                }
            }
        }
    }

    void SpawnCorridorBetween(Vector3 a, Vector3 b)
    {
        var dir = b - a;
        var mid = (a + b) / 2f;
        var go = Instantiate(pool.corridorPrefab, mid, Quaternion.identity, transform);
        // assume corridor prefab is 1x1 in local scale and oriented along x axis; scale length accordingly and rotate to match dir
        float length = dir.magnitude;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        go.transform.rotation = Quaternion.Euler(0, 0, angle);
        // scale x to match length (assumes prefab's width is 1 world unit)
        var s = go.transform.localScale;
        go.transform.localScale = new Vector3(length / 1f, s.y, s.z);
    }

    void PlacePlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        player.transform.position = new Vector2(0, 0);
        /*
        if (player != null && entryRoom != null && entryRoom.doorEntry != null)
        {
            player.transform.position = entryRoom.doorEntry.position;
        }
        */
    }
    void BakeMesh()
    {
        if (surface != null)
        {
            surface.BuildNavMesh();
            Debug.Log("NavMesh rebuilt at runtime!");
        }

    }
}




       



/*  Don't remember what this was for, maybe an inbetween the OG and this one?

  [Header("Room Prefabs")]
    public GameObject entryRoomPrefab;
    public GameObject combatRoomPrefab;
    public GameObject shopRoomPrefab;
    public GameObject rewardRoomPrefab;
    public GameObject bossRoomPrefab;

    [Header("Settings")]
    public int combatRoomCount = 2;
  
    private List<RoomManager> spawnedRooms = new List<RoomManager>();
    void Start()
    {
        GenerateDungeon();
        PlacePlayer();
        BakeMesh();
    }
    void GenerateDungeon()
    {
        RoomManager previousRoom = null;
        // Entry
        entryRoom = SpawnRoom(entryRoomPrefab, null);
        previousRoom = entryRoom;
        // Combat rooms
        for (int i = 0; i < combatRoomCount; i++)
        {
            RoomManager combat = SpawnRoom(combatRoomPrefab, previousRoom.doorExit);
            previousRoom = combat;
        }
        // Shop
        RoomManager shop = SpawnRoom(shopRoomPrefab, previousRoom.doorExit);
        previousRoom = shop;
        // Reward
        RoomManager reward = SpawnRoom(rewardRoomPrefab, previousRoom.doorExit);
        previousRoom = reward;
        // Boss
        RoomManager boss = SpawnRoom(bossRoomPrefab, previousRoom.doorExit);
        previousRoom = boss;
    }
    RoomManager SpawnRoom(GameObject prefab, Transform previousExit)
    {
        GameObject roomObj = Instantiate(prefab, Vector3.zero, Quaternion.identity);
        RoomManager rm = roomObj.GetComponent<RoomManager>();

        if (previousExit != null && rm.doorEntry != null)
        {
            // Offset = previous exit position - this entry position
            Vector3 offset = previousExit.position - rm.doorEntry.position;
            roomObj.transform.position += offset;
        }
        spawnedRooms.Add(rm);
        return rm;
}
*/

