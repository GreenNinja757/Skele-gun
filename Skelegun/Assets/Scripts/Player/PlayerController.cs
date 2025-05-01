using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Composites;

public class PlayerController : MonoBehaviour
{
    public PlayerInput playerInput;
    public PlayerStats stats;
    public PlayerInventory inventory;
    public PlayerHUD hud;

    public Rigidbody2D rb;
    public SpriteRenderer sprite;

    public Transform weaponRotationPoint;
    public Transform weaponSpawnPoint;
    public Transform bulletSpawnPoint;

    public Animator animator;

    public AudioManager audio;

    public Camera cam;

    public bool isHeld;

    //This moves the player
    void Move()
    {
        rb.linearVelocity = playerInput.actions["Move"].ReadValue<Vector2>() * stats.moveSpeed;
    }

    //The aims the gun at the cursor
    void Aim()
    {
        Vector3 mousePos = (Vector2)cam.ScreenToWorldPoint(Input.mousePosition);
        float angleRad = Mathf.Atan2(mousePos.y - transform.position.y, mousePos.x - transform.position.x);
        float angleDeg = (180 / Mathf.PI) * angleRad;

        weaponRotationPoint.rotation = Quaternion.Euler(0f, 0f, angleDeg);
    }
    
    //This makes the equipped weapon fire
    void Shoot()
    {
        if (inventory.equippedWeapon != null)
        {
            if (playerInput.actions["Shoot"].WasPressedThisFrame())
            {
                inventory.equippedWeapon.Shoot();
                isHeld = true;
                if (inventory.equippedWeapon.canShoot)
                {
                    audio.playShootSound();
                }
            } 
            else if (isHeld)
            {
                if (inventory.equippedWeapon.isAuto)
                {
                    inventory.equippedWeapon.Shoot();
                    if (inventory.equippedWeapon.canShoot)
                    {
                        audio.playShootSound();
                    }
                }
            }
            
            if (playerInput.actions["Shoot"].WasReleasedThisFrame())
            {
                isHeld = false;
            }
        }
    }

    //This switches the equipped weapon from one to the other
    void SwitchWeapon()
    {
        if (playerInput.actions["SwitchWeapon"].ReadValue<float>() > 0f)
        {
            inventory.PreviousWeapon();
        } 
        else if (playerInput.actions["SwitchWeapon"].ReadValue<float>() < 0f)
        {
            inventory.NextWeapon();
        }
    }

    //This flips the player and weapon sprites depending on movement and cursor position
    void OrientSprites()
    {
        if (rb.linearVelocity.x < 0)
        {
            sprite.flipX = true;
        } 
        else if (rb.linearVelocity.x > 0)
        {
            sprite.flipX = false;
        }

        if (inventory.equippedWeapon != null)
        {
            if (cam.ScreenToWorldPoint(Input.mousePosition).x < transform.position.x)
            {
                inventory.equippedWeapon.weaponSprite.flipY = true;
            }
            else
            {
                inventory.equippedWeapon.weaponSprite.flipY = false;
            }

            if (cam.ScreenToWorldPoint(Input.mousePosition).y < transform.position.y)
            {
                inventory.equippedWeapon.weaponSprite.sortingOrder = 2;
            }
            else
            {
                inventory.equippedWeapon.weaponSprite.sortingOrder = 0;
            }
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

    public void Heal(int healing)
    {
        stats.currentHealth += healing;

        if (stats.currentHealth >= stats.maxHealth)
        {
            stats.currentHealth = stats.maxHealth;
        }

        hud.UpdateHUD();
    }

    public void TakeDamage(int damage)
    {
        stats.currentHealth -= damage;

        if (stats.currentHealth <= stats.maxHealth)
        {
            stats.currentHealth = stats.maxHealth;
        }

        hud.UpdateHUD();
    }

    void Update()
    {
        Move();
        Aim();
        Shoot();
        SwitchWeapon();
        OrientSprites();
        Animate();
    }
}
