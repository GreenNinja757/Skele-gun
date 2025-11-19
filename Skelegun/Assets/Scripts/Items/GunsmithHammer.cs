using UnityEngine;

public class GunsmithHammer : Item, IPlayerUpgrade
{
    public void ApplyPlayerUpgrade()
    {
        var player = FindAnyObjectByType<PlayerStats>();
        player.bonusBulletDamage *= 1.25f;
        player.bonusFireRate *= 1.25f;
        player.bonusBulletSpeed *= 1.25f;
        player.bonusBulletSize *= 1.25f;
    }
}
