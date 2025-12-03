using UnityEngine;

public class DoorHelper
{
    public static bool AreOpposite(DoorDirection a, DoorDirection b)
    {
        return
            (a == DoorDirection.North && b == DoorDirection.South) ||
            (a == DoorDirection.South && b == DoorDirection.North) ||
            (a == DoorDirection.East && b == DoorDirection.West) ||
            (a == DoorDirection.West && b == DoorDirection.East);
    }

    /// Given a parent door, find where the child's bottom-left tile should be 

    public static Vector2Int ComputeChildBottomLeft(
        Vector2Int parentDoorWorld,
        DoorDirection parentDir,
        Vector2Int childDoorLocal)
    {
        Vector2Int offset = parentDir switch
        {
            DoorDirection.North => new Vector2Int(0, 1),
            DoorDirection.South => new Vector2Int(0, -1),
            DoorDirection.East => new Vector2Int(1, 0),
            DoorDirection.West => new Vector2Int(-1, 0),
            _ => Vector2Int.zero
        };

        return parentDoorWorld + offset - childDoorLocal;
    }
}
