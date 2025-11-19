using UnityEngine;
using System.Collections.Generic;

public class RoomSpawner : MonoBehaviour
{
    [Header("Room Settings")]
    public GameObject[] roomPrefabs; // Regular room prefabs
    public GameObject spawnRoomPrefab;
    public GameObject finalRoomPrefab;
    public Transform roomsParent;
    public int maxRooms;
    public float roomSize;

    [Header("Enemy Settings")]
    public GameObject[] normalEnemyPrefabs; // Enemies for regular rooms
    public GameObject[] finalRoomEnemyPrefabs; // Enemies for the final room

    [Header("Final Room Settings")]
    public string nextLevelSceneName = "NextLevel"; // Scene to load when leaving final room
    public string finalRoomTeleporterName = "FinalExitTeleporter"; // A special teleporter in the final room

    private Dictionary<Vector2, GameObject> placedRooms = new Dictionary<Vector2, GameObject>();
    private List<Vector2> roomPositions = new List<Vector2>();
    private List<GameObject> spawnedRooms = new List<GameObject>();

    private Vector2[] directions = { Vector2.up, Vector2.down, Vector2.left, Vector2.right };

    void Awake()
    {
        // Ensure fresh initialization on scene load
        placedRooms = new Dictionary<Vector2, GameObject>();
        roomPositions = new List<Vector2>();
        spawnedRooms = new List<GameObject>();
    }

    void Start()
    {
        GenerateRooms();
        EnsureTeleporterConnections();  // New step: force-create missing adjacent rooms.
        LinkTeleporters();
        SpawnEnemiesInAllRooms();
        PlacePlayer();
    }

    void GenerateRooms()
    {
        // First room is the spawn room
        Vector2 startPosition = RoundVector2(Vector2.zero);
        roomPositions.Add(startPosition);
        GameObject firstRoom = Instantiate(spawnRoomPrefab, startPosition, Quaternion.identity, roomsParent);
        placedRooms.Add(startPosition, firstRoom);
        spawnedRooms.Add(firstRoom);

        //generate random rooms
        for (int i = 1; i < maxRooms; i++)
        {
            Vector2 newPos = RoundVector2(GetNewRoomPosition());
            if (!placedRooms.ContainsKey(newPos))
            {
                roomPositions.Add(newPos);
                GameObject room = SpawnRoom(newPos);
                placedRooms[newPos] = room;
                spawnedRooms.Add(room);
            }
        }

        //Generate treasure rooms

        //Generate shrine rooms

        //Generate secret rooms

        // Replace the last room with the final room
        if (spawnedRooms.Count > 0 && finalRoomPrefab != null)
        {
            GameObject lastRoom = spawnedRooms[spawnedRooms.Count - 1];
            Vector2 lastRoomPosition = lastRoom.transform.position;

            // Destroy the last randomly generated room and replace it
            Destroy(lastRoom);
            spawnedRooms.RemoveAt(spawnedRooms.Count - 1);
            placedRooms.Remove(lastRoomPosition);

            // Instantiate final room
            GameObject finalRoom = Instantiate(finalRoomPrefab, lastRoomPosition, Quaternion.identity, roomsParent);
            spawnedRooms.Add(finalRoom);
            placedRooms[lastRoomPosition] = finalRoom;
        }
    }

    void EnsureTeleporterConnections()
    {
        List<Vector2> positionsToAdd = new List<Vector2>();

        // Iterate over each placed room.
        foreach (var entry in placedRooms)
        {
            Vector2 roomPos = entry.Key;
            GameObject room = entry.Value;

            // Check each of the four standard teleporters.
            foreach (string teleporterName in new string[] { "NorthTeleporter", "SouthTeleporter", "EastTeleporter", "WestTeleporter" })
            {
                // Adjust the path if your teleporters are under a parent (e.g., "Teleporters/NorthTeleporter")
                Transform tp = room.transform.Find("Teleporters/" + teleporterName);
                if (tp != null && tp.gameObject.activeSelf)
                {
                    // Calculate target room position.
                    Vector2 targetPos = GetOppositeRoomPosition(roomPos, teleporterName);
                    targetPos = RoundVector2(targetPos);  // Ensure consistency.
                    if (!placedRooms.ContainsKey(targetPos))
                    {
                        // Mark this position to create a room.
                        positionsToAdd.Add(targetPos);
                    }
                }
            }
        }

        // Spawn rooms for all target positions that don't already exist.
        foreach (Vector2 pos in positionsToAdd)
        {
            if (!placedRooms.ContainsKey(pos))
            {
                GameObject newRoom = SpawnRoom(pos);
                placedRooms.Add(pos, newRoom);
                spawnedRooms.Add(newRoom);
                roomPositions.Add(pos);
                Debug.Log("Force-created room at: " + pos);
            }
        }
    }

    GameObject SpawnRoom(Vector2 position)
    {
        // For simplicity, we always choose a room from roomPrefabs.
        GameObject newRoom = Instantiate(roomPrefabs[Random.Range(0, roomPrefabs.Length)], position, Quaternion.identity, roomsParent);
        ActivateRandomTeleporters(newRoom);
        return newRoom;
    }

    void ActivateRandomTeleporters(GameObject room)
    {
        List<string> allTeleporters = new List<string> { "NorthTeleporter", "SouthTeleporter", "EastTeleporter", "WestTeleporter" };
        int teleporterCount = Random.Range(1, 5);

        // Shuffle and activate a random number of teleporters
        for (int i = 0; i < teleporterCount; i++)
        {
            string teleporterName = allTeleporters[Random.Range(0, allTeleporters.Count)];
            Transform teleporter = room.transform.Find(teleporterName);
            if (teleporter != null) teleporter.gameObject.SetActive(true);
            allTeleporters.Remove(teleporterName);
        }
    }

    Vector2 GetNewRoomPosition()
    {
        // Simple approach: choose a random direction from the last room.
        Vector2 lastPos = roomPositions[roomPositions.Count - 1];
        Vector2 direction = directions[Random.Range(0, directions.Length)];
        Vector2 newPos = lastPos + direction * roomSize;
        return RoundVector2(newPos);
    }

    void LinkTeleporters()
    {
        foreach (var entry in placedRooms)
        {
            Vector2 roomPos = entry.Key;
            GameObject room = entry.Value;

            foreach (string teleporterName in new string[] { "NorthTeleporter", "SouthTeleporter", "EastTeleporter", "WestTeleporter" })
            {
                Transform tp = room.transform.Find("Teleporters/" + teleporterName);
                if (tp != null && tp.gameObject.activeSelf)
                {
                    Vector2 targetPos = GetOppositeRoomPosition(roomPos, teleporterName);

                    // Debug Log: Checking the computed teleportation target
                    Debug.Log($"[{teleporterName}] in Room at {roomPos} ? TargetPos: {targetPos}");

                    if (placedRooms.ContainsKey(targetPos) && placedRooms[targetPos] != room)
                    {
                        GameObject linkedRoom = placedRooms[targetPos];
                        Transform linkedTP = linkedRoom.transform.Find("Teleporters/" + GetOppositeTeleporterName(teleporterName));

                        if (linkedTP != null)
                        {
                            // Debug Log: Successful linking
                            Debug.Log($"[LINKED] {teleporterName} in Room at {roomPos} ? {GetOppositeTeleporterName(teleporterName)} in Room at {targetPos}");

                            TeleporterScript teleporterComp = tp.GetComponent<TeleporterScript>();
                            TeleporterScript linkedTeleporterComp = linkedTP.GetComponent<TeleporterScript>();

                            if (teleporterComp != null && linkedTeleporterComp != null)
                            {
                                teleporterComp.SetDestination(linkedTP.position);
                                linkedTeleporterComp.SetDestination(tp.position); // Ensure the reverse link exists
                            }
                        }
                        else
                        {
                            Debug.LogWarning($"[MISSING TELEPORTER] {GetOppositeTeleporterName(teleporterName)} not found in Room at {targetPos}");
                            HideTeleporterSprite(tp);
                        }
                    }
                    else
                    {
                        Debug.LogWarning($"[NO LINK] No room found at computed targetPos {targetPos} for {teleporterName} in Room at {roomPos}");
                        HideTeleporterSprite(tp);
                    }
                }
            }
        }
    }

    void HideTeleporterSprite(Transform teleporterTransform)
    {
        SpriteRenderer sr = teleporterTransform.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.enabled = false;
        }
    }

    Vector2 GetOppositeRoomPosition(Vector2 currentPos, string teleporterName)
    {
        Vector2 offset = teleporterName switch
        {
            "NorthTeleporter" => new Vector2(0, roomSize),  // Keep centered
            "SouthTeleporter" => new Vector2(0, -roomSize),
            "EastTeleporter" => new Vector2(roomSize, 0),  // Shift X correctly
            "WestTeleporter" => new Vector2(-roomSize, 0),
            _ => Vector2.zero
        };

        return RoundVector2(currentPos + offset);
    }

    string GetOppositeTeleporterName(string teleporterName)
    {
        return teleporterName switch
        {
            "NorthTeleporter" => "SouthTeleporter",
            "SouthTeleporter" => "NorthTeleporter",
            "EastTeleporter" => "WestTeleporter",
            "WestTeleporter" => "EastTeleporter",
            _ => ""
        };
    }

    void SpawnEnemiesInAllRooms()
    {
        for (int i = 0; i < spawnedRooms.Count; i++)
        {
            GameObject room = spawnedRooms[i];
            Transform enemySpawns = room.transform.Find("EnemySpawns");

            if (enemySpawns != null)
            {
                
                bool isBossRoom = room.CompareTag("Finish");

                // Assign the correct enemy list based on whether it's the boss room
                GameObject[] enemyList = isBossRoom ? finalRoomEnemyPrefabs : normalEnemyPrefabs;

                foreach (Transform spawnPoint in enemySpawns)
                {
                    if (Random.value > 0.5f) // 50% chance to spawn an enemy
                    {
                        Instantiate(enemyList[Random.Range(0, enemyList.Length)], spawnPoint.position, Quaternion.identity);
                    }
                }
            }
        }
    }

    void PlacePlayer()
    {
        foreach (Vector2 pos in placedRooms.Keys)
            Debug.Log("Room at: " + pos);
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null && spawnedRooms.Count > 0)
        {
            player.transform.position = spawnedRooms[0].transform.position;
        }
    }

    private Vector2 RoundVector2(Vector2 vec)
    {
        float precision = 10f; // Multiplier for one decimal place (adjust as needed)
        float roundedX = Mathf.Round(vec.x * precision) / precision;
        float roundedY = Mathf.Round(vec.y * precision) / precision;
        return new Vector2(roundedX, roundedY);
    }
}