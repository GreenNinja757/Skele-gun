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
        Vector3 center = transform.position;
        Vector3 directionVector = GetDirectionVector() * 0.5f;
        Gizmos.DrawLine(center, center + directionVector);
        Gizmos.DrawSphere(center + directionVector, 0.1f);
    }

    Vector3 GetDirectionVector()
    {
        switch (direction)
        {
            case DoorDirection.North: return Vector3.up;
            case DoorDirection.South: return Vector3.down;
            case DoorDirection.East: return Vector3.right;
            case DoorDirection.West: return Vector3.left;
            default: return Vector3.zero;
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





















  



