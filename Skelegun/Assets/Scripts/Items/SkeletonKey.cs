using UnityEngine;

public class SkeletonKey : Item, IPlayerUpgrade
{
    public void ApplyPlayerUpgrade()
    {
        var player = FindAnyObjectByType<PlayerStats>();
        player.keys += 99;
    }
}
