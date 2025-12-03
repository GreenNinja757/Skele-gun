using UnityEngine;

public class MapPlayerIcon
{
    public RectTransform icon;
    public Transform player;
    public float scale = 0.1f; // world → minimap scale


    void Update()
    {
        icon.anchoredPosition = new Vector2(
            player.position.x * scale,
            player.position.y * scale
        );
    }


}
