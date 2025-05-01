using UnityEngine;

public abstract class Item : MonoBehaviour
{
    public int maxHealth;
    public int health;
    public int maxAmmo;
    public int ammo;
    public int money;
    public int moveSpeed;

    public SpriteRenderer itemSprite;

    public abstract void OnPickup();

    public abstract void OnShoot();

    public abstract void OnInterval();

    public abstract void OnHit();

    public abstract void OnGetHit();

    public abstract void OnKill();

    public abstract void OnKilled();
}
