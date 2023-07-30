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
    public string Description { get; set; }
    public bool IsUnlocked { get; set; }
    public bool IsMatched { get; set; }
}

