using UnityEngine;

public class Milk : Item, IPlayerUpgrade
{
    public void ApplyPlayerUpgrade()
    {
        var player = FindAnyObjectByType<PlayerStats>();
        player.maxHealth += 5;
        player.currentHealth += 5;
    }
}
