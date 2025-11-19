using UnityEngine;

public class SoldiersBoot: Item, IPlayerUpgrade
{
    public void ApplyPlayerUpgrade()
    {
        var player = FindAnyObjectByType<PlayerStats>();
        player.bonusMoveSpeed += 3;
    }
}
