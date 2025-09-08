using UnityEngine;

public abstract class Item : MonoBehaviour
{
    public SpriteRenderer itemSprite;
    public string nameText;
    public string flavorText;

    private void Start()
    {
        itemSprite = GetComponent<SpriteRenderer>();
    }
}
