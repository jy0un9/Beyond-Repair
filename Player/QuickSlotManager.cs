using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class QuickSlotManager : MonoBehaviour
{
    public static QuickSlotManager instance;
    public Inventory inventory;
    public EquipManager equipManager;
    public List<ItemData> quickSlots;
    public int currentQuickSlotIndex;
    public PlayerInput playerInput;
    public ItemData[] itemsToAdd;
    public Image[] images;
    public ItemData[] upgradedItems;
    public ItemData[] upgradedItemsTier2;
    public event Action<ItemData> OnQuickSlotItemRemoved;

    private void Awake()
    {
        instance = this;
        playerInput = GameObject.FindWithTag("Player")?.GetComponent<PlayerInput>();
    }

    private void Start()
    {
        quickSlots = new List<ItemData>();
        currentQuickSlotIndex = 0;
        AddPermanentItemsToQuickSlots();
        UpdateQuickSlotUI();
        UpdateQuickSlotColors();
    }

    private void AddPermanentItemsToQuickSlots()
    {
        List<ItemData> permanentItems = Inventory.instance?.PermanentItems;
        for (int i = 0; i < permanentItems?.Count; i++)
        {
            if (permanentItems[i].type == ItemType.Equipable)
            {
                AddToQuickSlot(permanentItems[i], images[i]);
            }
        }
    }

    public void AddEquippableItemsToQuickSlots()
    {
        int maxIndex = Mathf.Min(itemsToAdd.Length, images.Length);
        for (int i = 0; i < maxIndex; i++)
        {
            if (Inventory.instance.HasItems(itemsToAdd[i], 1) && itemsToAdd[i].type == ItemType.Equipable)
            {
                AddToQuickSlot(itemsToAdd[i], images[i]);
            }
        }
        ReplaceUpgradedItems();

    }
    private void ReplaceUpgradedItems()
    {
        for (int i = 0; i < itemsToAdd.Length; i++)
        {
            ItemData itemToEquip = null;

            if (Inventory.instance.HasItems(upgradedItemsTier2[i], 1))
            {
                itemToEquip = upgradedItemsTier2[i];
            }
            else if (Inventory.instance.HasItems(upgradedItems[i], 1))
            {
                itemToEquip = upgradedItems[i];
            }

            if (itemToEquip != null && itemsToAdd[i] != itemToEquip)
            {
                RemoveItemFromQuickSlot(itemsToAdd[i]);
                AddToQuickSlot(itemToEquip, images[i]);
            }
        }
    }


    private void OnEnable()
    {
        playerInput.actions["CycleQuickSlots"].started += OnCycleQuickSlots;
        if (Inventory.instance != null)
        {
            Inventory.instance.OnItemRemoved += OnItemRemovedFromInventory;
        }
        if (equipManager != null)
        {
            equipManager.OnItemEquipped += OnItemEquipped;
        }
        OnQuickSlotItemRemoved += OnItemRemovedFromInventory;
    }

    private void OnDisable()
    {
        if (playerInput != null)
        {
            playerInput.actions["CycleQuickSlots"].started -= OnCycleQuickSlots;
        }
        if (Inventory.instance != null)
        {
            Inventory.instance.OnItemRemoved -= OnItemRemovedFromInventory;
        }
        if (equipManager != null)
        {
            equipManager.OnItemEquipped -= OnItemEquipped;
        }
        OnQuickSlotItemRemoved -= OnItemRemovedFromInventory;
    }
    
    public void RemoveItemFromQuickSlot(ItemData item)
    {
        OnQuickSlotItemRemoved?.Invoke(item);
    }

    private void OnItemEquipped(ItemData item) => UpdateQuickSlotColors();

    public void OnItemRemovedFromInventory(ItemData item)
    {
        if (item.type == ItemType.Equipable)
        {
            int itemIndex = quickSlots.IndexOf(item);
            if (itemIndex == currentQuickSlotIndex)
            {
                Inventory.instance.Unequip(itemIndex);
                currentQuickSlotIndex = -1;
            }
            RemoveFromQuickSlot(item);
            UpdateQuickSlotColors();
        }
    }

    private void OnCycleQuickSlots(InputAction.CallbackContext context)
    {
        Vector2 scrollDelta = context.ReadValue<Vector2>();
        CycleQuickSlots(scrollDelta.y < 0 ? -1 : 1);
    }

    public void AddToQuickSlot(ItemData item, Image image)
    {
        if (!quickSlots.Contains(item))
        {
            quickSlots.Add(item);
            UpdateQuickSlotUI();
        }
    }

    public void RemoveFromQuickSlot(ItemData item)
    {
        if (quickSlots.Remove(item))
        {
            UpdateQuickSlotUI();
            if (currentQuickSlotIndex == quickSlots.IndexOf(item))
            {
                currentQuickSlotIndex = (currentQuickSlotIndex - 1 + quickSlots.Count) % quickSlots.Count;
            }
            UpdateQuickSlotColors();
        }
    }

    public void CycleQuickSlots(int direction)
    {
        if (quickSlots.Count == 0) return;
        currentQuickSlotIndex = (currentQuickSlotIndex + direction + quickSlots.Count) % quickSlots.Count;
        EquipQuickSlotItem();
    }

    private void EquipQuickSlotItem()
    {
        ItemData itemToEquip = quickSlots[currentQuickSlotIndex];

        if (itemToEquip?.type == ItemType.Equipable)
        {
            inventory?.OnEquipButton();
            equipManager?.EquipNewItem(itemToEquip);
        }
    
        UpdateQuickSlotColors();
    }

    private void UpdateQuickSlotColors()
    {
        Color equippedColor = new Color(253 / 255f, 255 / 117f, 0 / 255f);
        Color defaultColor = new Color(200 / 255f, 200 / 255f, 200 / 255f);

        for (int i = 0; i < images.Length; i++)
        {
            Image parentImage = images[i].transform.parent.GetComponent<Image>();

            if (i < quickSlots.Count)
            {
                parentImage.color = (i == currentQuickSlotIndex) ? equippedColor : defaultColor;
            }
            else
            {
                parentImage.color = defaultColor;
            }
        }

        if (currentQuickSlotIndex == -1)
        {
            foreach (Image img in images)
            {
                img.transform.parent.GetComponent<Image>().color = defaultColor;
            }
        }
    }

    public void UpdateEquippedItem(ItemData item)
    {
        int itemIndex = quickSlots.IndexOf(item);
        if (itemIndex != -1)
        {
            currentQuickSlotIndex = itemIndex;
            UpdateQuickSlotColors();
        }
    }

    public void UpdateQuickSlotUI()
    {
        for (int i = 0; i < images.Length; i++)
        {
            if (i < quickSlots.Count)
            {
                images[i].enabled = true;
                images[i].sprite = quickSlots[i].icon;
            }
            else
            {
                images[i].enabled = false;
            }
        }
    }
}
