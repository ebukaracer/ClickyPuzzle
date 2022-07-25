using Racer.SaveSystem;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Control the activities of the shop-menu.
/// </summary>
public class ShopController : MonoBehaviour
{
    // Holds items available for purchase in the store
    readonly List<Item> itemContainer = new List<Item>();

    readonly string bought = "PURCHASED";

    int currentItemIndex;

    //Used
    [SerializeField]
    GameObject itemPrefab;

    [SerializeField]
    Transform parent;

    [Space(15)]

    [SerializeField]
    TextMeshProUGUI itemPriceT;

    [SerializeField]
    Button buyB;


    void Awake()
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
    void SpawnItems()
    {
        var amount = ItemManager.Instance.GetItemCount;

        for (int i = 0; i < amount; i++)
        {
            var cloneItem = Instantiate(itemPrefab, parent);

            cloneItem.SetActive(false);

            itemContainer.Add(cloneItem.GetComponent<Item>());
        }
    }


    /// <summary>
    /// Initializes the items in the list from the scriptable object properties.
    /// The items spawned matches the scriptable objects created.
    /// After initialization, set the first item in the list active.
    /// </summary>
    void InitItems()
    {
        for (int i = 0; i < itemContainer.Count; i++)
        {
            var item = itemContainer[i];

            var itemDetail = ItemManager.Instance.GetPuzzle(i);

            if (item != null)
            {
                item.SetSprite = itemDetail.PreviewImage;

                item.Price = itemDetail.GetItemPrice;

                item.StoreID = itemDetail.GetIndexID;
            }
        }

        itemPriceT.text = "$" + itemContainer[currentItemIndex].Price.ToString();

        itemContainer[0].gameObject.SetActive(true);
    }

    /// <summary>
    /// Moves to the next item.
    /// This is assigned to the forward button in the shop panel.
    /// </summary>
    public void SetItemActiveFoward()
    {
        if (currentItemIndex >= itemContainer.Count - 1)
            return;

        DisableItem(currentItemIndex);

        currentItemIndex++;

        EnableItem(currentItemIndex);
    }

    /// <summary>
    /// Moves to the previous item.
    /// This is assigned to the backward button in the shop-panel.
    /// </summary>
    public void SetItemActiveBackward()
    {
        if (currentItemIndex <= 0)
            return;

        DisableItem(currentItemIndex);

        currentItemIndex--;

        EnableItem(currentItemIndex);
    }

    /// <summary>
    /// Sets the next item in the list active.
    /// </summary>
    private void EnableItem(int index)
    {
        CheckHasPurchased(index);

        itemContainer[index].gameObject.SetActive(true);
    }

    /// <summary>
    /// Sets the previous item in the list active.
    /// </summary>
    void DisableItem(int index)
    {
        itemContainer[index].gameObject.SetActive(false);
    }

    /// <summary>
    /// Initializes current item with appropriate properties if purchased.
    /// </summary>
    private void CheckHasPurchased(int index)
    {
        if (itemContainer[index].HasPurchased)
            InitPurchasedItem(t: bought, isPurchased: true);
        else
            InitPurchasedItem(t: "$" + itemContainer[index].Price.ToString(), isPurchased: false);
    }


    /// <summary>
    /// Modifies the current purchased item's properties(its text and button).
    /// </summary>
    /// <param name="t">current item price text</param>
    /// <param name="isPurchased">has the current item been purchased?</param>
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
    void BuyItem(Item item)
    {
        var uiController = UIControllerMain.Instance;

        //Buy logic
        if (uiController.TotalCash < item.Price)
            return;

        item.HasPurchased = true;

        CheckHasPurchased(currentItemIndex);

        uiController.TotalCash -= item.Price;

        uiController.SyncCash();

        //SaveManager.Instance.Save($"Item_{currentItemIndex}", item.StoreID);
        SaveSystem.SaveData($"Item_{currentItemIndex}", item.StoreID);
    }


    /// <summary>
    /// Unlocks the current item if not already purchased.
    /// This is assigned to the 'buy button' on the shop-panel.
    /// </summary>
    public void UnlockItem()
    {
        var item = itemContainer[currentItemIndex];

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
        var item = itemContainer[currentItemIndex];


        if (!item.HasPurchased)
            return;

        SetCurrentUsingItemCheckmark(item);
        //Logging.Log("Using!");

        itemContainer[currentItemIndex].GetUsingCheckmark.enabled = true;

        //SaveManager.Instance.Save("CurrentItemUsed", item.StoreID);
        SaveSystem.SaveData("CurrentItemUsed", item.StoreID);
    }



    /// <summary>
    /// Loads a list of purchased item from the save manager.
    /// Initializes the items to their appropriate properties.
    /// </summary>
    void UnlockPurchasedItemsOnStart()
    {
        for (int i = 0; i < itemContainer.Count; i++)
        {
            //var itemId = SaveManager.Instance.LoadInt($"Item_{i}");
            var itemId = SaveSystem.GetData<int>($"Item_{i}");


            if (itemContainer[i].StoreID == itemId)
            {
                itemContainer[i].HasPurchased = true;

                CheckHasPurchased(currentItemIndex);
            }
        }
    }

    /// <summary>
    /// Adds a check-mark to indicate the item currently being used.
    /// </summary>
    /// <param name="currentItem">Current item</param>
    private void SetCurrentUsingItemCheckmark(Item currentItem)
    {
        currentItem.GetUsingCheckmark.enabled = true;

        for (int i = 0; i < itemContainer.Count; i++)
        {
            if (itemContainer[i] == currentItem)
                continue;

            if (itemContainer[i].GetUsingCheckmark.enabled)
                itemContainer[i].GetUsingCheckmark.enabled = false;

        }
    }

    /// <summary>
    /// Adds a check-mark on start to indicate the item currently being used.
    /// </summary>
    void SetCurrentUsingItemOnStart()
    {
        var itemId = SaveSystem.GetData<int>("CurrentItemUsed");

        for (int i = 0; i < itemContainer.Count; i++)
        {
            if (itemContainer[i].StoreID != itemId)
            {
                if (itemContainer[i].GetUsingCheckmark.enabled)
                    itemContainer[i].GetUsingCheckmark.enabled = false;
            }
            else
                itemContainer[i].GetUsingCheckmark.enabled = true;
        }

    }
}