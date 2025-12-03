using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Generator: places rooms from a level chart, prevents overlaps, and connects doors/corridors.
/// Requires RoomInstance to implement:
/// - ComputeWorldBounds()
/// - BuildDoorWorldPositions()
/// </summary>
public class Generator : MonoBehaviour
{
    [Header("Level Design")]
    [Tooltip("List of available level charts to randomly choose from")]
    public List<LevelChart> levelChartOptions = new List<LevelChart>();

    [Header("Generation Settings")]
    [Tooltip("Parent transform for spawned rooms")]
    public Transform roomParent;

    [Header("Corridor Settings")]
    [Tooltip("Prefab used for corridors between non-adjacent doors")]
    public GameObject corridorPrefab;

    [Tooltip("Maximum distance to try direct door-to-door connection (in tiles)")]
    public float maxDirectConnectionDistance = 2f;

    // Runtime data
    private Dictionary<string, RoomInstance> runtimeRooms = new Dictionary<string, RoomInstance>();
    private HashSet<RoomData> usedRoomPrefabs = new HashSet<RoomData>();
    private LevelChart chosenChart;
    private HashSet<string> placedNodes = new HashSet<string>();

    void Start()
    {
        Generate();
    }

    [ContextMenu("Generate Level")]
    public void Generate()
    {
        ClearLevel();

        if (!ValidateSetup())
            return;

        PickLevelChart();
        PlaceRoomsFromGraph();
        ConnectRooms();

        Debug.Log($"Level generation complete: {runtimeRooms.Count} rooms spawned");
    }

    bool ValidateSetup()
    {
        if (levelChartOptions == null || levelChartOptions.Count == 0)
        {
            Debug.LogError("Generator: No level charts assigned!");
            return false;
        }

        if (roomParent == null)
        {
            Debug.LogWarning("Generator: No room parent assigned, creating one");
            roomParent = new GameObject("Generated Rooms").transform;
        }

        return true;
    }

    void ClearLevel()
    {
        runtimeRooms.Clear();
        placedNodes.Clear();
        usedRoomPrefabs.Clear();

        if (roomParent != null)
        {
            foreach (Transform child in roomParent)
            {
                Destroy(child.gameObject);
            }
        }
    }

    void PickLevelChart()
    {
        chosenChart = levelChartOptions[Random.Range(0, levelChartOptions.Count)];
    }

    // ------------------------------------------------------
    // Graph-based placement: place rooms relative to connections
    // ------------------------------------------------------
    void PlaceRoomsFromGraph()
    {
        if (chosenChart.nodes.Count == 0)
        {
            Debug.LogError("Level chart has no nodes!");
            return;
        }

        // Find starting node (spawn room or first node)
        LevelNode startNode = FindStartNode();

        // Place start room at origin
        PlaceRoom(startNode, Vector2Int.zero);

        // Use BFS to place connected rooms
        Queue<string> toProcess = new Queue<string>();
        toProcess.Enqueue(startNode.id);

        while (toProcess.Count > 0)
        {
            string currentNodeID = toProcess.Dequeue();

            if (!runtimeRooms.ContainsKey(currentNodeID))
                continue;

            RoomInstance currentRoom = runtimeRooms[currentNodeID];
            LevelNode currentNode = chosenChart.GetNode(currentNodeID);

            // Place all linked rooms
            foreach (string linkedID in currentNode.connectedNodeIDs)
            {
                if (placedNodes.Contains(linkedID))
                    continue; // Already placed

                LevelNode linkedNode = chosenChart.GetNode(linkedID);
                if (linkedNode == null)
                {
                    Debug.LogWarning($"Linked node '{linkedID}' not found in chart!");
                    continue;
                }

                // Calculate a raw candidate position (uses existing room as anchor)
                Vector2Int rawPosition = CalculateAdjacentPosition(currentRoom, linkedNode);

                // Determine primary placement direction from editor hints
                Vector2 gridDelta = linkedNode.editorPosition - currentRoom.editorPosition;
                Vector2Int direction = Vector2Int.zero;
                if (Mathf.Abs(gridDelta.x) > Mathf.Abs(gridDelta.y))
                    direction.x = gridDelta.x > 0 ? 1 : -1;
                else
                    direction.y = gridDelta.y > 0 ? 1 : -1;

                // Approximate size: prefer new node pool sample if available, otherwise use existing room size
                Vector2Int sampleSize = currentRoom.data.roomSize;

                // Find a non-overlapping position by nudging outward if needed
                Vector2Int finalPos = FindNonOverlappingPosition(rawPosition, sampleSize, direction, 12);

                PlaceRoom(linkedNode, finalPos);

                toProcess.Enqueue(linkedID);
            }
        }
    }

    LevelNode FindStartNode()
    {
        // Try to find spawn room
        LevelNode spawnNode = chosenChart.nodes.Find(n => n.roomType == RoomType.Spawn);

        if (spawnNode != null)
            return spawnNode;

        // Otherwise use first node
        return chosenChart.nodes[0];
    }

    void PlaceRoom(LevelNode node, Vector2Int worldPosition)
    {
        if (node.pool == null)
        {
            Debug.LogError($"Node '{node.id}' has no RoomPool assigned!");
            return;
        }

        if (!node.pool.IsValid())
        {
            Debug.LogError($"Node '{node.id}' has an invalid pool!");
            return;
        }

        // Get random unused room from the shared pool
        RoomData chosenPrefab = node.pool.GetRandomRoom(usedRoomPrefabs);

        if (chosenPrefab == null)
        {
            Debug.LogError($"Failed to get room from pool for node '{node.id}'");
            return;
        }

        // Mark this room as used
        usedRoomPrefabs.Add(chosenPrefab);

        // Instantiate room
        Vector3 worldPos = new Vector3(worldPosition.x, worldPosition.y, 0);
        GameObject roomObj = Instantiate(chosenPrefab.gameObject, worldPos, Quaternion.identity, roomParent);
        roomObj.name = $"Room_{node.displayName}_{chosenPrefab.name}";

        // Create runtime instance
        RoomInstance instance = new RoomInstance(chosenPrefab);
        instance.Initialize(roomObj, worldPos, Vector2Int.RoundToInt(node.editorPosition));

        // IMPORTANT: compute accurate bounds and door world positions immediately
        // (RoomInstance must implement these methods)
        instance.ComputeWorldBounds();
        instance.BuildDoorWorldPositions();

        runtimeRooms.Add(node.id, instance);
        placedNodes.Add(node.id);

        Debug.Log($"Placed room '{chosenPrefab.name}' for node '{node.displayName}' at {worldPosition}");
    }

    Vector2Int CalculateAdjacentPosition(RoomInstance existingRoom, LevelNode newNode)
    {
        // Use editorPosition as a hint for direction
        Vector2 gridDelta = newNode.editorPosition - existingRoom.editorPosition;

        // Normalize to get primary direction
        Vector2Int direction = Vector2Int.zero;
        if (Mathf.Abs(gridDelta.x) > Mathf.Abs(gridDelta.y))
        {
            direction.x = gridDelta.x > 0 ? 1 : -1; // East or West
        }
        else
        {
            direction.y = gridDelta.y > 0 ? 1 : -1; // North or South
        }

        // Use existing room bounds as anchor. This returns a lower-left tile coordinate
        int candidateX = existingRoom.bounds.xMin;
        int candidateY = existingRoom.bounds.yMin;

        if (direction.x > 0) // Place to the east
        {
            candidateX = existingRoom.bounds.xMax;
        }
        else if (direction.x < 0) // Place to the west
        {
            // Approximate new room width using existing room width (safe fallback)
            candidateX = existingRoom.bounds.xMin - existingRoom.data.roomSize.x;
        }
        else if (direction.y > 0) // Place to the north
        {
            candidateY = existingRoom.bounds.yMax;
        }
        else if (direction.y < 0) // Place to the south
        {
            candidateY = existingRoom.bounds.yMin - existingRoom.data.roomSize.y;
        }

        return new Vector2Int(candidateX, candidateY);
    }

    // ------------------------------------------------------
    // Connect rooms via doors (direct or with corridors)
    // ------------------------------------------------------
    void ConnectRooms()
    {
        foreach (var node in chosenChart.nodes)
        {
            if (!runtimeRooms.ContainsKey(node.id))
                continue;

            RoomInstance roomA = runtimeRooms[node.id];

            foreach (string linkedNodeID in node.connectedNodeIDs)
            {
                if (!runtimeRooms.ContainsKey(linkedNodeID))
                {
                    Debug.LogWarning($"Node '{node.id}' links to missing node '{linkedNodeID}'");
                    continue;
                }

                RoomInstance roomB = runtimeRooms[linkedNodeID];

                // Skip if already connected (bidirectional)
                if (roomA.IsConnectedTo(roomB))
                    continue;

                ConnectTwoRooms(roomA, roomB);
            }
        }
    }

    void ConnectTwoRooms(RoomInstance roomA, RoomInstance roomB)
    {
        // Refresh bounds/doors just before selecting doors
        roomA.ComputeWorldBounds();
        roomA.BuildDoorWorldPositions();
        roomB.ComputeWorldBounds();
        roomB.BuildDoorWorldPositions();

        // Find best matching doors
        DoorLocation doorA = roomA.GetBestDoorTowards(roomB);
        DoorLocation doorB = roomB.GetBestDoorTowards(roomA);

        if (doorA == null || doorB == null)
        {
            Debug.LogWarning($"Could not find suitable doors between rooms");
            return;
        }

        // Validate directions are opposite
        if (!AreDirectionsOpposite(doorA.direction, doorB.direction))
        {
            Debug.LogWarning($"Door directions not opposite: {doorA.direction} vs {doorB.direction}");
        }

        // Register connections
        roomA.AddConnection(doorA, roomB);
        roomB.AddConnection(doorB, roomA);

        // Check if doors need a corridor
        float distance = Vector2.Distance(doorA.worldPosition, doorB.worldPosition);

        if (distance > maxDirectConnectionDistance)
        {
            SpawnCorridor(doorA.worldPosition, doorB.worldPosition);
        }
        else
        {
            Debug.Log($"Direct door-to-door connection (distance: {distance:F1})");
        }
    }

    bool AreDirectionsOpposite(DoorDirection dirA, DoorDirection dirB)
    {
        return (dirA == DoorDirection.North && dirB == DoorDirection.South) ||
               (dirA == DoorDirection.South && dirB == DoorDirection.North) ||
               (dirA == DoorDirection.East && dirB == DoorDirection.West) ||
               (dirA == DoorDirection.West && dirB == DoorDirection.East);
    }

    void SpawnCorridor(Vector2 fromPos, Vector2 toPos)
    {
        if (corridorPrefab == null)
        {
            Debug.LogWarning("No corridor prefab assigned!");
            return;
        }

        // Snap endpoints to integer tile centers to avoid drift
        Vector2 from = new Vector2(Mathf.Round(fromPos.x), Mathf.Round(fromPos.y));
        Vector2 to = new Vector2(Mathf.Round(toPos.x), Mathf.Round(toPos.y));
        Vector2 delta = to - from;

        // Determine if corridor is horizontal or vertical (prefer largest axis)
        bool isHorizontal = Mathf.Abs(delta.x) >= Mathf.Abs(delta.y);

        // Corridor step (tile size). If your corridor prefab has a RoomData specifying roomSize, you can use that.
        int step = 1;
        RoomData corridorData = corridorPrefab.GetComponent<RoomData>();
        if (corridorData != null)
        {
            step = isHorizontal ? Mathf.Max(1, corridorData.roomSize.x) : Mathf.Max(1, corridorData.roomSize.y);
        }

        int numPieces = isHorizontal ? Mathf.Abs(Mathf.RoundToInt(delta.x)) / step : Mathf.Abs(Mathf.RoundToInt(delta.y)) / step;
        if (numPieces == 0) numPieces = 1;

        Vector2 current = from;
        Vector2 stepDir = isHorizontal ? new Vector2(Mathf.Sign(delta.x), 0) : new Vector2(0, Mathf.Sign(delta.y));

        Quaternion rotation = Quaternion.identity;
        if (isHorizontal)
            rotation = delta.x >= 0 ? Quaternion.Euler(0, 0, 0) : Quaternion.Euler(0, 0, 180);
        else
            rotation = delta.y >= 0 ? Quaternion.Euler(0, 0, 90) : Quaternion.Euler(0, 0, -90);

        for (int i = 0; i < numPieces; i++)
        {
            Vector3 worldPos = new Vector3(Mathf.Round(current.x), Mathf.Round(current.y), -1);
            GameObject corridorPiece = Instantiate(corridorPrefab, worldPos, rotation, roomParent);
            corridorPiece.name = $"Corridor_{i}_{from}to{to}";
            current += stepDir * step;
        }

        Debug.Log($"Spawned {numPieces} corridor pieces from {from} to {to}");
    }

    // -------------------------
    // Overlap helpers
    // -------------------------

    RectInt GetRoomRectAt(Vector2Int worldPos, Vector2Int roomSize)
    {
        return new RectInt(worldPos.x, worldPos.y, roomSize.x, roomSize.y);
    }

    bool RectIntersectsPlaced(RectInt candidate)
    {
        foreach (var kv in runtimeRooms)
        {
            RectInt r = kv.Value.bounds;
            if (candidate.Overlaps(r))
                return true;
        }
        return false;
    }

    // Try to find a non-overlapping position by nudging in the given direction (roomSize steps).
    Vector2Int FindNonOverlappingPosition(Vector2Int startPos, Vector2Int roomSize, Vector2Int direction, int maxAttempts = 8)
    {
        Vector2Int candidate = startPos;

        // If no direction (0,0) we'll try a small spiral around startPos
        if (direction == Vector2Int.zero)
        {
            for (int radius = 0; radius <= maxAttempts; radius++)
            {
                for (int dx = -radius; dx <= radius; dx++)
                {
                    for (int dy = -radius; dy <= radius; dy++)
                    {
                        Vector2Int probe = startPos + new Vector2Int(dx * roomSize.x, dy * roomSize.y);
                        if (!RectIntersectsPlaced(GetRoomRectAt(probe, roomSize)))
                            return probe;
                    }
                }
            }
            return startPos; // fallback
        }

        for (int i = 0; i < maxAttempts; i++)
        {
            RectInt candRect = GetRoomRectAt(candidate, roomSize);
            if (!RectIntersectsPlaced(candRect))
                return candidate;

            // Nudge outward by the room size (avoids small sliding intersections)
            candidate += new Vector2Int(direction.x * Mathf.Max(1, roomSize.x), direction.y * Mathf.Max(1, roomSize.y));
        }

        return startPos; // fallback if we couldn't find a free spot
    }
}
