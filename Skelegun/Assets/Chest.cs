using UnityEditor;
using UnityEngine;

public class Chest : MonoBehaviour
{
    public SpriteRenderer sr;
    public Sprite closedChest;
    public Sprite openChest;
    public Transform itemSpawnPoint;
    public GameObject itemPrefab;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Opened chest!");
        if (collision.gameObject.tag == "Player")
        {
            sr.sprite = openChest;
            Instantiate(itemPrefab, itemSpawnPoint);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
