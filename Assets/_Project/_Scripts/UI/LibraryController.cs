using Racer.SaveSystem;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Control the activities of the shop-menu like buying and using of items.
/// </summary>
internal class LibraryController : MonoBehaviour
{

    // Holds items available for purchase in the store
    private readonly List<Item> _itemContainer = new();

    private int _currentItemIndex;

    [SerializeField, TextArea]
    private string lockedPlaceholderText;

    [Header("MISC")]
    [SerializeField] private Image lockSpr;
    [SerializeField] private Button setBtn;

    [Space(5), Header("ITEM PROPERTIES")]
    [SerializeField] private Transform itemParent;
    [SerializeField] private Item itemPrefab;
    [SerializeField] private TextMeshProUGUI itemDescT;


    private void Awake()
    {
        SpawnItems();
    }

    private void Start()
    {
        InitItems();
        CheckMatchedItemOnStart();
        SetCurrentUsingItemOnStart();
        CheckNextUnlockItem();
        CheckHasUnlocked();
    }

    /// <summary>
    /// Spawn and add items to be purchased in a list.
    /// </summary>
    private void SpawnItems()
    {
        var amount = ItemManager.Instance.ItemCount;

        for (var i = 0; i < amount; i++)
        {
            var cloneItem = Instantiate(itemPrefab, itemParent);

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
        for (var i = 0; i < _itemContainer.Count; i++)
        {
            var item = _itemContainer[i];
            var itemDetail = ItemManager.Instance.GetPuzzle(i);

            if (!item) continue;

            item.SetSprite = itemDetail.PreviewImage;
            item.StoreID = itemDetail.ItemID;
            item.Name = itemDetail.ImageName;
            item.Description = itemDetail.Description;
        }

        _itemContainer[0].gameObject.SetActive(true);
    }

    public void ForwardArrow()
    {
        if (_currentItemIndex >= _itemContainer.Count - 1)
            return;

        DisableItem(_currentItemIndex);

        _currentItemIndex++;

        EnableItem(_currentItemIndex);
    }

    public void BackwardArrow()
    {
        if (_currentItemIndex <= 0)
            return;

        DisableItem(_currentItemIndex);

        _currentItemIndex--;

        EnableItem(_currentItemIndex);
    }

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
        CheckHasMatched(index);

        CheckHasUnlocked();

        _itemContainer[index].gameObject.SetActive(true);
    }

    /// <summary>
    /// Sets the previous item in the list active.
    /// </summary>
    private void DisableItem(int index)
    {
        _itemContainer[index].gameObject.SetActive(false);
    }

    private void CheckHasUnlocked()
    {
        setBtn.interactable = _itemContainer[_currentItemIndex].IsUnlocked;
    }

    private void CheckHasMatched(int index)
    {
        if (_itemContainer[index].IsMatched)
            InitMatchedItem(_itemContainer[index].Description, true);
        else
            InitMatchedItem(t: lockedPlaceholderText, false);
    }

    private void InitMatchedItem(string t, bool isMatched)
    {
        if (itemDescT.text != t)
            itemDescT.text = t;

        lockSpr.enabled = !isMatched;
    }

    public void UseItem()
    {
        var item = _itemContainer[_currentItemIndex];

        if (!item.IsUnlocked)
            return;

        SetCurrentUsingItemCheckmark(item);

        _itemContainer[_currentItemIndex].GetCheckmark.enabled = true;

        SaveSystem.SaveData(Metrics.CurrentItemInUse, item.StoreID);
    }

    private void CheckMatchedItemOnStart()
    {
        for (var i = 0; i < _itemContainer.Count; i++)
        {
            var itemId = SaveSystem.GetData(Metrics.IsMatched(i), -1);

            if (_itemContainer[i].StoreID != itemId) continue;

            _itemContainer[i].IsMatched = true;
        }

        CheckHasMatched(_currentItemIndex);

        UIControllerMain.Instance.TotalBooksRead = _itemContainer.FindAll(b => b.IsMatched).Count;
    }

    private void CheckNextUnlockItem()
    {
        for (var i = 0; i < _itemContainer.Count; i++)
        {
            if (_itemContainer[i].IsMatched)
            {
                var index = i + 1;

                if (index < _itemContainer.Count)
                    _itemContainer[index].IsUnlocked = true;

                _itemContainer[i].IsUnlocked = true;
            }
        }
    }

    /// <summary>
    /// Adds a check-mark to indicate the item currently being used.
    /// </summary>
    /// <param name="currentItem">Current item</param>
    private void SetCurrentUsingItemCheckmark(Item currentItem)
    {
        currentItem.GetCheckmark.enabled = true;

        for (var i = 0; i < _itemContainer.Count; i++)
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

        for (var i = 0; i < _itemContainer.Count; i++)
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