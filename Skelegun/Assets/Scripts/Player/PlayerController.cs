#if UNITY_EDITOR
using UnityEditor.SpeedTree.Importer;  
using UnityEditor.SpeedTree.Importer;
#endif

using UnityEngine.Tilemaps;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public PlayerInput playerInput;
    public PlayerStats stats;
    public PlayerInventory inventory;
    public Rigidbody2D rb;
    public SpriteRenderer sprite;
    public Transform weaponRotationPoint;
    public Transform bulletSpawnPoint;
    public GameObject bulletPrefab;
    public Rigidbody2D rb;
    public float moveSpeed;
    public float bulletSpeed;
    public Vector2 horMov;
    public Vector2 verMov;
    public SpriteRenderer playerSprite;
    public SpriteRenderer gunSprite;
    public Camera cam;

    void Move()
    {
        rb.linearVelocity = playerInput.actions["Move"].ReadValue<Vector2>() * stats.moveSpeed;
        horMov = playerInput.actions["Move"].ReadValue<Vector2>() * moveSpeed;
        verMov = playerInput.actions["Move"].ReadValue<Vector2>() * moveSpeed;
        rb.linearVelocityX = horMov.x;
        rb.linearVelocityY = horMov.y;
    }
    
    void Shoot()
    {
        //inventory.equippedWeapon.Shoot();
    }

    void SwitchWeapon()
    {

    }

    void FlipSprite()
    {
        if (rb.linearVelocity.x < 0)
        {
            sprite.flipX = true;
        } 
        else if (rb.linearVelocity.x > 0)
        {
            sprite.flipX = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        Shoot();
        SwitchWeapon();
        FlipSprite();
    }

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}
