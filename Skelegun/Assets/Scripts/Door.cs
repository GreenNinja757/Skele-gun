using UnityEngine;

public class Door : MonoBehaviour
{
    [Header("Visual")]
    public SpriteRenderer sr;
    public Sprite openDoor;

    [Header("Settings")]
    public DoorDirection direction;
    public Vector2 localTile;  // tile coords relative to bottom-left of room

    /* Logic for later
    public bool startsLocked = false;
    private bool isOpen = false;
    private bool isLocked = false;
    */ 

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            sr.sprite = openDoor;
        }
    }

    void OnDrawGizmos()
    {
        // Draw direction indicator
        Gizmos.color = Color.yellow;
        Vector2 center = transform.position;
        Vector2 directionVector = GetDirectionVector() * 0.5f;
        Gizmos.DrawLine(center, center + directionVector);
        Gizmos.DrawSphere(center + directionVector, 0.1f);
    }

    Vector2 GetDirectionVector()
    {
        switch (direction)
        {
            case DoorDirection.North: return Vector2.up;
            case DoorDirection.South: return Vector2.down;
            case DoorDirection.East: return Vector2.right;
            case DoorDirection.West: return Vector2.left;
            default: return Vector2.zero;
        }
    }

}

public enum DoorDirection
{
    North,
    South,
    East,
    West
}





















  



