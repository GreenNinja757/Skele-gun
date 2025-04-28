using UnityEngine;

public class Shotgun : Weapon
{
    public override void Shoot()
    {
        Instantiate(bullet, player.bulletSpawnPoint.position, player.bulletSpawnPoint.rotation);
    }
}
