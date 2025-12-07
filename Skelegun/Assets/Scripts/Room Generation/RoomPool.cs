using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "RoomGen/RoomPool")]
public class RoomPool : ScriptableObject
{
    public int roomsPerLevel = 10;
    public int seed = 0; // 0 = random by time
    public Vector2 gridCellSize = new Vector2(12, 8); // size of a single grid cell in world units (tune)
    public GameObject corridorPrefab; // simple corridor/connector prefab
    public List<RoomManager> roomPrefabs; // all prefabs including special ones

    // convenience helpers:
    public RoomManager GetRandomFromPool(System.Random rnd, System.Predicate<RoomManager> predicate = null)
    {
        var list = roomPrefabs;
        int total = 0;
        for (int i = 0; i < list.Count; i++)
        {
            if (predicate == null || predicate(list[i])) total += Mathf.Max(1, list[i].weight);
        }
        if (total <= 0) return null;
        int pick = rnd.Next(total);
        for (int i = 0; i < list.Count; i++)
        {
            var d = list[i];
            if (predicate != null && !predicate(d)) continue;
            int w = Mathf.Max(1, d.weight);
            if (pick < w) return d;
            pick -= w;
        }
        return null;
    }
}
