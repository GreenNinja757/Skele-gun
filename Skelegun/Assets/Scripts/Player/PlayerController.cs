using UnityEditor.SpeedTree.Importer;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public PlayerInput playerInput;

    public Transform weaponRotationPoint;
    public Transform bulletSpawnPoint;
    public GameObject bulletPrefab;

    public Rigidbody2D rb;
    public float moveSpeed;
    public float bulletSpeed;

    public SpriteRenderer playerSprite;
    public SpriteRenderer gunSprite;

    public Camera cam;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
         
    }

    void Move()
    {
        rb.linearVelocity = playerInput.actions["Move"].ReadValue<Vector2>() * moveSpeed;
    }
    
    void Shoot()
    {
        Vector3 diff = Camera.main.ScreenToWorldPoint(Input.mousePosition) - bulletSpawnPoint.transform.position;
        diff.Normalize();
        float rotZ = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
        weaponRotationPoint.rotation = Quaternion.Euler(0f, 0f, rotZ);

        if (playerInput.actions["Shoot"].WasPressedThisFrame())
        {
            Instantiate(bulletPrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
        }
    }

    void FlipSprites()
    {
        if (rb.linearVelocity.x < 0)
        {
            playerSprite.flipX = true;
        } else if (rb.linearVelocity.x > 0)
        {
            playerSprite.flipX = false;
        }

        if (Camera.main.ScreenToWorldPoint(Input.mousePosition).x < transform.position.x)
        {
            gunSprite.flipY = true;
        } else if (Camera.main.ScreenToWorldPoint(Input.mousePosition).x > transform.position.x)
        {
            gunSprite.flipY = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        Shoot();
        FlipSprites();
    }
}
