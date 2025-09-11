using UnityEngine;

public class RoomManager : MonoBehaviour
{
    public enum RoomType
    {
        Entry,
        Combat,
        Shop,
        Reward,
        Boss
    }

    public RoomType roomType;
   
    [Header("Door Anchors")]
    public Transform doorEntry;  
    public Transform doorExit;  


}