using UnityEngine;

public class DeathMode : MonoBehaviour
{
    
    public static bool DeathModeEnabled = false;

    public void ToggleDeathMode()
    {
        DeathModeEnabled = !DeathModeEnabled;
        Debug.Log("Death Mode: " + (DeathModeEnabled ? "ON" : "OFF"));
    }
}


