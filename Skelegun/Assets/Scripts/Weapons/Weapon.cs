using System.Collections;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    public int maxAmmo;
    public int currentAmmo;
    public float fireRate;
    public bool canShoot;
    public bool isAuto;
    public Transform[] bulletSpawnPoints;

    public GameObject bulletPrefab;

    public SpriteRenderer weaponSprite;

    public abstract void Shoot();

    public abstract IEnumerator FireRateTimer();
}
