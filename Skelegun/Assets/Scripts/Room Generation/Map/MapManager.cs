using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MiniMapManager : MonoBehaviour
{
    public static MiniMapManager instance;
    void Awake() => instance = this;

    public RectTransform mapGrid;
    public RectTransform playerIcon;
    public GameObject roomIconPrefab;
    public GameObject connectionPrefab;
    public Vector2 roomSpacing = new Vector2(32, 32);
    public float sizeScale = 3f;

    private Dictionary<Vector2Int, RectTransform> roomIcons = new ();
    private HashSet<(Vector2Int, Vector2Int)> drawnConnections = new ();

    public void DiscoverRoom(Vector2Int roomPos, Vector2 roomSize)
    {
        if (roomIcons.ContainsKey(roomPos))
            return;

        GameObject icon = Instantiate(roomIconPrefab, mapGrid);
        RectTransform rect = icon.GetComponent<RectTransform>();
        rect.anchoredPosition = new Vector2(roomPos.x * roomSpacing.x, roomPos.y * roomSpacing.y);
        rect.sizeDelta = new Vector2(roomSize.x * sizeScale, roomSize.y * sizeScale);

        roomIcons.Add(roomPos, rect);
    }

    public void UpdatePlayerRoom(Vector2Int newRoomPos)
    {
        mapGrid.anchoredPosition = -new Vector2(newRoomPos.x * roomSpacing.x, newRoomPos.y * roomSpacing.y);
    }

    public void ConnectRooms(Vector2Int roomA, Vector2Int roomB)
    {
        // Avoid duplicates
        if (drawnConnections.Contains((roomA, roomB)) || drawnConnections.Contains((roomB, roomA)))
            return;

        if (!roomIcons.ContainsKey(roomA) || !roomIcons.ContainsKey(roomB))
            return; // can't draw yet

        RectTransform iconA = roomIcons[roomA];
        RectTransform iconB = roomIcons[roomB];

        Vector2 posA = iconA.anchoredPosition;
        Vector2 posB = iconB.anchoredPosition;

        Vector2 dir = posB - posA;
        float distance = dir.magnitude;

        GameObject line = Instantiate(connectionPrefab, mapGrid);
        RectTransform lineRect = line.GetComponent<RectTransform>();

        lineRect.anchoredPosition = posA;
        lineRect.sizeDelta = new Vector2(distance, 4f); // line thickness
        lineRect.rotation = Quaternion.FromToRotation(Vector3.right, dir);

        drawnConnections.Add((roomA, roomB));
    }
}
