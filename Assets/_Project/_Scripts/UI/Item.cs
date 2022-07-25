using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Item displayed on the shop-menu
/// </summary>
public class Item : MonoBehaviour
{
    // Item's Image component 
    [SerializeField]
    Image itemImage;

    [SerializeField]
    Image usingCheckmark;

    //Item properties get initialized in (ShopController)
    public Sprite SetSprite
    {
        get => itemImage.sprite;

        set => itemImage.sprite = value;
    }

    public Image GetUsingCheckmark => usingCheckmark;

    public int StoreID { get; set; }

    public int Price { get; set; }

    public bool HasPurchased { get; set; }
}

