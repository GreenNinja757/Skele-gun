using UnityEngine;
using TMPro;

public class DamageNumber : MonoBehaviour
{
    public TextMeshProUGUI damageNumber;

    public void SetValue(string type, float damage, bool isCrit)
    {
        damageNumber.text = damage.ToString();

        if (isCrit)
        {
            damageNumber.fontStyle = FontStyles.Bold;
        }

        switch (type)
        {
            case "fire":
                damageNumber.color = Color.red;
                break;

            case "ice":
                damageNumber.color = Color.blue;
                break;

            case "lightning":
                damageNumber.color = Color.yellow;
                break;

            case "doom":
                damageNumber.color = Color.magenta;
                break;
        }
    }

    void Start()
    {
        transform.position = new Vector2(transform.position.x + Random.Range(-.1f, .1f), transform.position.y + Random.Range(-.1f, .1f));
        Destroy(gameObject, .5f);
    }

    void Update()
    {
        transform.position += new Vector3(0, .0025f, 0);
    }
}

