using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviour
{
    public PlayerStats player;
    public PlayerInventory playerInventory;

    public Slider healthBar;

    public TextMeshProUGUI healthValue;
    public TextMeshProUGUI moneyValue;
    public TextMeshProUGUI ammoValue;

    public Grid itemSprites;
    public Image weaponSprite;

    public void UpdateHealth()
    {
        healthBar.value = player.currentHealth;
        healthBar.maxValue = player.maxHealth;
        healthValue.text = player.currentHealth + "/" + player.maxHealth;
    }

    public void UpdateMoney()
    {
        moneyValue.text = player.money + " $";
    }
    public void UpdateAmmo()
    {
        if (playerInventory.equippedWeapon != null)
        {
            ammoValue.text = playerInventory.equippedWeapon.currentAmmo + "/" + playerInventory.equippedWeapon.maxAmmo;
        }
    }
    public void UpdateWeapon()
    {
        //weaponSprite.sprite = playerInventory.equippedWeapon.sr.sprite;
    }

    public void UpdateItems()
    {

    }
}
