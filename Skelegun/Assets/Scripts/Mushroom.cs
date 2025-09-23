using UnityEngine;

public class Mushroom : Enemy
{
    public float orbitSpeed = 3f;        // How fast the Mushroom circles around the player
    public float orbitRadius = 3f;       // Distance from the player while orbiting
    public float orbitTime = 3f;         // Time to orbit before charging
    public float chargeSpeed = 10f;      // Speed when charging

    private Transform player;
    private float orbitTimer;
    private bool isCharging = false;
    private Vector3 chargeDirection;

    void Start()
    {
        base.Start(); // Keep Enemy's setup
        player = GameObject.FindGameObjectWithTag("Player").transform;
        orbitTimer = orbitTime;
    }

    void Update()
    {
        base.Update();

        if (player == null) return;

        if (!isCharging)
        {
            // Orbiting logic
            orbitTimer -= Time.deltaTime;
            OrbitAroundPlayer();

            if (orbitTimer <= 0f)
            {
                // Start charging
                isCharging = true;
                chargeDirection = (player.position - transform.position).normalized;
            }
        }
        else
        {
            // Charge straight at the player
            transform.position += chargeDirection * chargeSpeed * Time.deltaTime;
        }
    }

    void OrbitAroundPlayer()
    {
        // Find direction around player
        Vector3 offset = transform.position - player.position;
        offset = Quaternion.Euler(0, orbitSpeed * Time.deltaTime * 50f, 0) * offset;
        offset = offset.normalized * orbitRadius;

        // Maintain orbit around player
        transform.position = player.position + offset;

        // Face the player
        transform.LookAt(player.position);
    }
}

