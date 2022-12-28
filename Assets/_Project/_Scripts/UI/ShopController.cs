using Racer.SaveSystem;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Control the activities of the shop-menu like buying and using of items.
/// </summary>
internal class ShopController : MonoBehaviour
{
    // Holds items available for purchase in the store
    private readonly List<Item> _itemContainer = new List<Item>();

    private int _currentItemIndex;

    [Header("ITEM PROPERTIES")]
    [SerializeField] private Item itemPrefab;
    [SerializeField] private Transform parent;

    [Space(5)]
    [SerializeField] private TextMeshProUGUI itemNameT;
    [SerializeField] private TextMeshProUGUI itemPriceT;
    [SerializeField] private Button buyB;


    private void Awake()
    {
        SpawnItems();
    }

    private void Start()
    {
        InitItems();
        UnlockPurchasedItemsOnStart();
        SetCurrentUsingItemOnStart();
    }

    /// <summary>
    /// Spawn and add items to be purchased in a list.
    /// </summary>
    private void SpawnItems()
    {
        var amount = ItemManager.Instance.ItemCount;

        for (int i = 0; i < amount; i++)
        {
            var cloneItem = Instantiate(itemPrefab, parent);

            cloneItem.gameObject.SetActive(false);

            _itemContainer.Add(cloneItem);
        }
    }

    /// <summary>
    /// Initializes the items in the list from the scriptable object properties.
    /// The items spawned matches the scriptable objects created.
    /// After initialization, set the first item in the list active.
    /// </summary>
    private void InitItems()
    {
        for (int i = 0; i < _itemContainer.Count; i++)
        {
            var item = _itemContainer[i];
            var itemDetail = ItemManager.Instance.GetPuzzle(i);

            if (!item) continue;

            item.SetSprite = itemDetail.PreviewImage;
            item.Price = itemDetail.ItemPrice;
            item.StoreID = itemDetail.ItemID;
            item.Name = itemDetail.ImageName;
        }

        itemNameT.text = _itemContainer[_currentItemIndex].Name;
        itemPriceT.text = $"${_itemContainer[_currentItemIndex].Price}";

        _itemContainer[0].gameObject.SetActive(true);
    }

    /// <summary>
    /// Moves to the next item.
    /// This is assigned to the forward button in the shop panel.
    /// </summary>
    public void ForwardArrow()
    {
        if (_currentItemIndex >= _itemContainer.Count - 1)
            return;

        DisableItem(_currentItemIndex);

        _currentItemIndex++;

        EnableItem(_currentItemIndex);
    }

    /// <summary>
    /// Moves to the previous item.
    /// This is assigned to the backward button in the shop-panel.
    /// </summary>
    public void BackwardArrow()
    {
        if (_currentItemIndex <= 0)
            return;

        DisableItem(_currentItemIndex);

        _currentItemIndex--;

        EnableItem(_currentItemIndex);
    }

    /// <summary>
    /// Resets the item's list as user departs from shop.
    /// </summary>
    public void ResetItemOrder()
    {
        if (_currentItemIndex == 0) return;

        DisableItem(_currentItemIndex);
        EnableItem(_currentItemIndex = 0);
    }

    /// <summary>
    /// Sets the next item in the list active.
    /// </summary>
    private void EnableItem(int index)
    {
        SetCurrentItemName(index);
        CheckHasPurchased(index);

        _itemContainer[index].gameObject.SetActive(true);
    }

    /// <summary>
    /// Displays current item's name. 
    /// </summary>
    private void SetCurrentItemName(int index)
    {
        itemNameT.text = _itemContainer[index].Name;
    }

    /// <summary>
    /// Sets the previous item in the list active.
    /// </summary>
    private void DisableItem(int index)
    {
        _itemContainer[index].gameObject.SetActive(false);
    }

    /// <summary>
    /// Initializes current item with appropriate properties if purchased.
    /// </summary>
    private void CheckHasPurchased(int index)
    {
        if (_itemContainer[index].HasPurchased)
            InitPurchasedItem(t: Metrics.PurchasedText, isPurchased: true);
        else
            InitPurchasedItem(t: $"${_itemContainer[index].Price}", isPurchased: false);
    }

    /// <summary>
    /// Modifies the current purchased item's properties(its text and button).
    /// </summary>
    private void InitPurchasedItem(string t, bool isPurchased)
    {
        if (itemPriceT.text != t)
            itemPriceT.text = t;

        buyB.interactable = !isPurchased;
    }

    /// <summary>
    /// Purchases the current item player if player has enough cash.
    /// Saves the purchased item via its store-id.
    /// Updates the player's wallet amount.
    /// </summary>
    private void BuyItem(Item item)
    {
        var uiController = UIControllerMain.Instance;

        if (uiController.TotalCash < item.Price)
            return;

        item.HasPurchased = true;

        CheckHasPurchased(_currentItemIndex);

        uiController.TotalCash -= item.Price;
        uiController.SyncCash();

        SaveSystem.SaveData(Metrics.Item + _currentItemIndex, item.StoreID);
    }

    /// <summary>
    /// Unlocks the current item if not already purchased.
    /// This is assigned to the 'buy button' on the shop-panel.
    /// </summary>
    public void UnlockItem()
    {
        var item = _itemContainer[_currentItemIndex];

        if (!item.HasPurchased)
        {
            BuyItem(item);
        }
    }

    /// <summary>
    /// Uses the current purchased item.
    /// This is assigned to the 'use button' on the shop-panel.
    /// Saves the current item being used.
    /// </summary>
    public void UseItem()
    {
        var item = _itemContainer[_currentItemIndex];

        if (!item.HasPurchased)
            return;

        SetCurrentUsingItemCheckmark(item);

        _itemContainer[_currentItemIndex].GetCheckmark.enabled = true;

        SaveSystem.SaveData(Metrics.CurrentItemInUse, item.StoreID);
    }

    /// <summary>
    /// Loads a list of purchased item from the save manager.
    /// Initializes the items to their appropriate properties.
    /// </summary>
    private void UnlockPurchasedItemsOnStart()
    {
        for (int i = 0; i < _itemContainer.Count; i++)
        {
            var itemId = SaveSystem.GetData<int>(Metrics.Item + i);

            if (_itemContainer[i].StoreID != itemId) continue;

            _itemContainer[i].HasPurchased = true;

            CheckHasPurchased(_currentItemIndex);
        }
    }

    /// <summary>
    /// Adds a check-mark to indicate the item currently being used.
    /// </summary>
    /// <param name="currentItem">Current item</param>
    private void SetCurrentUsingItemCheckmark(Item currentItem)
    {
        currentItem.GetCheckmark.enabled = true;

        for (int i = 0; i < _itemContainer.Count; i++)
        {
            if (_itemContainer[i] == currentItem)
                continue;

            if (_itemContainer[i].GetCheckmark.enabled)
                _itemContainer[i].GetCheckmark.enabled = false;
        }
    }

    /// <summary>
    /// Adds a check-mark to indicate the item currently being used.
    /// </summary>
    private void SetCurrentUsingItemOnStart()
    {
        var itemId = SaveSystem.GetData<int>(Metrics.CurrentItemInUse);

        for (int i = 0; i < _itemContainer.Count; i++)
        {
            if (_itemContainer[i].StoreID != itemId)
            {
                if (_itemContainer[i].GetCheckmark.enabled)
                    _itemContainer[i].GetCheckmark.enabled = false;
            }
            else
                _itemContainer[i].GetCheckmark.enabled = true;
        }
    }
}