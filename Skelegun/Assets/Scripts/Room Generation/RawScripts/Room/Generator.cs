using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Generator : MonoBehaviour
{
    [Header("Level Design")]
    public List<LevelChart> levelChartOptions = new List<LevelChart>();

    [Header("Generation Settings")]
    public Transform roomParent;

    [Header("Corridor Settings")]
    public GameObject corridorPrefab;
    public float maxDirectConnectionDistance = 2f;

    // Runtime data
    private Dictionary<string, RoomInstance> runtimeRooms = new Dictionary<string, RoomInstance>();
    private HashSet<RoomData> usedRoomPrefabs = new HashSet<RoomData>();
    private LevelChart chosenChart;
    private HashSet<string> placedNodes = new HashSet<string>();
    private System.Random rng;

    void Start()
    {
        Generate();
    }

    public void Generate()
    {
        ClearLevel();

        if (!ValidateSetup())
            return;

        rng = new System.Random();

        PickLevelChart();
        PlaceAndConnectRooms();
        ApplyActiveDoorsToAllRooms();

        Debug.Log($"<color=green>Level generation complete: {runtimeRooms.Count} rooms spawned</color>");
    }

    void ClearLevel()
    {
        runtimeRooms.Clear();
        placedNodes.Clear();
        usedRoomPrefabs.Clear();

        if (roomParent != null)
        {
            for (int i = roomParent.childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(roomParent.GetChild(i).gameObject);
            }
        }
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
            roomParent = new GameObject("Generated Rooms").transform;
        }

        return true;
    }

    void PickLevelChart()
    {
        chosenChart = levelChartOptions[Random.Range(0, levelChartOptions.Count)];
        Debug.Log($"Selected chart: {chosenChart.name}");
    }

    void PlaceAndConnectRooms()
    {
        if (chosenChart.nodes.Count == 0) return;

        LevelNode startNode = chosenChart.nodes.Find(n => n.roomType == RoomType.Spawn) ?? chosenChart.nodes[0];
        PlaceRoom(startNode, Vector2Int.zero);

        Queue<LevelNode> toProcess = new Queue<LevelNode>();
        toProcess.Enqueue(startNode);

        while (toProcess.Count > 0)
        {
            LevelNode currentNode = toProcess.Dequeue();
            if (!runtimeRooms.TryGetValue(currentNode.id, out RoomInstance currentRoom)) continue;


            List<DoorLocation> shuffledDoors = currentRoom.doors
                .Where(d => !currentRoom.connections.ContainsKey(d))
                .OrderBy(x => rng.Next())
                .ToList();

            int doorIndex = 0;


            foreach (string linkedID in currentNode.connectedNodeIDs)
            {
                if (placedNodes.Contains(linkedID)) continue;

                if (doorIndex >= shuffledDoors.Count)
                {
                    Debug.LogWarning($"Not enough doors in parent {currentNode.id} for node {linkedID}");
                    break;
                }

                DoorLocation parentDoor = shuffledDoors[doorIndex++];
                LevelNode childNode = chosenChart.GetNode(linkedID);
                if (childNode == null) continue;

                RoomData childPrefab = childNode.pool?.GetRandomRoom(usedRoomPrefabs);
                if (childPrefab == null) continue;

                Vector2Int? childPos = CalculateDoorAlignedPosition(currentRoom, childPrefab, parentDoor);
                if (!childPos.HasValue)
                {
                    Debug.LogWarning($"Could not place room {childNode.displayName}");
                    continue;
                }

                RoomInstance childRoom = PlaceRoom(childNode, childPos.Value, childPrefab);

                // Connect rooms and register active doors
                ConnectTwoRooms(currentRoom, childRoom, parentDoor);

                toProcess.Enqueue(childNode);
            }
        }
    }


    RoomInstance PlaceRoom(LevelNode node, Vector2Int worldPos, RoomData prefab = null)
    {
        prefab ??= node.pool?.GetRandomRoom(usedRoomPrefabs);
        if (prefab == null)
        {
            Debug.LogError($"No prefab available for node {node.id}");
            return null;
        }

        usedRoomPrefabs.Add(prefab);
        Vector3 pos3D = new Vector3(worldPos.x, worldPos.y, 0);
        GameObject roomObj = Instantiate(prefab.gameObject, pos3D, Quaternion.identity, roomParent);
        roomObj.name = $"Room_{node.displayName}_{prefab.name}";

        RoomInstance instance = new RoomInstance(prefab);
        instance.Initialize(roomObj, pos3D, node.editorPosition);

        runtimeRooms[node.id] = instance;
        placedNodes.Add(node.id);

        return instance;
    }

    Vector2Int? CalculateDoorAlignedPosition(RoomInstance parentRoom, RoomData childPrefab, DoorLocation parentDoor)
    {
        DoorDirection childDir = DirectionUtils.Opposite(parentDoor.direction);
        Door childDoor = childPrefab.doorways.FirstOrDefault(d => d.direction == childDir);
        if (childDoor == null) return null;

        Vector2 parentWorld = parentDoor.worldPosition;
        Vector2Int offset = DirectionUtils.OffsetFor(parentDoor.direction);
        Vector2 localOffset = childPrefab.GetTilemapOriginOffset() + childDoor.localTile;

        Vector2 finalPos = parentWorld + new Vector2(offset.x, offset.y) - localOffset;
        return new Vector2Int(Mathf.RoundToInt(finalPos.x), Mathf.RoundToInt(finalPos.y));
    }

    void ConnectTwoRooms(RoomInstance parent, RoomInstance child, DoorLocation parentDoor = null)
    {
        parentDoor ??= parent.GetBestDoorTowards(child);
        DoorLocation childDoor = child.GetBestDoorTowards(parent);

        if (parentDoor == null || childDoor == null) return;

        parent.AddConnection(parentDoor, child);
        child.AddConnection(childDoor, parent);

        parent.activeDoorDirections.Add(parentDoor.direction);
        child.activeDoorDirections.Add(childDoor.direction);

        if (Vector2.Distance(parentDoor.worldPosition, childDoor.worldPosition) > maxDirectConnectionDistance)
            SpawnCorridor(parentDoor.worldPosition, childDoor.worldPosition);
    }

    void SpawnCorridor(Vector2 from, Vector2 to)
    {
        if (corridorPrefab == null) return;

        Vector2 delta = to - from;
        bool horizontal = Mathf.Abs(delta.x) >= Mathf.Abs(delta.y);
        RoomData data = corridorPrefab.GetComponent<RoomData>();
        int step = data != null ? (horizontal ? Mathf.Max(1, data.roomSize.x) : Mathf.Max(1, data.roomSize.y)) : 1;

        int count = Mathf.CeilToInt((horizontal ? Mathf.Abs(delta.x) : Mathf.Abs(delta.y)) / step);
        Vector2 stepDir = horizontal ? new Vector2(Mathf.Sign(delta.x) * step, 0) : new Vector2(0, Mathf.Sign(delta.y) * step);

        Vector2 pos = from;
        for (int i = 0; i < count; i++)
        {
            Vector3 wPos = new Vector3(Mathf.Round(pos.x), Mathf.Round(pos.y), -1);
            Instantiate(corridorPrefab, wPos, Quaternion.identity, roomParent).name = $"Corridor_{i}";
            pos += stepDir;
        }
    }

    void ApplyActiveDoorsToAllRooms()
    {
        foreach (var room in runtimeRooms.Values)
        {
            room.data.ApplyDoorsForInstance(room);
        }
    }






}
public static class DirectionUtils
{
    public static Vector2Int OffsetFor(DoorDirection dir) => dir switch
    {
        DoorDirection.North => new Vector2Int(0, 1),
        DoorDirection.South => new Vector2Int(0, -1),
        DoorDirection.East => new Vector2Int(1, 0),
        DoorDirection.West => new Vector2Int(-1, 0),
        _ => Vector2Int.zero
    };

    public static DoorDirection Opposite(DoorDirection dir) => dir switch
    {
        DoorDirection.North => DoorDirection.South,
        DoorDirection.South => DoorDirection.North,
        DoorDirection.East => DoorDirection.West,
        DoorDirection.West => DoorDirection.East,
        _ => DoorDirection.North
    };

    public static DoorDirection DirectionFromDelta(Vector2 delta)
    {
        return Mathf.Abs(delta.x) > Mathf.Abs(delta.y)
            ? (delta.x > 0 ? DoorDirection.East : DoorDirection.West)
            : (delta.y > 0 ? DoorDirection.North : DoorDirection.South);
    }
}


