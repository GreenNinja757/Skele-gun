using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Tilemaps;
using System;

public class RoomInstance
{
    // References
    public RoomData data { get; private set; }
    public GameObject instanceObject { get; private set; }

    // Spatial data
    public Vector2 editorPosition { get; private set; }
    public RectInt bounds { get; private set; }

    // Door data
    public List<DoorLocation> doors { get; private set; }
    public Dictionary<DoorLocation, RoomInstance> connections { get; private set; }

    public RoomInstance(RoomData sourceData)
    {
        data = sourceData;
        doors = new List<DoorLocation>();
        connections = new Dictionary<DoorLocation, RoomInstance>();
    }

    public void Initialize(GameObject instance, Vector3 worldOffset, Vector2 editorPos)
    {
        instanceObject = instance;
        editorPosition = editorPos;

        BuildDoorWorldPositions();
    }



    public void BuildDoorWorldPositions()
    {
        doors.Clear();

        Tilemap tm = instanceObject.GetComponentInChildren<Tilemap>();
        if (tm == null) return;

        Vector3 tilemapOffset = tm.cellBounds.min;

        foreach (Door d in data.doorways)
        {
            Vector2 trueCell = tilemapOffset + new Vector3(d.localTile.x, d.localTile.y);
            Vector2 world = trueCell + (Vector2)(tm.cellSize / 2f);
            doors.Add(new DoorLocation(world, d.direction, d));
        }
    }

    public DoorLocation GetBestDoorTowards(RoomInstance otherRoom)
    {
        if (doors.Count == 0)
            return null;

        DoorLocation bestDoor = null;
        float closestDistance = float.MaxValue;

        Vector2 otherCenter = otherRoom.bounds.center;

        foreach (DoorLocation door in doors)
        {
            // Skip if already connected
            if (connections.ContainsKey(door))
                continue;

            // Calculate distance to other room's center
            float distance = Vector2.Distance(door.worldPosition, otherCenter);

            // Prefer doors facing the other room
            if (IsDoorFacingRoom(door, otherRoom))
            {
                distance *= 0.5f; // Bonus for correct facing
            }

            if (distance < closestDistance)
            {
                closestDistance = distance;
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


    public void ComputeWorldBounds()
    {
        Tilemap[] tilemaps = instanceObject.GetComponentsInChildren<Tilemap>();

        if (tilemaps.Length == 0)
        {
            Debug.LogWarning($"Room {instanceObject.name} has no Tilemaps!");
            bounds = new RectInt(0, 0, 1, 1);
            return;
        }

        BoundsInt combined = tilemaps[0].cellBounds;

        foreach (var tm in tilemaps)
        {
            combined.xMin = Math.Min(combined.xMin, tm.cellBounds.xMin);
            combined.yMin = Math.Min(combined.yMin, tm.cellBounds.yMin);
            combined.xMax = Math.Max(combined.xMax, tm.cellBounds.xMax);
            combined.yMax = Math.Max(combined.yMax, tm.cellBounds.yMax);
        }

        // Convert to world coords
        Vector3 worldMin = instanceObject.transform.TransformPoint(combined.min);
        Vector3 worldMax = instanceObject.transform.TransformPoint(combined.max);

        bounds = new RectInt(
            Mathf.RoundToInt(worldMin.x),
            Mathf.RoundToInt(worldMin.y),
            Mathf.RoundToInt(worldMax.x - worldMin.x),
            Mathf.RoundToInt(worldMax.y - worldMin.y)
        );
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