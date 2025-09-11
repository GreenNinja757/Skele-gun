using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class RoomSpawner : MonoBehaviour
{


    [Header("Room Prefabs")]
    public GameObject entryRoomPrefab;
    public GameObject combatRoomPrefab;
    public GameObject shopRoomPrefab;
    public GameObject rewardRoomPrefab;
    public GameObject bossRoomPrefab;

    [Header("Settings")]
    public int combatRoomCount = 2;
  
    private List<RoomManager> spawnedRooms = new List<RoomManager>();
    private RoomManager entryRoom;

    void Start()
    {
        GenerateDungeon();
        PlacePlayer();
    }

    void PlacePlayer() { 
    GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null && entryRoom != null && entryRoom.doorEntry != null)
        {
            player.transform.position = entryRoom.doorEntry.position;
        }
    }









    void GenerateDungeon()
    {
        RoomManager previousRoom = null;

        // Entry
        entryRoom = SpawnRoom(entryRoomPrefab, null);
        previousRoom = entryRoom;

        // Combat rooms
        for (int i = 0; i < combatRoomCount; i++)
        {
            RoomManager combat = SpawnRoom(combatRoomPrefab, previousRoom.doorExit);
            previousRoom = combat;
        }

        // Shop
        RoomManager shop = SpawnRoom(shopRoomPrefab, previousRoom.doorExit);
        previousRoom = shop;

        // Reward
        RoomManager reward = SpawnRoom(rewardRoomPrefab, previousRoom.doorExit);
        previousRoom = reward;

        // Boss
        RoomManager boss = SpawnRoom(bossRoomPrefab, previousRoom.doorExit);
        previousRoom = boss;
    }

    RoomManager SpawnRoom(GameObject prefab, Transform previousExit)
    {
        GameObject roomObj = Instantiate(prefab, Vector3.zero, Quaternion.identity);
        RoomManager rm = roomObj.GetComponent<RoomManager>();

        if (previousExit != null && rm.doorEntry != null)
        {
            // Offset = previous exit position - this entry position
            Vector3 offset = previousExit.position - rm.doorEntry.position;
            roomObj.transform.position += offset;
        }

        spawnedRooms.Add(rm);
        return rm;
    
}

}



        
