using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Camera cam;
    public Transform player;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    void AimCamera()
    {
        float newCamX = (cam.ScreenToWorldPoint(Input.mousePosition).x + player.position.x) / 2f;
        float newCamY = (cam.ScreenToWorldPoint(Input.mousePosition).y + player.position.y) / 2f;
        cam.transform.position = new Vector3(newCamX, newCamY, -1);
    }

    // Update is called once per frame
    void Update()
    {
        AimCamera();
    }
}
