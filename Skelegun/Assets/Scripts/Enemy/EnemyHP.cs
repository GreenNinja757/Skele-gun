using UnityEngine;
using System;

public class EnemyHP : MonoBehaviour
{
    public int maxHealth = 3;
    private int currentHealth;
    public event Action<EnemyHP> OnDeath;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log("Enemy took damage! Current HP: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        Debug.Log("Enemy defeated!");
        OnDeath?.Invoke(this);
        Destroy(gameObject);
    }
}