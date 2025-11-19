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
                StartCoroutine(nameof(BurnTimer));
                break;

            case "ice":
                iceStacks++;
                StartCoroutine(nameof(FreezeTimer));
                break;

            case "lightning":
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
        yield return new WaitForSeconds(1);
        fireStacks--;
        TakeDamage("fire", 3, false);
    }

    public IEnumerator FreezeTimer()
    {
        yield return new WaitForSeconds(1);
        iceStacks--;
        TakeDamage("ice", 3, false);
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