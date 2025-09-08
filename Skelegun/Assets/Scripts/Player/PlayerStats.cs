using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PlayerStats : MonoBehaviour
{
    public PlayerController player;
    public PlayerHUD hud;

    [Header("Base Player Stats")]
    public float maxHealth;
    public float currentHealth;
    public float shield;
    public float moveSpeed;
    public float size;
    public int money;
    public int keys;

    [Header("Bonus Player Stats")]
    public int bonusMaxHealth;
    public int bonusHealth;
    public int bonusShield;
    public int bonusAmmo;
    public int bonusMoney;
    public int bonusMoveSpeed;
    public int bonusSize;

    [Header("Bonus Weapon Stats")]
    public float bonusBulletDamage;
    public float bonusFireRate;
    public float bonusBulletSpeed;
    public float bonusBulletSize;
    public float bonusAccuracy;
    public int bonusMaxAmmo;
    public int currentAmmo;
    public int ammoCost;
    public int bonusPyroDamage;
    public int bonusCryoDamage;
    public int bonusElectroDamage;

    public bool isInvincible;

    public void GiveHealth(float health)
    {
        currentHealth += health;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
    }

    public void GiveShield(float shield)
    {
        this.shield += shield;
        if (this.shield > maxHealth)
        {
            this.shield = maxHealth;
        }
    }

    public void TakeDamage(float damage)
    {
        if (!isInvincible && player.isAlive)
        {
            if (shield > 0)
            {
                shield -= damage;

                if (shield <= 0)
                {
                    shield = 0;
                }
            } 
            else
            {
                currentHealth -= damage;

                if (currentHealth <= 0)
                {
                    currentHealth = 0;
                    player.isAlive = false;

                    Die();
                }
            }

            Mathf.Round(currentHealth);

            StartCoroutine(nameof(InvincibilityTimer));
            hud.UpdateHUD();
        }
    }

    public void Die()
    {
        player.animator.Play("Die");
        if (player.inventory.equippedWeapon != null)
        {
            player.inventory.equippedWeapon.GetComponent<SpriteRenderer>().enabled = false;
            if (player.inventory.equippedWeapon.GetComponent<Light2D>() != null)
            {
                player.inventory.equippedWeapon.GetComponent<Light2D>().enabled = false;
            }
        }
        player.rb.constraints = RigidbodyConstraints2D.FreezePosition;
    }

    public IEnumerable InvincibilityTimer()
    {
        //play hurt animation
        isInvincible = true;
        yield return new WaitForSeconds(1f);
        isInvincible = false;
    }
}
