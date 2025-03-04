using UnityEngine;

public class Door : MonoBehaviour
{
    public SpriteRenderer sr;
    public Sprite openDoor;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            sr.sprite = openDoor;
        }
    }
}
