using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHUD : MonoBehaviour
{
    public EnemyStats stats;

    public Slider healthBar;

    public Image burnSprite;
    public Image freezeSprite;
    public Image shockSprite;
    public Image doomSprite;

    void Start()
    {
        UpdateHUD();
    }

    public void UpdateHUD()
    {
        healthBar.maxValue = stats.maxHealth;
        healthBar.value = stats.currentHealth;

        if (stats.fireStacks > 0)
        {
            burnSprite.enabled = true;
        } 
        else
        {
            burnSprite.enabled = false;
        }

        if (stats.iceStacks > 0)
        {
            freezeSprite.enabled = true;
        }
        else
        {
            freezeSprite.enabled = false;
        }

        if (stats.lightningStacks > 0)
        {
            shockSprite.enabled = true;
        }
        else
        {
            shockSprite.enabled = false;
        }

        if (stats.doomStacks > 0)
        {
            doomSprite.enabled = true;
        }
        else
        {
            doomSprite.enabled = false;
        }
    }
}
