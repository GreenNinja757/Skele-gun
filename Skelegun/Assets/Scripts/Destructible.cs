using System.Collections.Generic;
using UnityEngine;

public class Destructible : MonoBehaviour
{
    public List<Pickup> pickups;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Projectile"))
        {
            if (Random.Range(0, 2) == 1)
            {
                Instantiate(pickups[Random.Range(0, pickups.Count)], transform.position, transform.rotation);
            }
            Destroy(gameObject);
        }
    }
}
