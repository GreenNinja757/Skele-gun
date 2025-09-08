using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class PlayerInventory : MonoBehaviour
{
    public PlayerController player;
    public PlayerStats stats;
    public PlayerHUD hud;

    public List<Item> itemInventory; 
    public List<Weapon> weaponInventory;

    public Weapon equippedWeapon;
    public int equipSlot;

    public AudioClip pickupSFX;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Item"))
        {
            var item = collision.GetComponent<Item>();
            item.GetComponent<IPlayerUpgrade>()?.ApplyPlayerUpgrade();
            item.GetComponent<SpriteRenderer>().sortingOrder = 0;
            item.transform.SetPositionAndRotation(player.transform.position, player.transform.rotation);
            item.transform.localScale = player.transform.localScale;
            item.transform.parent = player.transform;
            itemInventory.Add(item);
            item.gameObject.GetComponent<BoxCollider2D>().enabled = false;
            item.GetComponent<SpriteRenderer>().enabled = false;
            StopAllCoroutines();
            StartCoroutine(hud.DisplayPickupMessage(item.itemSprite.sprite, item.nameText, item.flavorText));
        } 
        else if (collision.CompareTag("Weapon"))
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
            weapon.GetComponent<SpriteRenderer>().sortingOrder = 2;
            weapon.transform.SetPositionAndRotation(player.weaponSpawnPoint.position, player.weaponRotationPoint.rotation);
            weapon.transform.localScale = player.weaponRotationPoint.localScale;
            weapon.transform.parent = player.weaponSpawnPoint;
            weaponInventory.Add(weapon);
            equippedWeapon = weaponInventory[weaponInventory.Count - 1];
            equippedWeapon.gameObject.GetComponent<BoxCollider2D>().enabled = false;
            equippedWeapon.SetStats();
            equipSlot++;

            StopAllCoroutines();
            StartCoroutine(hud.DisplayPickupMessage(weapon.weaponSprite.sprite, weapon.nameText, weapon.flavorText));
        }
        else if (collision.CompareTag("Pickup"))
        {
            collision.GetComponent<Pickup>().OnPickup();
            Destroy(collision.gameObject);
        }
        hud.UpdateHUD();

        var AudioManager = FindAnyObjectByType<AudioManager>();
        AudioManager.PlaySFX(pickupSFX);
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
            equippedWeapon.SetStats();
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
            equippedWeapon.SetStats();
        }
        hud.UpdateHUD();
    }
}
