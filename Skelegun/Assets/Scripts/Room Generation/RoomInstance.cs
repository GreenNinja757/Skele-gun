using UnityEngine;

public class RoomInstance
{
    public GameObject go;
    public RoomManager man;
    public Vector2Int gridPos; // bottom-left cell position on the grid
    public RectInt occupied; // grid cells occupied

    public RoomInstance(GameObject go, RoomManager man, Vector2Int gridPos, RectInt occupied)
    {
        this.go = go;
        this.man = man;
        this.gridPos = gridPos;
        this.occupied = occupied;
    }

    public Vector3 GetWorldExitPosition(RoomDirection dir, Vector2 gridCellSize)
    {
        var anchor = man.GetExit(dir);
        if (anchor == null) return Vector3.zero;
        return anchor.position;
    }
}