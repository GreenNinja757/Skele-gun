using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class PlayerInventory : MonoBehaviour
{
    public PlayerController player;
    public PlayerHUD hud;

    public List<Item> itemInventory; 
    public List<Weapon> weaponInventory;

    public Weapon equippedWeapon;
    public int equipSlot;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Item")
        {
            itemInventory.Add(collision.gameObject.GetComponent<Item>());
            collision.gameObject.SetActive(false);
        } 
        else if (collision.tag == "Weapon")
        {
            if (equippedWeapon != null)
            {
                equippedWeapon.weaponSprite.enabled = false;
                if (equippedWeapon.GetComponent<Light2DBase>() != null)
                {
                    equippedWeapon.GetComponent<Light2DBase>().enabled = false;
                }
            }
            var weapon = collision.gameObject.GetComponent<Weapon>();
            weapon.transform.SetPositionAndRotation(player.weaponSpawnPoint.position, player.weaponRotationPoint.rotation);
            weapon.transform.parent = player.weaponSpawnPoint;
            weaponInventory.Add(weapon);
            equippedWeapon = weaponInventory[weaponInventory.Count - 1];
            equippedWeapon.gameObject.GetComponent<BoxCollider2D>().enabled = false;
            equipSlot++;
        }

        hud.UpdateHUD();
    }
    
    public void NextWeapon()
    {
        if (equippedWeapon != null)
        {
            equippedWeapon.weaponSprite.enabled = false;
            if (equippedWeapon.GetComponent<Light2DBase>() != null)
            {
                equippedWeapon.GetComponent<Light2DBase>().enabled = false;
            }
            if (equipSlot + 1 <= weaponInventory.Count - 1)
            {
                equipSlot += 1;
            }
            else
            {
                equipSlot = 0;
            }
            equippedWeapon = weaponInventory[equipSlot];
            equippedWeapon.weaponSprite.enabled = true;
            if (equippedWeapon.GetComponent<Light2DBase>() != null)
            {
                equippedWeapon.GetComponent<Light2DBase>().enabled = true;
            }
        }

        hud.UpdateHUD();
    }

    public void PreviousWeapon()
    {
        if (equippedWeapon != null)
        {
            equippedWeapon.weaponSprite.enabled = false;
            if (equippedWeapon.GetComponent<Light2DBase>() != null)
            {
                equippedWeapon.GetComponent<Light2DBase>().enabled = false;
            }
            if (equipSlot - 1 >= 0)
            {
                equipSlot -= 1;
            }
            else
            {
                equipSlot = weaponInventory.Count - 1;
            }
            equippedWeapon = weaponInventory[equipSlot];
            equippedWeapon.weaponSprite.enabled = true;
            if (equippedWeapon.GetComponent<Light2DBase>() != null)
            {
                equippedWeapon.GetComponent<Light2DBase>().enabled = true;
            }
        }

        hud.UpdateHUD();
    }
}
