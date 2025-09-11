using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviour
{
    public PlayerStats stats;
    public PlayerInventory inventory;

    public Slider healthBar;
    public Slider shieldBar;

    public TextMeshProUGUI healthAndShieldValue;
    public TextMeshProUGUI moneyValue;
    public TextMeshProUGUI keysValue;
    public TextMeshProUGUI ammoValue;

    public List<Image> itemSprites;
    public List<Image> weaponSprites;
    public Image weaponSprite;

    public Canvas pickupDisplayCanvas;
    public Image pickupSprite;
    public TextMeshProUGUI pickupNameText;
    public TextMeshProUGUI pickupFlavorText;

    void Start()
    {
        UpdateHUD();
    }

    public IEnumerator DisplayPickupMessage(Sprite sprite, string nameText, string flavorText)
    {
        pickupDisplayCanvas.enabled = true;
        pickupSprite.sprite = sprite;
        pickupNameText.text = nameText;
        pickupFlavorText.text = flavorText;
        pickupSprite.preserveAspect = true;
        yield return new WaitForSeconds(3.5f);
        pickupDisplayCanvas.enabled = false;
    }

    public void UpdateHUD()
    {
        healthBar.maxValue = stats.maxHealth;
        healthBar.value = stats.currentHealth;
        shieldBar.maxValue = stats.maxHealth;
        shieldBar.value = stats.shield;

        healthAndShieldValue.text = stats.currentHealth + stats.shield + "/" + stats.maxHealth;
        moneyValue.text = "" + stats.money;
        keysValue.text = "" + stats.keys;

        //for (int i = 0; i < itemSprites.Count; i++)
        //{
        //    itemSprites[i].sprite = inventory.itemInventory[i].itemSprite.sprite;
        //}

        //for (int i = 0; i < weaponSprites.Count; i++)
        //{
        //    weaponSprites[i].sprite = inventory.weaponInventory[i].weaponSprite.sprite;
        //}

        if (inventory.equippedWeapon != null )
        {
            weaponSprite.enabled = true;
            weaponSprite.sprite = inventory.equippedWeapon.weaponSprite.sprite;
            ammoValue.text = inventory.equippedWeapon.currentAmmo + "/" + inventory.equippedWeapon.maxAmmo;
        } 
        else
        {
            ammoValue.text = "\u0020"; //This is a blank character, so the ammo count won't show if they don't have a gun
        }
    }
}
