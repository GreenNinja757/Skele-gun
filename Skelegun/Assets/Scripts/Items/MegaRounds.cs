using System;
using UnityEngine;

public class MegaRounds : Item, IPlayerUpgrade
{
    public void ApplyPlayerUpgrade()
    {
        var player = FindAnyObjectByType<PlayerStats>();
        var inventory = FindAnyObjectByType<PlayerInventory>();
        player.bonusBulletDamage *= 1.5f;
        player.bonusBulletSpeed *= 0.5f;
        player.bonusBulletSize *= 2;
        if (inventory.equippedWeapon != null)
        {
            inventory.equippedWeapon.SetStats();
        }
    }
}
