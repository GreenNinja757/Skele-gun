using UnityEngine;

public class Chest : MonoBehaviour
{
    public SpriteRenderer sr;
    public Sprite closedChest;
    public Sprite openChest;
    public Transform itemSpawnPoint;
    public GameObject itemPrefab;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            sr.sprite = openChest;
            Instantiate(itemPrefab, itemSpawnPoint);
        }
    }

    void spawnItem()
    {
        //pick rarity
        
        //pick item within selected rarity
    }
}
