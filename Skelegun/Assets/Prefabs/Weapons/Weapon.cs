using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    public HUDManager playerHUD;

    public int maxAmmo;
    public int currentAmmo;
    public int fireRate;
    public int projectileCount;
    public float accuracy;

    public SpriteRenderer sprite;

    public string flavorText;

    public void Start()
    {
        //playerHUD.updateHUD();
    }

    public abstract void Shoot();
}
