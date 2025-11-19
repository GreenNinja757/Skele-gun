using System.Collections;
using UnityEngine;
using System;

public class EnemyStats : MonoBehaviour
{
    public EnemyHUD enemyHud;

    public GameObject damageNumberPrefab;

    public float maxHealth;
    public float currentHealth;

    public int fireStacks;
    public int iceStacks;
    public int lightningStacks;
    public int doomStacks;

    public event Action<EnemyStats> OnDeath;

    public bool isBurning;
    public bool isFreezing;
    public bool isShocking;


    public bool isStunned;

    public void TakeDamage(string type, float damage, bool isCrit)
    {
        if (isCrit)
        {
            currentHealth -= damage * 2;
        }
        else
        {
            currentHealth -= damage;
        }

        if (currentHealth <= 0)
        {
            var player = FindAnyObjectByType<PlayerInventory>();
            for (int i = 0; i < player.itemInventory.Count; i++)
            {
                //player.itemInventory[i].OnEnemyKill(gameObject);
            }
            Die();
        }

        var damageNumber = Instantiate(damageNumberPrefab, transform.position, Quaternion.identity);
        damageNumber.GetComponent<DamageNumber>().SetValue(type, damage, isCrit);

        Mathf.Round(currentHealth);

        enemyHud.UpdateHUD();
    }

    public void ApplyStatusEffect(string type)
    {
        switch (type)
        {
            case "fire":
                fireStacks++;
                if (!isBurning)
                {
                    StartCoroutine(nameof(BurnTimer));
                }
                break;

            case "ice":
                iceStacks++;
                if (!isFreezing)
                {
                    StartCoroutine(nameof(FreezeTimer));
                }
                break;

            case "lightning":
                lightningStacks++;
                if (!isShocking)
                {
                    StartCoroutine(nameof(ShockTimer));
                }
                break;

            case "doom":
                doomStacks++;
                if (doomStacks == currentHealth)
                {
                    //die
                    currentHealth = 0;
                }
                break;
        }

        enemyHud.UpdateHUD();
    }

    public IEnumerator BurnTimer()
    {
        isBurning = true;
        yield return new WaitForSeconds(1);
        TakeDamage("fire", 1, false);
        fireStacks--;
        if (fireStacks > 0)
        {
            StartCoroutine(nameof(BurnTimer));
        }
    }

    public IEnumerator FreezeTimer()
    {
        isFreezing = true;
        yield return new WaitForSeconds(1);
        TakeDamage("ice", 1, false);
        iceStacks--;
        if (iceStacks > 0)
        {
            StartCoroutine(nameof(FreezeTimer));
        }
    }

    public IEnumerator ShockTimer()
    {
        isShocking = true;
        yield return new WaitForSeconds(1);
        TakeDamage("lightning", 1, false);
        lightningStacks--;
        if (lightningStacks > 0)
        {
            StartCoroutine(nameof(ShockTimer));
        }
    }


    public void Die()
    {
        Debug.Log("Enemy defeated!");
        OnDeath?.Invoke(this);
        Destroy(gameObject);
    }



    public void Start()
    {
        currentHealth = maxHealth;
    }
}