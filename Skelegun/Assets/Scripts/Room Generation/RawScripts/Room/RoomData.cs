using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(SpriteRenderer))]
public class RoomData : MonoBehaviour
{
    [Header("Room Layout")]
    [Tooltip("Tilemap representing the WALKABLE FLOOR AREA (not walls). This defines room bounds.")]
    public Tilemap layout;

    [Header("Room Type")]
    public RoomType roomType = RoomType.Combat;

    [Header("Doorways")]
    [Tooltip("Doors on room perimeter. Use RefreshDoors to auto-populate from children.")]
    public List<Door> doorways = new List<Door>();


    public Vector2Int roomSize
    {
        get
        {
            if (layout == null)
            {
                Debug.LogWarning($"RoomData '{gameObject.name}' has no layout tilemap!");
                return Vector2Int.zero;
            }

            BoundsInt cellBounds = layout.cellBounds;
            return new Vector2Int(cellBounds.size.x, cellBounds.size.y);
        }
    }


    [ContextMenu("Refresh Doors from Children")]
    public void RefreshDoors()
    {
        doorways.Clear();
        doorways.AddRange(GetComponentsInChildren<Door>(true));
        Debug.Log($"Found {doorways.Count} doors in '{gameObject.name}'");
    }

    

    void OnDrawGizmos()
    {
        if (layout == null) return;

        /*Vector2Int size = roomSize;

        // Draw room bounds from GameObject origin (0,0)
        Vector3 center = transform.position + new Vector3(size.x / 2f, size.y / 2f, 0);
        Gizmos.color = new Color(0, 1, 0, 0.3f);
        Gizmos.DrawWireCube(center, new Vector3(size.x, size.y, 0));

        // Draw GameObject origin (what we use for placement)
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(transform.position, 0.3f);

        // Draw tilemap's actual cell origin (for reference)
        BoundsInt cellBounds = layout.cellBounds;
        Gizmos.color = Color.red;
        Vector3 tilemapOrigin = transform.position + new Vector3(cellBounds.xMin, cellBounds.yMin, 0);
        Gizmos.DrawSphere(tilemapOrigin, 0.2f);

        // Draw doorways relative to GameObject origin
        Gizmos.color = Color.cyan;
        foreach (Door door in doorways)
        {
            if (door == null) continue;
            Vector3 doorWorldPos = transform.position + new Vector3(door.localTile.x, door.localTile.y, 0);
            Gizmos.DrawSphere(doorWorldPos, 0.5f);
        }
        */




        // tilemap position
        var tp = layout.transform.position;

        // bounds + offset
        var tBounds = layout.cellBounds;

        // corner points
        var c0 = new Vector3(tBounds.min.x, tBounds.min.y) + tp;
        var c1 = new Vector3(tBounds.min.x, tBounds.max.y) + tp;
        var c2 = new Vector3(tBounds.max.x, tBounds.max.y) + tp;
        var c3 = new Vector3(tBounds.max.x, tBounds.min.y) + tp;





        // draw borders
        Debug.DrawLine(c0, c1, Color.red);
        Debug.DrawLine(c1, c2, Color.red);
        Debug.DrawLine(c2, c3, Color.red);
        Debug.DrawLine(c3, c0, Color.red);

        // draw origin cross
        Debug.DrawLine(new Vector3(tp.x, tBounds.min.y + tp.y), new Vector3(tp.x, tBounds.max.y + tp.y), Color.green);
        Debug.DrawLine(new Vector3(tBounds.min.x + tp.x, tp.y), new Vector3(tBounds.max.x + tp.x, tp.y), Color.green);






    }














public class DrawTilemapBorder : MonoBehaviour
{
    Tilemap tilemap;

    void OnValidate()
    {
        if (tilemap == null)
            tilemap = GetComponent<Tilemap>();
    }



    void Draw()
    {
     

    }
}




















}

public enum RoomType
{
    Spawn,
    Combat,
    Shop,
    Reward,
    Boss,
    Secret,
    Treasure
}