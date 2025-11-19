using UnityEngine;

public class RoomManager : MonoBehaviour
{
   

    public Vector2Int gridPosition; 
    public bool discovered;
    public GameObject prefab;

    [Header("Exits (set in prefab)")]
    public Transform exitNorth;
    public Transform exitSouth;
    public Transform exitEast;
    public Transform exitWest;

    public Vector2Int gridSize; // width x height on grid

    public int minDistanceFromSpawn = 0;

    [Header("Room Settings")]
    public RoomType roomType;
    public int preferredMinDistanceFromSpawn = 0;
    public int weight = 1; // selection weight in pool
    public int maxSpawnAttempts = 8; // attempts to place this type before skipping


    [Header("Optional: Make this true to force how many exits")]
    public bool forceExitCount = false;
    [Range(0, 4)] public int forcedExitCount = 2;

  

    void Awake()
    {
         //Auto-measure room size if it has a collider

        BoxCollider2D coleTrain = GetComponent<BoxCollider2D>();
        if (coleTrain != null)
            gridSize = Vector2Int.RoundToInt(coleTrain.size);
        
    }


    // helper to get local anchor for direction
    public Transform GetExit(RoomDirection dir)
    {
        switch (dir)
        {
            case RoomDirection.North: return exitNorth;
            case RoomDirection.South: return exitSouth;
            case RoomDirection.East: return exitEast;
            case RoomDirection.West: return exitWest;
            default: return null;
        }
    }
}

public enum RoomType
{
    Spawn,
    Combat,
    Shop,
    Reward,
    Boss
}
public enum RoomDirection 
{
    North, 
    South, 
    East, 
    West 
}






/* Use this someplace else

MapManager.instance.DiscoverRoom(currentRoom.gridPosition, currentRoom.size);
MapManager.instance.UpdatePlayerRoom(currentRoom.gridPosition);

*/


























