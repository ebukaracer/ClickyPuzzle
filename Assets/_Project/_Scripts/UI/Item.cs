using UnityEngine;
using UnityEngine.UI;

// Holds an item's properties during playmode.
internal class Item : MonoBehaviour
{
    [Header("ITEM'S PROPERTIES")]
    [SerializeField] private Image itemImage;
    [SerializeField] private Image checkmark;

    public Sprite SetSprite
    {
        get => itemImage.sprite;
        set => itemImage.sprite = value;
    }

    public Image GetCheckmark => checkmark;

    public string Name { get; set; }
    public int StoreID { get; set; }
    public int Price { get; set; }
    public bool HasPurchased { get; set; }
}

