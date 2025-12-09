using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Tilemaps;


public class RoomInstance
{

    public RoomData data { get; private set; }
    public GameObject instanceObject { get; private set; }

    public Vector2 editorPosition { get; private set; }
    public RectInt bounds { get; private set; }


    public List<DoorLocation> doors { get; private set; }
    public Dictionary<DoorLocation, RoomInstance> connections { get; private set; }

    public List<Transform> spawnPoints { get; private set; }

    // which doors are active
    public List<DoorDirection> activeDoorDirections { get; set; } = new List<DoorDirection>();


    public RoomInstance(RoomData sourceData)
    {
        data = sourceData;
        doors = new List<DoorLocation>();
        connections = new Dictionary<DoorLocation, RoomInstance>();
        activeDoorDirections = new List<DoorDirection>();
        spawnPoints = new List<Transform>();
    }


    public void Initialize(GameObject instance, Vector3 worldOffset, Vector2 editorPos)
    {
        instanceObject = instance;
        editorPosition = editorPos;

        data.ApplyTilemapOriginOffsetToInstance(instanceObject);
        BuildDoorWorldPositions();

        // cache spawn points
        spawnPoints.Clear();
        foreach (var sp in data.spawnPoints)
        {
            if (sp == null) continue;
            Transform instSP = instanceObject.transform.Find(sp.name);
            if (instSP != null)
                spawnPoints.Add(instSP);
            else
            {
                // fallback
                GameObject marker = new GameObject(sp.name + "_inst");
                marker.transform.SetParent(instanceObject.transform, false);
                marker.transform.localPosition = sp.localPosition;
                spawnPoints.Add(marker.transform);
            }
        }

        ComputeWorldBounds();
    }

    public void ComputeWorldBounds()
    {
        if (data.layout == null || instanceObject == null)
        {
            bounds = new RectInt(0, 0, 1, 1);
            return;
        }

        Tilemap instTilemap = instanceObject.GetComponentInChildren<Tilemap>();
        if (instTilemap == null)
        {
            Debug.LogWarning($"Room {instanceObject.name} has no Tilemap child!");
            bounds = new RectInt(0, 0, 1, 1);
            return;
        }

        BoundsInt cb = instTilemap.cellBounds;

        Vector3 worldMinF = instTilemap.CellToWorld(cb.min);
        Vector2Int worldMin = new Vector2Int(Mathf.RoundToInt(worldMinF.x), Mathf.RoundToInt(worldMinF.y));
        Vector2Int size = new Vector2Int(cb.size.x, cb.size.y);

        bounds = new RectInt(worldMin, size);
    }

    public void BuildDoorWorldPositions()
    {
        doors.Clear();

        if (data.doorways == null || data.doorways.Count == 0)
        {
            Debug.LogWarning($"Room '{instanceObject.name}' has no doorways!");
            return;
        }

        Tilemap instTilemap = instanceObject.GetComponentInChildren<Tilemap>();
        if (instTilemap == null)
        {
            Debug.LogWarning($"RoomInstance '{instanceObject.name}': no instantiated Tilemap found.");
            return;
        }

        foreach (Door door in data.doorways)
        {
            if (door == null) continue;

            Vector3Int cellPos = new Vector3Int(Mathf.RoundToInt(door.localTile.x), Mathf.RoundToInt(door.localTile.y), 0);
            Vector3 worldPosF = instTilemap.CellToWorld(cellPos);
            Vector2 worldPos = new Vector2(worldPosF.x, worldPosF.y);

            doors.Add(new DoorLocation(worldPos, door.direction, door));
        }
    }

    public DoorLocation GetBestDoorTowards(RoomInstance otherRoom)
    {
        if (doors.Count == 0)
            return null;

        DoorLocation bestDoor = null;
        float bestScore = float.MaxValue;

        Vector2 otherCenter = otherRoom.bounds.center;

        foreach (DoorLocation door in doors)
        {
            if (connections.ContainsKey(door))
                continue;

            float distance = Vector2.Distance(door.worldPosition, otherCenter);

            float score = distance;

            if (IsDoorFacingRoom(door, otherRoom))
            {
                score *= 0.3f; 
            }
            else
            {
                score *= 2.0f; 
            }

            if (score < bestScore)
            {
                bestScore = score;
                bestDoor = door;
            }
        }

        return bestDoor;
    }

    bool IsDoorFacingRoom(DoorLocation door, RoomInstance otherRoom)
    {
        Vector2 directionToOther = (Vector2)otherRoom.bounds.center - door.worldPosition;

        switch (door.direction)
        {
            case DoorDirection.North:
                return directionToOther.y > 0;
            case DoorDirection.South:
                return directionToOther.y < 0;
            case DoorDirection.East:
                return directionToOther.x > 0;
            case DoorDirection.West:
                return directionToOther.x < 0;
        }

        return false;
    }

    public void AddConnection(DoorLocation door, RoomInstance connectedRoom)
    {
        if (!connections.ContainsKey(door))
        {
            connections.Add(door, connectedRoom);
        }
    }

    public bool IsConnectedTo(RoomInstance otherRoom)
    {
        return connections.ContainsValue(otherRoom);
    }

    public List<RoomInstance> GetConnectedRooms()
    {
        return connections.Values.ToList();
    }


}

public class DoorLocation
{
    public Vector2 worldPosition { get; private set; }
    public DoorDirection direction { get; private set; }
    public Door doorComponent { get; private set; }

    public DoorLocation(Vector2 worldPos, DoorDirection dir, Door component)
    {
        worldPosition = worldPos;
        direction = dir;
        doorComponent = component;
    }
}
