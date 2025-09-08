using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [Header("Base Weapon Stats")]
    public float bulletDamage;
    public float fireRate;
    public float bulletSpeed;
    public float bulletSize;
    public float accuracy;
    public int maxAmmo;
    public int currentAmmo;
    public int ammoCost;
    public int bulletCount;
    public bool isAuto;

    public float newBulletDamage;
    public float newFireRate;
    public float newBulletSpeed;
    public float newBulletSize;
    public float newAccuracy;

    public GameObject bulletPrefab;
    public GameObject muzzleFlashPrefab;
    public Sprite muzzleFlashSprite;

    public SpriteRenderer weaponSprite;
    public string nameText;
    public string flavorText;

    public AudioClip shootSFX;
    public AudioClip emptyMagSFX;

    protected PlayerController playerController;
    protected PlayerInventory playerInventory;
    protected PlayerStats playerStats;

    private bool canShoot;

    public void Start()
    {
        weaponSprite = GetComponent<SpriteRenderer>();    
        playerController = FindAnyObjectByType<PlayerController>();
        playerInventory = FindAnyObjectByType<PlayerInventory>();
        playerStats = FindAnyObjectByType<PlayerStats>();
        SetStats();
    }

    public void SetStats()
    {
        newBulletDamage = bulletDamage * playerStats.bonusBulletDamage;
        newFireRate = fireRate * playerStats.bonusFireRate;
        newBulletSpeed = bulletSpeed * playerStats.bonusBulletSpeed;
        newBulletSize = bulletSize * playerStats.bonusBulletSize;
        newAccuracy = accuracy * playerStats.bonusAccuracy;
        canShoot = true;
    } 

    public void Shoot(List<Transform> bulletSpawnPoints)
    {
        if (canShoot && currentAmmo >= ammoCost && currentAmmo != 0)
        {
            for (int i = 0; i < bulletSpawnPoints.Count; i++)
            {
                for (int k = 0; k < playerInventory.itemInventory.Count; k++)
                {
                    playerInventory.itemInventory[k].GetComponent<IShootEffect>()?.ApplyShootEffect();
                }

                for (int k = 0; k < bulletCount; k++) 
                {
                    var bullet = Instantiate(bulletPrefab, bulletSpawnPoints[i].position, bulletSpawnPoints[i].rotation);

                    bullet.transform.Rotate(bulletSpawnPoints[i].transform.forward, (10 * k) + (-5 * (bulletCount - 1)));

                    bullet.GetComponent<Projectile>().SetStats(newBulletDamage, newBulletSpeed, newBulletSize);
                }

                var muzzleFlash = Instantiate(muzzleFlashPrefab, bulletSpawnPoints[i].position, bulletSpawnPoints[i].rotation, transform);

                muzzleFlash.GetComponent<MuzzleFlash>().SetSprite(muzzleFlashSprite);
            }
            StartCoroutine(nameof(FireRateTimer));

            currentAmmo -= ammoCost;

            var audioManager = FindAnyObjectByType<AudioManager>();
            audioManager.PlayWeaponSFX(shootSFX);

            var cam = FindAnyObjectByType<Camera>().GetComponent<CameraController>();
            cam.StartCoroutine(cam.ShakeCamera());
        } 
        else if (currentAmmo == 0) 
        {
            var audioManager = FindAnyObjectByType<AudioManager>();
            audioManager.PlayWeaponSFX(emptyMagSFX);
        }
    }

    public void GiveAmmo(int ammo)
    {
        currentAmmo += ammo;
        if (currentAmmo > maxAmmo)
        {
            currentAmmo = maxAmmo;
        }
    }

    public IEnumerator FireRateTimer()
    {
        canShoot = false;
        yield return new WaitForSeconds(1 / newFireRate);
        canShoot = true;
    }
}
