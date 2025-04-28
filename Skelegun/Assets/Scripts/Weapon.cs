using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    public Camera cam;

    public int maxAmmo;
    public int currentAmmo;
    public int fireRate;
    public int projectileCount;
    public float accuracy;
    public bool isPickup;

    public SpriteRenderer sr;

    public PlayerController player;

    public GameObject bullet;

    private void Start()
    {
        player = FindAnyObjectByType<PlayerController>();
        cam = FindAnyObjectByType<Camera>();
    }

    public abstract void Shoot();

    public void FlipSprite()
    {
        if (!isPickup)
        {
            Vector3 mousePos = (Vector2)cam.ScreenToWorldPoint(Input.mousePosition);

            if (mousePos.x < player.transform.position.x)
            {
                sr.flipY = true;
            }
            else if (mousePos.x > player.transform.position.x)
            {
                sr.flipY = false;
            }
        }
    }

    private void Update()
    {
        FlipSprite();
    }
}
