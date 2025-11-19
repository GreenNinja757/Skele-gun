using UnityEngine;

public class HuntersGlove : Item, IPlayerUpgrade
{
    public void ApplyPlayerUpgrade()
    {
        var player = FindAnyObjectByType<PlayerStats>();
        player.bonusFireRate *= 0.25f;
    }
}
