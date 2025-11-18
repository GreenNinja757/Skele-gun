using UnityEngine;

public class Golem : Enemy
{
    [Header("Movement")]
    public float chaseDistance = 8f;   // How far Golem will chase
    public float moveSpeed = 2f;

    [Header("Attack")]
    public GameObject projectile;      // Prefab that uses the Projectile script
    public float projectileSpeed = 10f;
    public float fireRate = 70f;      // Seconds between shots
    public float attackRange = 6f;     // Max distance to attack

    private float fireCooldown = 0f;

    private Transform player;
    private Rigidbody2D rb;
    private Rigidbody2D playerRb;

    void Start()
    {
        player = FindAnyObjectByType<PlayerController>().transform;
        rb = GetComponent<Rigidbody2D>();
        playerRb = player.GetComponent<Rigidbody2D>();
    }

    void Moves()
    {
        Vector2 direction;
        if (Vector2.Distance(transform.position, player.position) <= chaseDistance)
            direction = (player.position - transform.position).normalized;
        else
            direction = Vector2.zero;

        rb.linearVelocity = direction * moveSpeed;
    }

    void Attack()
    {
        if (fireCooldown > 0)
        {
            fireCooldown -= Time.deltaTime;
            return;
        }

        float distToPlayer = Vector2.Distance(transform.position, player.position);
        if (distToPlayer > attackRange) return;

        // Predict player movement
        Vector2 playerPos = player.position;
        Vector2 playerVel = playerRb.linearVelocity;
        Vector2 golemPos = transform.position;

        Vector2 displacement = playerPos - golemPos;
        float distance = displacement.magnitude;
        float timeToHit = distance / projectileSpeed;

        Vector2 predictedPos = playerPos + playerVel * timeToHit;
        Vector2 shootDir = (predictedPos - golemPos).normalized;

        // Spawn projectile
        GameObject proj = Instantiate(projectile, transform.position, Quaternion.identity);

        // Rotate it so its "transform.right" points toward the shootDir
        float angle = Mathf.Atan2(shootDir.y, shootDir.x) * Mathf.Rad2Deg;
        proj.transform.rotation = Quaternion.Euler(0, 0, angle);

        // Set its stats (damage=0, speed, size=1)
        Projectile projScript = proj.GetComponent<Projectile>();
        projScript.SetStats(0, projectileSpeed, 1);

        fireCooldown = fireRate;
    }

    void Update()
    {
        Moves();
        Attack();
    }
}




