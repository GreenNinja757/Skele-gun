using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public PlayerInput input;
    public PlayerStats stats;
    public PlayerInventory inventory;
    public PlayerHUD hud;
    public Rigidbody2D rb;
    public SpriteRenderer sr;
    public Animator animator;

    public Transform weaponRotationPoint;
    public Transform weaponSpawnPoint;
    public List<Transform> bulletSpawnPoints;


    [Header("Knockback")]

    private bool isKnockedBack = false;
    private float knockbackTimer = 0f;
    private Vector2 knockbackVelocity;
    public bool IsKnockedBack => isKnockedBack;

    public Animator animator;

    public Camera cam;

    public bool isHeld;
    public bool isAlive;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        
    }

    //This moves the player
    void Move()
    {
        rb.linearVelocity = input.actions["Move"].ReadValue<Vector2>() * stats.moveSpeed;
    }

    //The aims the gun at the cursor
    void Aim()
    {
        Vector3 mousePos = (Vector2)cam.ScreenToWorldPoint(Input.mousePosition);
        float angleRad = Mathf.Atan2(mousePos.y - transform.position.y, mousePos.x - transform.position.x);
        float angleDeg = (180 / Mathf.PI) * angleRad;

        weaponRotationPoint.rotation = Quaternion.Euler(0f, 0f, angleDeg);
    }
    
    //This makes the equipped weapon shoot
    void Shoot()
    {
        if (inventory.equippedWeapon != null)
        {
            if (input.actions["Shoot"].WasPressedThisFrame())
            {
                inventory.equippedWeapon.Shoot(bulletSpawnPoints);
                isHeld = true;
            } 
            else if (isHeld && inventory.equippedWeapon.currentAmmo != 0)
            {
                if (inventory.equippedWeapon.isAuto)
                {
                    inventory.equippedWeapon.Shoot(bulletSpawnPoints);
                }
            }
            
            if (input.actions["Shoot"].WasReleasedThisFrame())
            {
                isHeld = false;
            }
        }
        hud.UpdateHUD();
    }

    //This switches the equipped weapon from one to the other
    void SwitchWeapon()
    {
        if (input.actions["SwitchWeapon"].ReadValue<float>() > 0f)
        {
            inventory.PreviousWeapon();
        } 
        else if (input.actions["SwitchWeapon"].ReadValue<float>() < 0f)
        {
            inventory.NextWeapon();
        }
    }

    //This flips the player and weapon sprites depending on movement and cursor position
    void FlipSprite()
    {
        if (cam.ScreenToWorldPoint(Input.mousePosition).x < transform.position.x)
        {
            sr.flipX = true;
        }
        else
        {
            sr.flipX = false;
        }

        if (cam.ScreenToWorldPoint(Input.mousePosition).x < transform.position.x)
        {
            weaponRotationPoint.transform.localScale = new Vector3(1, -1, 1);
        }
        else
        {
            weaponRotationPoint.transform.localScale = new Vector3(1, 1, 1);
        }
    }

    //This animates the sprite of player
    void Animate()
    {
        if (rb.linearVelocity != Vector2.zero)
        {
            animator.Play("Walk");
        }
        else
        {
            animator.Play("Idle");
        }
    }

    void Update()
    {
        if (isAlive)
        {
            if (isKnockedBack)
            {
                knockbackTimer -= Time.deltaTime;
                if (knockbackTimer <= 0f)
                {
                    isKnockedBack = false;
                }
                else
                {
                    rb.MovePosition(rb.position + knockbackVelocity * Time.deltaTime);
                    return; // Skip normal input while knocked back
                }
            }

            Move();
            Aim();
            Shoot();
            SwitchWeapon();
            FlipSprite();
            Animate();
        }
    }

    public void ApplyKnockback(Vector2 sourcePosition, float strength, float duration)
    {
        if (isKnockedBack) return; // Ignore if already knocked back

        isKnockedBack = true;
        knockbackTimer = duration;

        Vector2 direction = (transform.position - (Vector3)sourcePosition).normalized;
        knockbackVelocity = direction * strength;
    }







}
