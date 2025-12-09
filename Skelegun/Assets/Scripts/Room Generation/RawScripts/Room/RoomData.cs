using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(SpriteRenderer))]
public class RoomData : MonoBehaviour
{
    
    public Tilemap layout;

    public RoomType roomType = RoomType.Combat;

    public List<Door> doorways = new List<Door>();
    public List<Transform> spawnPoints = new List<Transform>();
    public GameObject wallPrefab;

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

 
    public Vector2 GetTilemapOriginOffset()
    {
        if (layout == null)
            return Vector2.zero;

        BoundsInt cb = layout.cellBounds;
        return new Vector2(-cb.xMin, -cb.yMin);
    }

    public void ApplyTilemapOriginOffsetToInstance(GameObject instance)
    {
        if (layout == null || instance == null) return;
        Tilemap instTilemap = instance.GetComponentInChildren<Tilemap>();
        if (instTilemap == null)
        {
            Debug.LogWarning($"RoomData '{name}': instantiated object has no Tilemap child to normalize.");
            return;
        }

        Vector2 offset = GetTilemapOriginOffset();
        Vector3 lp = instTilemap.transform.localPosition;
        instTilemap.transform.localPosition = new Vector3(offset.x, offset.y, lp.z);
    }

    public void ApplyDoorsForInstance(RoomInstance instance)
    {
        foreach (Door door in doorways)
        {
            if (door == null) continue;

            bool isActive = instance.activeDoorDirections.Contains(door.direction);

            if (door.sr != null)door.sr.enabled = isActive;

            if (!isActive && wallPrefab != null)
            {
                // check if wall already exists
                if (instance.instanceObject.transform.Find($"Wall_{door.direction}") == null)
                {
                    Vector3 localDoorPos = door.transform.localPosition;
                    Vector3 wallLocalPos = new Vector3(Mathf.Floor(localDoorPos.x), Mathf.Floor(localDoorPos.y), localDoorPos.z);
                    GameObject wall = Instantiate(wallPrefab, instance.instanceObject.transform);
                    wall.transform.localPosition = wallLocalPos;
                    wall.name = $"Wall_{door.direction}";
                }
            }

        }
    }


    void OnDrawGizmos()
    {
        if (layout == null) return;

        Vector2Int size = roomSize;

        // Draw room bounds from GameObject origin (0,0)
        Vector3 center = transform.position + new Vector3(size.x / 2f, size.y / 2f, 0);
        Gizmos.color = new Color(0, 1, 0, 0.3f);
        Gizmos.DrawWireCube(center, new Vector3(size.x, size.y, 0));

        // Draw GameObject origin (what we use for placement)
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(transform.position, 0.15f);

        // Draw tilemap's actual cell origin (for reference)
        BoundsInt cellBounds = layout.cellBounds;
        Gizmos.color = Color.red;
        Vector3 tilemapOrigin = transform.position + new Vector3(cellBounds.xMin, cellBounds.yMin, 0);
        Gizmos.DrawSphere(tilemapOrigin, 0.12f);

        // Draw doorways relative to GameObject origin
        Gizmos.color = Color.cyan;
        foreach (Door door in doorways)
        {
            if (door == null) continue;
            Vector3 doorWorldPos = transform.position + new Vector3(door.localTile.x, door.localTile.y, 0);
            Gizmos.DrawSphere(doorWorldPos, 0.2f);
        }

        // tilemap position drawing (debug)
        var tp = layout.transform.position;
        var tBounds = layout.cellBounds;
        var c0 = new Vector3(tBounds.min.x, tBounds.min.y) + tp;
        var c1 = new Vector3(tBounds.min.x, tBounds.max.y) + tp;
        var c2 = new Vector3(tBounds.max.x, tBounds.max.y) + tp;
        var c3 = new Vector3(tBounds.max.x, tBounds.min.y) + tp;
        Debug.DrawLine(c0, c1, Color.red);
        Debug.DrawLine(c1, c2, Color.red);
        Debug.DrawLine(c2, c3, Color.red);
        Debug.DrawLine(c3, c0, Color.red);

        Debug.DrawLine(new Vector3(tp.x, tBounds.min.y + tp.y), new Vector3(tp.x, tBounds.max.y + tp.y), Color.yellow);
        Debug.DrawLine(new Vector3(tBounds.min.x + tp.x, tp.y), new Vector3(tBounds.max.x + tp.x, tp.y), Color.yellow);
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
