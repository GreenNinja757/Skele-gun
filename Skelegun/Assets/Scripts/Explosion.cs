using UnityEngine;

public class Explosion : MonoBehaviour
{
    public float damage;
    public float radius;
    public float power;

    public SpriteRenderer explosionSprite;

    public void SetStats()
    {

    }

    public void SetSprite(Sprite explosionSprite)
    {
        this.explosionSprite.sprite = explosionSprite;
    }

    void Start()
    {
        Vector2 explosionPos = transform.position;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(explosionPos, radius);
        foreach (Collider2D hit in colliders)
        {
            Debug.Log("Hit: " + hit.name);
            Rigidbody rb = hit.GetComponent<Rigidbody>();

            if (rb != null)
            {
                rb.AddExplosionForce(power, explosionPos, radius, 3.0F);
            }
        }

        Destroy(gameObject, 1f);
    }
}
