using System.Collections;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public ArrayList inventory;

    void Start()
    {
        inventory = new ArrayList();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        collision.gameObject.SetActive(false);
    }

    private void addItem()
    {

    }
}
