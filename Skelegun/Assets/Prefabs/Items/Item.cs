using UnityEngine;

public abstract class Item : MonoBehaviour
{
    public HUDManager playerHUD;

    public int maxHealth;
    public int health;
    public int maxAmmo;
    public int ammo;
    public int money;
    public int moveSpeed;

    public SpriteRenderer sprite;

    public string flavorText;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            //add to inventory
            //update item hud
        }
    }

    public abstract void applyStats();

    public abstract void applyPassiveEffect();

    public abstract void applyOnGetHitEffect();

    public abstract void applyOnHitEffect();

    public abstract void applyOnKillEffect();
}
