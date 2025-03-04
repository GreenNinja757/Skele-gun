<<<<<<< Updated upstream
<<<<<<< Updated upstream
<<<<<<< Updated upstream
<<<<<<< Updated upstream
=======
=======
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes
<<<<<<< HEAD
#if UNITY_EDITOR
using UnityEditor.SpeedTree.Importer;  
using UnityEditor.SpeedTree.Importer;
#endif

using UnityEngine.Tilemaps;
=======
>>>>>>> 9b0d114037be07104583a503d745c7e4a2e4e37b
<<<<<<< Updated upstream
<<<<<<< Updated upstream
<<<<<<< Updated upstream
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes
using UnityEngine;
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

    void Move()
    {
<<<<<<< Updated upstream
<<<<<<< Updated upstream
<<<<<<< Updated upstream
<<<<<<< Updated upstream
        rb.linearVelocity = playerInput.actions["Move"].ReadValue<Vector2>() * moveSpeed;
=======
=======
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes
<<<<<<< HEAD
        horMov = playerInput.actions["Move"].ReadValue<Vector2>() * moveSpeed;
        verMov = playerInput.actions["Move"].ReadValue<Vector2>() * moveSpeed;
        rb.linearVelocityX = horMov.x;
        rb.linearVelocityY = horMov.y;
=======
        rb.linearVelocity = playerInput.actions["Move"].ReadValue<Vector2>() * moveSpeed;
>>>>>>> 9b0d114037be07104583a503d745c7e4a2e4e37b
<<<<<<< Updated upstream
<<<<<<< Updated upstream
<<<<<<< Updated upstream
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes
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
