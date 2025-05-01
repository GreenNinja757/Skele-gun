using System.Collections;
using UnityEngine;

public class AssaultRifle : Weapon
{
    public override void Shoot()
    {
        if (canShoot)
        {
            Instantiate(bulletPrefab, bulletSpawnPoints[0].position, bulletSpawnPoints[0].rotation);
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
