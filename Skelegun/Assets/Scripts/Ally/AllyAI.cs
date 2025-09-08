using UnityEngine;

public class AllyAI : MonoBehaviour
{
    public int damage;
    public float moveSpeed;
    public Transform player;
    public float chaseDistance;
    public float pushbackForce;

    private Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        player = FindAnyObjectByType<PlayerController>().transform;
        rb = GetComponent<Rigidbody2D>();
    }

    void Move()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        rb.linearVelocity = direction * moveSpeed;
    }

    void FlipSprite()
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

    // Update is called once per frame
    void Update()
    {
        if (Vector2.Distance(transform.position, player.position) <= chaseDistance)
        {
            Move();
        } 
        else
        {
            rb.linearVelocity = Vector2.zero;
        }

        FlipSprite();
    }

    // When ally hits the player, apply pushback
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            
        }
    }
}