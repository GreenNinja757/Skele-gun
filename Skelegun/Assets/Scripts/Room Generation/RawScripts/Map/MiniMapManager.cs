using System.Collections.Generic;
using UnityEngine;

public class MiniMapManager : MonoBehaviour
{
    public static MiniMapManager instance;
    private Dictionary<Vector2Int, RoomNode> roomNodes = new Dictionary<Vector2Int, RoomNode>();

    public MapUI minimapUI;

    void Awake()
    {
        instance = this;
    }


    // Call room gen when created
    public void RegisterRoom(Vector2Int gridPos, RoomType type)
    {
        if (!roomNodes.ContainsKey(gridPos))
        {
            roomNodes.Add(gridPos, new RoomNode(gridPos, type));
            minimapUI.CreateRoomIcon(gridPos, type);
        }
    }


    // Player enters 
    public void RevealRoom(Vector2Int gridPos)
    {
        if (!roomNodes.ContainsKey(gridPos)) return;

        // Reveal current room
        roomNodes[gridPos].isRevealed = true;
        minimapUI.SetRoomVisible(gridPos, true);

        // Reveal connection to adjacent rooms - but not actual room
        Vector2Int[] directions = {
            Vector2Int.up, Vector2Int.down,
            Vector2Int.left, Vector2Int.right
        };

        foreach (var dir in directions)
        {
            Vector2Int adjacentPos = gridPos + dir;

            if (roomNodes.ContainsKey(adjacentPos))
            {
                minimapUI.ShowConnection(gridPos, adjacentPos);
            }
        }
    }
}

