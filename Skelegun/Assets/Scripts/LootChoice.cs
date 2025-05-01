using Unity.VisualScripting;
using UnityEngine;

public class LootChoice : MonoBehaviour
{
    public GameObject choice1;
    public GameObject choice2;
    public GameObject triggers;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Destroy(choice1);
            Destroy(choice2);
            Destroy(triggers);
        }
    }
}
