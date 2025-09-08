using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Projectile : MonoBehaviour
{
    public Rigidbody2D rb;

    public GameObject bulletImpactPrefab;

    public Light2D light2D;
    public SpriteRenderer bulletSprite;
    public Sprite bulletImpactSprite;

    public AudioClip impactSFX;

    private float damage;
    private float speed;
    private float size;
    private bool isCrit;
    //private int bounces;
    //private int pierces;

    public void SetStats(float damage, float speed, float size)
    {
        this.damage = damage;
        this.speed = speed;
        this.size = size;
        transform.localScale *= this.size;
        light2D.pointLightOuterRadius *= size;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy")) // Use CompareTag for better performance
        {
            var player = FindAnyObjectByType<PlayerInventory>();
            for (int i = 0; i < player.itemInventory.Count; i++)
            {
                player.itemInventory[i].GetComponent<IEnemyHitEffect>()?.ApplyEnemyHitEffect(collision.gameObject);
            }

            // Get the enemy health component
            EnemyStats enemyHealth = collision.gameObject.GetComponent<EnemyStats>();

            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage("", damage, isCrit); // Deal damage
            }

            var bulletImpact = Instantiate(bulletImpactPrefab, transform.position, Quaternion.identity);
            bulletImpact.GetComponent<BulletImpact>().SetSprite(bulletImpactSprite);

            Destroy(gameObject); // Destroy the projectile after collision
        }
        else
        {
            var bulletImpact = Instantiate(bulletImpactPrefab, transform.position, Quaternion.identity);
            bulletImpact.GetComponent<BulletImpact>().SetSprite(bulletImpactSprite);

            Destroy(gameObject); // Destroy the projectile on other collisions (e.g., environment)
        }

        var audioManager = FindAnyObjectByType<AudioManager>();
        audioManager.PlaySFX(impactSFX);
    }

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
}
