using UnityEngine;

public class ThreeDimensionalGlasses : Item, IPlayerUpgrade
{
    public void ApplyPlayerUpgrade()
    {
        var player = FindAnyObjectByType<PlayerController>();
        var bulletSpawnPoint = Instantiate(player.bulletSpawnPoints[0], player.bulletSpawnPoints[0].position, player.bulletSpawnPoints[0].rotation, player.bulletSpawnPoints[0].parent);
        player.bulletSpawnPoints.Add(bulletSpawnPoint);
    }
}
