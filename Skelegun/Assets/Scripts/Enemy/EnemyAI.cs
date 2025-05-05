using UnityEngine;

public class EnemyAI : MonoBehaviour
{

    [Header("Stats")]
    public int damage;
    public float moveSpeed = 3f;
    public float knockbackStrength = 8f;
    public float chaseDistance = 5f; // Distance at which the enemy starts chasing
    public float knockbackDuration = .3f;
    private Rigidbody2D rb;
    private bool beenPushed = false;


    [Header("PlayerInfo")]
    private Rigidbody2D playerRb;
    public Transform playerTran; 
    private PlayerStats playerStats;
    private PlayerHUD playerHUD;


    
    // Start is called before the first frame update
    void Start()
    {
        playerTran = FindAnyObjectByType<PlayerController>().transform;
        playerRb = playerTran.GetComponent<Rigidbody2D>(); 
        playerStats = playerTran.GetComponent<PlayerStats>();
        playerHUD = playerTran.GetComponent<PlayerHUD>();
        //Get all the PlayerInfo

        rb = GetComponent<Rigidbody2D>(); // Get the enemy's Rigidbody2D

    }

    void FixedUpdate()

    {
        float distToPlayer = Vector2.Distance(transform.position, playerTran.position);
        if (distToPlayer <= chaseDistance)
        {
            // Move towards the player
            Vector2 direction = (playerTran.position - transform.position).normalized;
            rb.linearVelocity = direction * moveSpeed;
        }

        else if (!beenPushed)
        {
            // Stop moving if out of range or after knockback
            rb.linearVelocity = Vector2.zero;
        }

    }




    //If this code looks janky, it really is, getting the Enemies to do a proper knockback was way harder than it should've been for some reason
    void OnCollisionEnter2D(Collision2D coleTrain)
    {
        if (coleTrain.gameObject.CompareTag("Player") && !beenPushed)
        {
            beenPushed = true;
            var playCon = coleTrain.gameObject.GetComponent<PlayerController>();
            if (playCon != null)
            {
                Vector2 pushDirection = (playCon.rb.position - rb.position).normalized;  //Get Direction
                playCon.ApplyKnockback(pushDirection, knockbackStrength, knockbackDuration);
                playCon.TakeDamage(damage);
            }
             
        }
    }

    void OnCollisionExit2D(Collision2D coleTrain)
    {
        if (coleTrain.gameObject.CompareTag("Player"))
        {
            // Allow next knockback once fully separated
            beenPushed = false;
        }

    }
}