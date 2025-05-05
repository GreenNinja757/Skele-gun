using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GetAGun : MonoBehaviour
{
    public Transform playerHead;
    public GameObject goGetaGun;
    public PlayerInventory inventory;
    public Collider2D theWall;


    void Start()
    {
        playerHead = FindAnyObjectByType<PlayerController>().transform;
    }

    void OnTriggerEnter2D(Collider2D ugh)
    {
        if (ugh.CompareTag("Player"))
        {

            if (inventory.equippedWeapon == null)
            {
                SpawnText();
            }
            else
                Destroy(theWall.gameObject);
        }
    }

    void SpawnText()
    {
        goGetaGun.SetActive(true);
    }

    void Update()
    {
        goGetaGun.transform.position = playerHead.position + new Vector3 (0,.5f, 0);

        if (goGetaGun.activeSelf == true && inventory.equippedWeapon != null)
        {
            goGetaGun.SetActive(false);
        }
    }
}