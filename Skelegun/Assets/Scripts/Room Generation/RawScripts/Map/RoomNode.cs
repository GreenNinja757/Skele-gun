using UnityEngine;

public class RoomNode
{
    public Vector2Int gridPos;
    public RoomType type;
    public bool isRevealed = false;

    public RoomNode(Vector2Int pos, RoomType type)
    {
        gridPos = pos;
        this.type = type;
    }
}


