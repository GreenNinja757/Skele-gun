using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Camera cam;
    public Transform player;
    public float threshold;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        Vector3 targetPos = (player.position - mousePos) / 2f;

        targetPos.x = Mathf.Clamp(targetPos.x, -threshold + player.position.x, threshold + player.position.x);
        targetPos.x = Mathf.Clamp(targetPos.x, -threshold + player.position.y, threshold + player.position.y);

        this.transform.position = targetPos;
    }
}
