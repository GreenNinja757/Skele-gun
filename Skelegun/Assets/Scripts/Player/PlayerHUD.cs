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

    public TextMeshProUGUI healthValue;
    public TextMeshProUGUI moneyValue;
    public TextMeshProUGUI ammoValue;

    public List<Image> itemSprites;
    public Image weaponSprite;

    void Start()
    {
        UpdateHUD();
    }

    public void UpdateHUD()
    {
        healthBar.maxValue = stats.maxHealth + stats.currentShield;
        healthBar.value = stats.currentHealth;
        shieldBar.maxValue = healthBar.maxValue;
        shieldBar.value = stats.currentShield;

        healthValue.text = stats.currentHealth + stats.currentShield + "/" + stats.maxHealth;
        moneyValue.text = "$" + stats.money;

        if (inventory.equippedWeapon != null )
        {
            weaponSprite.enabled = true;
            weaponSprite.sprite = inventory.equippedWeapon.weaponSprite.sprite;
            ammoValue.text = inventory.equippedWeapon.currentAmmo + "/" + inventory.equippedWeapon.maxAmmo;
        } else
        {
            ammoValue.text = "\u221E/\u221E";
        }
    }
}
