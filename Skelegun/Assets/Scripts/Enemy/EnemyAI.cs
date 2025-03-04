using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public float moveSpeed = 3f;
    public Transform player; // Reference to the player's transform
    public float chaseDistance = 5f; // Distance at which the enemy starts chasing
    public float pushbackForce = 5f; // Force applied to the player when hit

    private Rigidbody2D rb;
    private Rigidbody2D playerRb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>(); // Get the enemy's Rigidbody2D
        playerRb = player.GetComponent<Rigidbody2D>(); // Get the player's Rigidbody2D
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector2.Distance(transform.position, player.position) <= chaseDistance)
        {
            // Move towards the player
            Vector2 direction = (player.position - transform.position).normalized;
            rb.linearVelocity = direction * moveSpeed; // Apply velocity to move
        }
        else
        {
            rb.linearVelocity = Vector2.zero; // Stop moving when out of chase range
        }
    }

    // When enemy hits the player, apply pushback
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Apply a pushback force to the player
            Vector2 pushDirection = collision.transform.position - transform.position;
            playerRb.AddForce(pushDirection.normalized * pushbackForce, ForceMode2D.Impulse);
        }
    }
}