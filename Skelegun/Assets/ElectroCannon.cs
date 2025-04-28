using UnityEngine;

public class ElectroCannon : Weapon
{
    public override void Shoot()
    {
        Instantiate(bullet, player.bulletSpawnPoint.position, player.bulletSpawnPoint.rotation);
    }
}
