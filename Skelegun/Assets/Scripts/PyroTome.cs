using System.Collections;
using UnityEngine;

public class PyroTome : Item, IShootEffect
{
    public GameObject FireBall;

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
            for (int i = 0; i < 3; i++)
            {
                var fireBall = Instantiate(FireBall, player.bulletSpawnPoints[0].transform.position, player.bulletSpawnPoints[0].transform.rotation);

                fireBall.GetComponent<Projectile>().SetStats(10f, 4.5f, 1.5f);

                switch (i)
                {
                    case 0:
                        fireBall.transform.Rotate(player.bulletSpawnPoints[0].transform.forward, 15f);
                        break;

                    case 1:
                        fireBall.transform.Rotate(player.bulletSpawnPoints[0].transform.forward, 0f);
                        break;

                    case 2:
                        fireBall.transform.Rotate(player.bulletSpawnPoints[0].transform.forward, -15f);
                        break;
                }
            }

            StartCoroutine(nameof(FireRateTimer));
        }
    }

    public IEnumerator FireRateTimer()
    {
        canShoot = false;
        yield return new WaitForSeconds(3);
        canShoot = true;
    }
}
