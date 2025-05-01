using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public PlayerHUD playerHUD;

    public int maxHealth;
    public int currentHealth;
    public int currentShield;
    public int money;
    public int moveSpeed;
    public int bonusHealth;
    public int bonusMoveSpeed;
    public int bonusFireRate;
    public int bonusAmmo;
    public int bonusDamage;
    public int bonusMoney;
    public int bonusPyroDamage;
    public int bonusCryoDamage;
    public int bonusElectroDamage;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        maxHealth = 10;
        currentHealth = maxHealth;
        currentShield = 0;
        money = 0;
        moveSpeed = 5;
    }
}
