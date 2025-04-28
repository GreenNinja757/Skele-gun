using UnityEngine;

public class Pistol : Weapon
{
    public override void Shoot()
    {
        if (bullet == null)
        {
            Debug.Log("bullet");
        }
        if (player.bulletSpawnPoint == null)
        {
            Debug.Log("bulletspawnpoint");
        }
        Instantiate(bullet, player.bulletSpawnPoint.position, player.bulletSpawnPoint.rotation);
    }
}
