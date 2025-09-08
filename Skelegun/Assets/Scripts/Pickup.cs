using UnityEngine;

public class Pickup : MonoBehaviour
{
    public int health;
    public int shield;
    public int money;
    public int keys;
    public int ammo;

    public void OnPickup()
    {
        var player = FindAnyObjectByType<PlayerStats>();
        var weapon = FindAnyObjectByType<PlayerInventory>().equippedWeapon;

        player.GiveHealth(health);
        player.GiveShield(shield);
        player.money += money;
        player.keys += keys;
        if (weapon != null)
        {
            weapon.GiveAmmo(ammo);
        }
    }
}
