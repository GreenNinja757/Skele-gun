using UnityEngine;

public class DoubleShotgun : Weapon
{
    public override void Shoot()
    {
        Instantiate(bullet, player.bulletSpawnPoint.position, player.bulletSpawnPoint.rotation);
    }
}
