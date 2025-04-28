using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public HUDManager playerHUD;

    public int maxHealth;
    public int currentHealth;
    public int money;
    public int moveSpeed;

    void Start()
    {
        //base stats
        maxHealth = 10;
        currentHealth = maxHealth;
        money = 0;
        moveSpeed = 5;
        //playerHUD.updateHUD();
    }

    void Update()
    {
        
    }
}
