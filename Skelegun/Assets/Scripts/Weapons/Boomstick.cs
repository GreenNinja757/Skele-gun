using System.Collections;
using UnityEngine;

public class Boomstick : Weapon
{
    public override void Shoot()
    {
        if (canShoot)
        {
            Instantiate(bulletPrefab, bulletSpawnPoints[0].position, bulletSpawnPoints[0].rotation);
            Instantiate(bulletPrefab, bulletSpawnPoints[1].position, bulletSpawnPoints[1].rotation);
            Instantiate(bulletPrefab, bulletSpawnPoints[2].position, bulletSpawnPoints[2].rotation);
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
