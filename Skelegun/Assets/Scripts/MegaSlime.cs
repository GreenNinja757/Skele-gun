using UnityEngine;

public class MegaSlime : Enemy
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = FindAnyObjectByType<PlayerController>().transform;
        rb = GetComponent<Rigidbody2D>(); // Get the enemy's Rigidbody2D
        playerRb = player.GetComponent<Rigidbody2D>(); // Get the player's Rigidbody2D
    }

    void Moves()
    {
        Vector2 direction;
        if (Vector2.Distance(transform.position, player.position) <= chaseDistance)
        {
            // Move towards the player
            direction = (player.position - transform.position).normalized;
        }
        else
        {
            direction = Vector2.zero;
        }

        rb.linearVelocity = direction * moveSpeed; // Apply velocity to move
    }

    // Update is called once per frame
    void Update()
    {
        Moves();
        //Attack();
    }
}
