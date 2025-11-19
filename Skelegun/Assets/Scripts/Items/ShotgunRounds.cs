using System.Collections.Generic;
using UnityEngine;

public class ShotgunRounds : Item, IPlayerUpgrade, IShootEffect
{
    public void ApplyPlayerUpgrade()
    {
        var player = FindAnyObjectByType<PlayerStats>();
        var inventory = FindAnyObjectByType<PlayerInventory>();

        player.bonusBulletDamage *= 0.5f;
        if (inventory.equippedWeapon != null)
        {
            inventory.equippedWeapon.SetStats();
        }
    }

    public void ApplyShootEffect()
    {
        var player = FindAnyObjectByType<PlayerController>();
        var inventory = FindAnyObjectByType<PlayerInventory>();
        var weapon = inventory.equippedWeapon;

        for (int i = 0; i < player.bulletSpawnPoints.Count; i++)
        {
            for (int k = 0; k < player.inventory.equippedWeapon.bulletCount; k++)
            {
                for (int h = 0; h < 2; h++)
                {
                    var bullet = Instantiate(inventory.equippedWeapon.bulletPrefab, player.bulletSpawnPoints[i].transform.position, player.bulletSpawnPoints[i].transform.rotation);

                    bullet.GetComponent<Projectile>().SetStats(weapon.newBulletDamage, weapon.newBulletSpeed, weapon.newBulletSize);

                    switch (h)
                    {
                        case 0:
                            bullet.transform.Rotate(player.bulletSpawnPoints[i].transform.forward, 7.5f + (10 * k) + (-5 * (inventory.equippedWeapon.bulletCount - 1)));
                            break;

                        case 1:
                            bullet.transform.Rotate(player.bulletSpawnPoints[i].transform.forward, -7.5f + (10 * k) + (-5 * (inventory.equippedWeapon.bulletCount - 1)));
                            break;
                    }
                }
            }
        }
    }
}
