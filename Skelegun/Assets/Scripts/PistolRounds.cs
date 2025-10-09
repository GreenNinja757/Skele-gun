using UnityEngine;

public class PistolRounds : Item, IPlayerUpgrade
{
    public void ApplyPlayerUpgrade()
    {
        var player = FindAnyObjectByType<PlayerStats>();
        var inventory = FindAnyObjectByType<PlayerInventory>();

        player.bonusBulletDamage *= 1.5f;
        player.bonusFireRate *= 0.75f;
        if (inventory.equippedWeapon != null)
        {
            inventory.equippedWeapon.SetStats();
        }
    }
}
