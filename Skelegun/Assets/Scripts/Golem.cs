using UnityEngine;

public class Golem : Enemy
{
    [Header("Movement")]
    public float chaseDistance = 8f;   // How far Golem will chase
    public float moveSpeed = 2f;

    [Header("Attack")]
    public GameObject projectile;
    public float projectileSpeed = 10f;
    public float fireRate = 1.5f;      // Seconds between shots
    public float attackRange = 6f;     // Max distance to attack

    private float fireCooldown = 0f;

    private Transform player;
    private Rigidbody2D rb;
    private Rigidbody2D playerRb;

    void Start()
    {
        player = FindAnyObjectByType<PlayerController>().transform;
        rb = GetComponent<Rigidbody2D>(); // Enemy rigidbody
        playerRb = player.GetComponent<Rigidbody2D>(); // Player rigidbody
    }

    void Moves()
    {
        Vector2 direction;
        if (Vector2.Distance(transform.position, player.position) <= chaseDistance)
        {
            // Moves towards the player
            direction = (player.position - transform.position).normalized;
        }
        else
        {
            direction = Vector2.zero;
        }

        rb.linearVelocity = direction * moveSpeed;
    }

    void Attack()
    {
        if (fireCooldown > 0)
        {
            fireCooldown -= Time.deltaTime;
            return;
        }

        // Only shoot if within attack range
        float distToPlayer = Vector2.Distance(transform.position, player.position);
        if (distToPlayer > attackRange) return;

        // Predicts player movement
        Vector2 playerPos = player.position;
        Vector2 playerVel = playerRb.linearVelocity;
        Vector2 golemPos = transform.position;

        Vector2 displacement = playerPos - golemPos;
        float distance = displacement.magnitude;
        float timeToHit = distance / projectileSpeed;

        Vector2 predictedPos = playerPos + playerVel * timeToHit;

        // Direction to shoot
        Vector2 shootDir = (predictedPos - golemPos).normalized;

        // Spawns projectiles
        GameObject proj = Instantiate(projectile, transform.position, Quaternion.identity);
        Rigidbody2D projRb = proj.GetComponent<Rigidbody2D>();
        projRb.linearVelocity = shootDir * projectileSpeed;

        fireCooldown = fireRate;
    }

    void Update()
    {
        Moves();
        Attack();
    }
}


