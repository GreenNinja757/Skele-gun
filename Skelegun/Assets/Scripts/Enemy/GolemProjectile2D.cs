using UnityEngine;
using UnityEngine.Rendering.Universal;

public class GolemProjectile2D : MonoBehaviour
{
    [Header("Projectile Settings")]
    public Rigidbody2D rb;
    public float lifeTime = 5f;
    public float speed = 10f;

    [Header("Visuals & Audio")]
    public GameObject bulletImpactPrefab;
    public SpriteRenderer bulletSprite;
    public Sprite bulletImpactSprite;
    public Light2D light2D;
    public AudioClip impactSFX;

    private Vector2 moveDirection;

    public void SetStats(float damage, float speed, float size)
    {
        this.speed = speed;
        transform.localScale *= size;

        if (light2D != null)
            light2D.pointLightOuterRadius *= size;
    }

    public void SetDirection(Vector2 direction)
    {
        moveDirection = direction.normalized;
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();

        rb.linearVelocity = moveDirection * speed;
        Debug.Log($"Projectile launched with direction {moveDirection}, speed {speed}");
    }

    private void Start()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();

        Destroy(gameObject, lifeTime);
    }

    private void Update()
    {
        if (rb != null)
            rb.linearVelocity = moveDirection * speed;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Enemy"))
        {
            if (bulletImpactPrefab != null)
            {
                var bulletImpact = Instantiate(bulletImpactPrefab, transform.position, Quaternion.identity);
                if (bulletImpactSprite != null)
                    bulletImpact.GetComponent<SpriteRenderer>().sprite = bulletImpactSprite;
            }

            if (impactSFX != null)
            {
                var audioManager = FindAnyObjectByType<AudioManager>();
                audioManager?.PlaySFX(impactSFX);
            }

            Destroy(gameObject);
        }
    }
}
