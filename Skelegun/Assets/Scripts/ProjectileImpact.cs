using UnityEngine;

public class BulletImpact : MonoBehaviour
{
    public SpriteRenderer bulletImpactSprite;

    public void SetSprite(Sprite bulletImpactSprite)
    {
        this.bulletImpactSprite.sprite = bulletImpactSprite;
    }

    void Start()
    {
        Destroy(gameObject, .1f);
    }
}
