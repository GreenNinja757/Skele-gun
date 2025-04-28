using UnityEngine;

public abstract class Item : MonoBehaviour
{
    public int maxHealth;
    public int health;
    public int maxAmmo;
    public int ammo;
    public int money;
    public int moveSpeed;

    public SpriteRenderer sr;

    public abstract void applyStats();

    public abstract void applyPassiveEffect();

    public abstract void applyOnGetHitEffect();

    public abstract void applyOnHitEffect();

    public abstract void applyOnKillEffect();
}
