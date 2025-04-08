using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public PlayerInput playerInput;
    public PlayerStats stats;
    public PlayerInventory inventory;

    public Rigidbody2D rb;
    public SpriteRenderer sprite;

    public Camera cam;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
         
    }

    void Move()
    {
        rb.linearVelocity = playerInput.actions["Move"].ReadValue<Vector2>() * stats.moveSpeed;
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
}
