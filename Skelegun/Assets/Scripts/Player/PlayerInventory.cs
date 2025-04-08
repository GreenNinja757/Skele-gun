using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public List<Item> itemInventory; 
    public List<Weapon> weaponInventory;

    public Weapon equippedWeapon;
    public int equipSlot;

    void Start()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Item")
        {
            collision.gameObject.SetActive(false);
        }
    }

    public void previousWeapon()
    {
        if (equipSlot - 1 >= 0)
        {
            equipSlot -= 1;
        } 
        else
        {
            equipSlot = weaponInventory.Count;
        }
        equippedWeapon = weaponInventory[equipSlot];
    }

    public void nextWeapon()
    {
        if (equipSlot + 1 <= weaponInventory.Count)
        {
            equipSlot += 1;
        }
        else
        {
            equipSlot = 0;
        }
        equippedWeapon = weaponInventory[equipSlot];
    }
}
