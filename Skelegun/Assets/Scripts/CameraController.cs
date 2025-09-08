using System.Collections;
using System.Security.Cryptography;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Camera cam;
    public Transform player;
    public Vector3 cameraShake;
    public float displacementMultiplier;

    void AimCamera()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 cameraDisplacement = (mousePos - player.transform.position) * displacementMultiplier;

        Vector3 finalCameraPos = player.transform.position + cameraDisplacement + cameraShake;
        finalCameraPos.z = -1;
        transform.position = finalCameraPos;
    }

    public IEnumerator ShakeCamera()
    {
        cameraShake = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0);
        cameraShake.Normalize();
        cameraShake.x /= 20f;
        cameraShake.y /= 20f;
        yield return new WaitForSeconds(.1f);
        cameraShake = Vector3.zero;   
    }

    // Update is called once per frame
    void Update()
    {
        AimCamera();
    }
}
