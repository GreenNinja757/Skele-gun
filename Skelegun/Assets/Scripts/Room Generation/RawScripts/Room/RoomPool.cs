using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "RoomPool", menuName = "Level Generation/Room Pool", order = 1)]
public class RoomPool : ScriptableObject
{
    [Tooltip("Type of rooms in this pool (Combat, Shop, etc.)")]
    public RoomType poolType;

    [Tooltip("All available room prefabs of this type (drag prefabs with RoomData component here)")]
    public List<GameObject> roomPrefabs = new List<GameObject>();


    public RoomData GetRandomRoom(HashSet<RoomData> usedRooms)
    {
        if (roomPrefabs == null || roomPrefabs.Count == 0)
        {
            Debug.LogError($"RoomPool '{name}' is empty!");
            return null;
        }

        // Build list of available (unused) rooms
        List<RoomData> availableRooms = new List<RoomData>();
        foreach (GameObject prefab in roomPrefabs)
        {
            if (prefab == null)
                continue;

            RoomData roomData = prefab.GetComponent<RoomData>();
            if (roomData != null && !usedRooms.Contains(roomData))
            {
                availableRooms.Add(roomData);
            }
        }

        if (availableRooms.Count == 0)
        {
            Debug.LogWarning($"RoomPool '{name}' has no available rooms (all used)! Allowing reuse.");
            // Fallback: allow reuse if pool exhausted
            foreach (GameObject prefab in roomPrefabs)
            {
                if (prefab != null)
                {
                    RoomData roomData = prefab.GetComponent<RoomData>();
                    if (roomData != null)
                        availableRooms.Add(roomData);
                }
            }
        }

        if (availableRooms.Count == 0)
        {
            Debug.LogError($"RoomPool '{name}' has no valid RoomData components!");
            return null;
        }

        return availableRooms[Random.Range(0, availableRooms.Count)];
    }

    public bool IsValid()
    {
        if (roomPrefabs == null || roomPrefabs.Count == 0)
            return false;

        // Check if at least one prefab has RoomData
        foreach (GameObject prefab in roomPrefabs)
        {
            if (prefab != null && prefab.GetComponent<RoomData>() != null)
                return true;
        }

        return false;
    }

    [ContextMenu("Validate Pool")]
    public void ValidatePool()
    {
        if (roomPrefabs.Count == 0)
        {
            Debug.LogWarning($"RoomPool '{name}' has no prefabs!");
            return;
        }

        int nullCount = 0;
        int missingRoomData = 0;
        int typeMismatch = 0;
        int validCount = 0;

        foreach (GameObject prefab in roomPrefabs)
        {
            if (prefab == null)
            {
                nullCount++;
                continue;
            }

            RoomData roomData = prefab.GetComponent<RoomData>();
            if (roomData == null)
            {
                Debug.LogWarning($"Prefab '{prefab.name}' has no RoomData component!");
                missingRoomData++;
                continue;
            }

            if (roomData.roomType != poolType)
            {
                Debug.LogWarning($"Room '{prefab.name}' type ({roomData.roomType}) doesn't match pool type ({poolType})!");
                typeMismatch++;
            }

            validCount++;
        }

        if (nullCount > 0)
            Debug.LogWarning($"Pool has {nullCount} null entries!");

        if (missingRoomData > 0)
            Debug.LogWarning($"Pool has {missingRoomData} prefabs without RoomData component!");

        Debug.Log($"Pool '{name}' validated: {validCount} valid rooms, {typeMismatch} type mismatches");
    }
}