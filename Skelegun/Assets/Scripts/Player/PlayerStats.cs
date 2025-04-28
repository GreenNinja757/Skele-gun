using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public PlayerHUD playerHUD;

    public int maxHealth;
    public int currentHealth;
    public int money;
    public float moveSpeed;

    void Start()
    {
        //base stats
        maxHealth = 10;
        currentHealth = maxHealth;
        money = 0;
        moveSpeed = 5f;

        playerHUD.UpdateHealth();
        playerHUD.UpdateAmmo();
        playerHUD.UpdateMoney();
    }
}
