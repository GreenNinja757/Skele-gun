using System.Collections;
using UnityEngine;

public class SniperRifle : Weapon
{
    public override void Shoot()
    {
        if (canShoot)
        {
            //set bullet stats in gun script
            var bullet = Instantiate(bulletPrefab, bulletSpawnPoints[0].position, bulletSpawnPoints[0].rotation);
            bullet.GetComponent<Bullet>().damage = 5;
            bullet.GetComponent<Bullet>().speed = 50f;
            canShoot = false;
            StartCoroutine("FireRateTimer");
        }
    }

    public override IEnumerator FireRateTimer()
    {
        yield return new WaitForSeconds(1 / fireRate);
        canShoot = true;
    }
}
