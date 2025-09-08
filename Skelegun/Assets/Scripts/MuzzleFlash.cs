using UnityEngine;

public class MuzzleFlash : MonoBehaviour
{
    public SpriteRenderer muzzleFlashSprite;

    public void SetSprite(Sprite bulletImpactSprite)
    {
        this.muzzleFlashSprite.sprite = bulletImpactSprite;
    }

    void Start()
    {
        Destroy(gameObject, .1f);
    }
}
