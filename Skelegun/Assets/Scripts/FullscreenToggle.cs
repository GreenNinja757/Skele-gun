using UnityEngine;

public class FullscreenToggle : MonoBehaviour
{
    public void ToggleFullscreen()
    {
        Screen.fullScreen = !Screen.fullScreen;
        print("Toggled screen");
    }
}