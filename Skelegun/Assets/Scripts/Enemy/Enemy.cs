using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    public int damage;
    public float moveSpeed;
    public float chaseDistance;
    public float pushbackForce;
    public float pushbackStunTime;
    public Animator animator;

    protected Transform player;
    protected Rigidbody2D rb;
    protected Rigidbody2D playerRb;

    // Start is called before the first frame update
    void Start()
    {
        player = FindAnyObjectByType<PlayerController>().transform;
        rb = GetComponent<Rigidbody2D>(); // Get the enemy's Rigidbody2D
        playerRb = player.GetComponent<Rigidbody2D>(); // Get the player's Rigidbody2D
    }


    public void Move()
    {
        // Move towards the player
        Vector2 direction = (player.position - transform.position).normalized;
        rb.linearVelocity = direction * moveSpeed; // Apply velocity to move
    }


    public void FlipSprite()
    {
        Camera cam = FindAnyObjectByType<Camera>();
        var player = FindAnyObjectByType<PlayerController>();

        if (player.transform.position.x < transform.position.x)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
    }


    public void Animate()
    {
        if (rb.linearVelocity != Vector2.zero)
        {
            animator.Play("Move");
        }
        else
        {
            animator.Play("Idle");
        }
    }

    // Update is called once per frame
    void Update()
    {
        Animate();
        FlipSprite();
        ChildUpdate();
    }

    protected virtual void ChildUpdate() { }



    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {

            player.GetComponent<PlayerStats>().TakeDamage(damage, transform.position, pushbackForce, pushbackStunTime);
            player.GetComponent<PlayerHUD>().UpdateHUD();

            /*
            //Apply a pushback force to the player
            Vector2 pushDirection = collision.transform.position - transform.position;
            playerRb.AddForce(pushDirection.normalized * pushbackForce, ForceMode2D.Impulse);

            //Deal damage to the player
            player.GetComponent<PlayerStats>().TakeDamage(damage);
            player.GetComponent<PlayerHUD>().UpdateHUD();
            */
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {

            player.GetComponent<PlayerStats>().TakeDamage(damage, transform.position, pushbackForce, pushbackStunTime);
            player.GetComponent<PlayerHUD>().UpdateHUD();

            /*
            //Apply a pushback force to the player
            Vector2 pushDirection = collision.transform.position - transform.position;
            playerRb.AddForce(pushDirection.normalized * pushbackForce, ForceMode2D.Impulse);
            //Deal damage to the player
            player.GetComponent<PlayerStats>().TakeDamage(damage);
            player.GetComponent<PlayerHUD>().UpdateHUD();
            */

        }

    }
}