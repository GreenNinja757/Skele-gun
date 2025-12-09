using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapUI : MonoBehaviour
{
    public RectTransform minimapRoot;

    public GameObject roomIconPrefab;
    public GameObject connectionIconPrefab;

    private Dictionary<Vector2Int, GameObject> roomIcons =
        new Dictionary<Vector2Int, GameObject>();

    private Dictionary<(Vector2Int, Vector2Int), GameObject> connections =
        new Dictionary<(Vector2Int, Vector2Int), GameObject>();

    public float iconSpacing = 32f; // distance between minimap nodes

    public void CreateRoomIcon(Vector2Int gridPos, RoomType type)
    {
        GameObject icon = Instantiate(roomIconPrefab, minimapRoot);
        icon.GetComponent<Image>().color = GetRoomColor(type);
        icon.SetActive(false);

        icon.GetComponent<RectTransform>().anchoredPosition =
            new Vector2(gridPos.x * iconSpacing, gridPos.y * iconSpacing);

        roomIcons.Add(gridPos, icon);
    }

    public void SetRoomVisible(Vector2Int gridPos, bool visible)
    {
        if (roomIcons.TryGetValue(gridPos, out GameObject icon))
            icon.SetActive(visible);
    }

    public void ShowConnection(Vector2Int a, Vector2Int b)
    {
        var key = (a, b);
        if (connections.ContainsKey(key)) return;

        GameObject line = Instantiate(connectionIconPrefab, minimapRoot);

        Vector2 posA = new Vector2(a.x, a.y) * iconSpacing;
        Vector2 posB = new Vector2(b.x, b.y) * iconSpacing;

        RectTransform rt = line.GetComponent<RectTransform>();
        rt.anchoredPosition = (posA + posB) / 2f;
        rt.sizeDelta = new Vector2(
            (posA - posB).magnitude,
            rt.sizeDelta.y
        );

        float angle = Mathf.Atan2(posB.y - posA.y, posB.x - posA.x) * Mathf.Rad2Deg;
        rt.rotation = Quaternion.Euler(0, 0, angle);

        connections[key] = line;
    }

    private Color GetRoomColor(RoomType type)
    {
        switch (type)
        {
            case RoomType.Reward: return Color.yellow;
            case RoomType.Boss: return Color.red;
            case RoomType.Shop: return Color.magenta;
            case RoomType.Spawn: return Color.green;
            default: return Color.white;
        }
    }
}
