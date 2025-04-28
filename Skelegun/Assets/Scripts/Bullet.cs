using UnityEngine;

public class Projectile : MonoBehaviour
{
    public Rigidbody2D rb;

    public int damage;
    public float speed;
    public int size;

    public SpriteRenderer sprite;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Destroy(gameObject, 5);
    }

    // Update is called once per frame
    void Update()
    {
        rb.linearVelocity = transform.right * speed;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy")) // Use CompareTag for better performance
        {
            // Get the enemy health component
            EnemyHP enemyHealth = collision.gameObject.GetComponent<EnemyHP>(); // Fix the error

            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage); // Deal damage
            }

            Destroy(gameObject); // Destroy the projectile after collision
        }
        else if (!collision.gameObject.CompareTag("Player"))
        {
            Destroy(gameObject); // Destroy the projectile on other collisions (e.g., environment)
        }
    }

    private void OnBecameInvisible()
    {
        if (gameObject != null)
        {
            Destroy(gameObject);
        }
    }
}
