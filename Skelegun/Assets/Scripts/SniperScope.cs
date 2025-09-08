using UnityEngine;

public class SniperScope : Item, IPlayerUpgrade
{
    public Texture2D sniperScope;

    public void ApplyPlayerUpgrade()
    {
        var camera = FindAnyObjectByType<CameraController>();
        camera.displacementMultiplier = 0.5f;
        Cursor.SetCursor(sniperScope, new Vector2(16f, 16f), CursorMode.Auto);
    }
}
