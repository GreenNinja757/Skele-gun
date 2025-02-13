using UnityEditor.SpeedTree.Importer;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public PlayerInput playerInput;

    public Transform weaponSpawnPoint;
    public Transform bulletSpawnPoint;
    public GameObject bulletPrefab;

    public Rigidbody2D rb;
    public Vector2 horMov;
    public Vector2 verMov;
    public float moveSpeed;
    public float bulletSpeed;

    public SpriteRenderer sr;

    public Camera cam;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
         
    }

    void Move()
    {
        horMov = playerInput.actions["Move"].ReadValue<Vector2>() * moveSpeed;
        verMov = playerInput.actions["Move"].ReadValue<Vector2>() * moveSpeed;

        rb.linearVelocityX = horMov.x;
        rb.linearVelocityY = horMov.y;
    }
    
    void Shoot()
    {
        Vector3 diff = Camera.main.ScreenToWorldPoint(Input.mousePosition) - bulletSpawnPoint.transform.position;
        diff.Normalize();
        float rotZ = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
        weaponSpawnPoint.rotation = Quaternion.Euler(0f, 0f, rotZ);

        if (playerInput.actions["Shoot"].WasPressedThisFrame())
        {
            Instantiate(bulletPrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
        }
    }

    void Flip()
    {
        if (rb.linearVelocity.x < 0)
        {
            sr.flipX = true;
        } else if (rb.linearVelocity.x > 0)
        {
            sr.flipX = false;
        }
    }

    void AimCamera()
    {
        float newCamX = (Camera.main.ScreenToWorldPoint(Input.mousePosition).x + transform.position.x) / 2f;
        float newCamY = (Camera.main.ScreenToWorldPoint(Input.mousePosition).y + transform.position.y) / 2f;
        cam.transform.position = new Vector3(newCamX, newCamY, -1);
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        Shoot();
        Flip();
        AimCamera();
    }
}
