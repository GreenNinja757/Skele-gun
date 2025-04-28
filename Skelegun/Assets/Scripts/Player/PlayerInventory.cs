using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public PlayerHUD playerHUD;
    public PlayerController player;

    public List<Item> itemInventory; 
    public List<Weapon> weaponInventory;

    public Weapon equippedWeapon;
    public int equipSlot;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Item"))
        {
            itemInventory.Add(collision.gameObject.GetComponent<Item>());
            collision.gameObject.SetActive(false);
        }
        else if (collision.CompareTag("Weapon"))
        {
            if (equippedWeapon != null)
            {
                equippedWeapon.gameObject.SetActive(false);
            }
            equippedWeapon = Instantiate(collision.gameObject.GetComponent<Weapon>(), player.weaponSpawnPoint.position, player.weaponRotationPoint.rotation, player.weaponRotationPoint);
            weaponInventory.Add(equippedWeapon);
            equippedWeapon.isPickup = false;
            collision.gameObject.SetActive(false);
        }
    }

    public void NextWeapon()
    {
        if (weaponInventory.Count != 0)
        {
            if (equipSlot + 1 <= weaponInventory.Count - 1)
            {
                equipSlot += 1;
            }
            else
            {
                equipSlot = 0;
            }
            equippedWeapon.gameObject.SetActive(false);
            equippedWeapon = weaponInventory[equipSlot];
            equippedWeapon.gameObject.SetActive(true);
            playerHUD.UpdateWeapon();
        }
    }

    public void PreviousWeapon()
    {
        if (weaponInventory.Count != 0)
        {
            if (equipSlot - 1 >= 0)
            {
                equipSlot -= 1;
            }
            else
            {
                equipSlot = weaponInventory.Count - 1;
            }
            equippedWeapon.gameObject.SetActive(false);
            equippedWeapon = weaponInventory[equipSlot];
            equippedWeapon.gameObject.SetActive(true);
            playerHUD.UpdateWeapon();
        }
    }
}
