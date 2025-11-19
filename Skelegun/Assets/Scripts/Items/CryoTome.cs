using System.Collections;
using UnityEngine;

public class CryoTome : Item, IShootEffect
{
    public GameObject IceBall;

    private bool canShoot;

    //sound effect

    public void Start()
    {
        canShoot = true;
    }

    public void ApplyShootEffect()
    {
        var player = FindAnyObjectByType<PlayerController>();
        var inventory = FindAnyObjectByType<PlayerInventory>();

        if (canShoot)
        {
            var iceBall = Instantiate(IceBall, player.bulletSpawnPoints[0].transform.position, player.bulletSpawnPoints[0].transform.rotation);

            iceBall.GetComponent<Projectile>().SetStats(30f, 3.5f, 2.5f);

            iceBall.transform.Rotate(player.bulletSpawnPoints[0].transform.forward, 0f);

            StartCoroutine(nameof(FireRateTimer));
        }
    }

    public IEnumerator FireRateTimer()
    {
        canShoot = false;
        yield return new WaitForSeconds(5f);
        canShoot = true;
    }
}
