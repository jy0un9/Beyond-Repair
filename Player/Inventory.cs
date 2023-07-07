using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using System;
using Random = UnityEngine.Random;

public class Inventory : MonoBehaviour
{
    public static Inventory instance;

    [Header("Item Slots")]
    public ItemSlotUI[] uiSlots;
    public ItemSlot[] slots;

    [Header("Windows")]
    public GameObject inventoryWindow;
    public GameObject anvilWindow;
    public GameObject forgeWindow;
    public GameObject craftingWindow;
    public GameObject cookingWindow;
    public GameObject buildingWindow;
    public GameObject StorageCanvas;

    [Header("Selected Item")]
    public TextMeshProUGUI selectedItemName;
    public TextMeshProUGUI selectedItemDescription;
    public TextMeshProUGUI selectedItemStatName;
    public TextMeshProUGUI selectedItemStatValue;
    private ItemSlot selectedItem;
    private int selectedItemIndex;

    [Header("Buttons")]
    public GameObject useButton;
    public GameObject EquipButton;
    public GameObject UnequipButton;
    public GameObject dropButton;
    public GameObject AddButton;
    public GameObject AddAllButton;

    [Header("Other")]
    public Transform dropPosition;
    public GameObject Needs;
    private int currentEquipndex;
    private PlayerController controller;
    private PlayerNeeds needs;

    [Header("Permanent Items")]
    public List<ItemData> permanentItems;
    public List<ItemData> PermanentItems => permanentItems;

    public ItemData waterBottle;
    public ItemData waterBottleEmpty;
    public ItemData seaWaterBottle;

    [Header("Events")]
    public UnityEvent onOpenInventory;
    public UnityEvent onCloseInventory;

    public GameObject inventoryPosition;
    public Action<ItemData> OnItemRemoved;


    private void Start()
    {
        inventoryWindow.SetActive(false);
        slots = new ItemSlot[uiSlots.Length];

        for (int x = 0; x < slots.Length; x++)
        {
            slots[x] = new ItemSlot();
            uiSlots[x].index = x;
            uiSlots[x].Clear();
        }

        ClearSelectedItemWindow();
        StartCoroutine(delayStart());

    }

    private IEnumerator delayStart()
    {
        yield return new WaitForSeconds(0.2f);
        foreach (ItemData item in permanentItems)
        {
            if (item != waterBottleEmpty && item != seaWaterBottle)
            {
                int slotIndex = PermanentItemSlotIndex(item);
                if (slotIndex == -1)
                {
                    AddItem(item);
                }
                else
                {
                    slots[slotIndex].quantity = 1;
                }
            }
        }
        UpdateUI();
        QuickSlotManager.instance.AddEquippableItemsToQuickSlots();
    }
      
    private int PermanentItemSlotIndex(ItemData item)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item == item)
            {
                return i;
            }
        }
        return -1;
    }
      
    private void Awake()
    {
        instance = this;
        controller = GetComponent<PlayerController>();
        needs = GetComponent<PlayerNeeds>();
    }     
     
    public void ReplaceItem(ItemData oldItem, ItemData newItem)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item == oldItem)
            {
                slots[i].item = newItem;
                UpdateUI();
                return;
            }
        }
    }
    public int GetItemID(ItemData item)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item == item)
            {
                return i;
            }
        }
        return -1;
    }
     
    public void OnInventoryButton(InputAction.CallbackContext context)
    {
        if(context.phase == InputActionPhase.Started)
        {
            Toggle();
        }
    }
    
    public void Toggle()
    {
        if (TutorialManager.Instance.isTutorialActive || PauseMenu.Instance.IsPauseMenuOpen())
            return;

        
        AudioManager.Instance.PlayOneShot("Bag");

        if(inventoryWindow.activeInHierarchy)
        {
            inventoryWindow.SetActive(false);
            onCloseInventory.Invoke();
            controller.ToggleCurser(false);
            Needs.gameObject.SetActive(true);
            
            if(StorageCanvas.activeInHierarchy)
            {
                StorageCanvas.SetActive(false);
                InventoryChest.instance.onCloseInventory.Invoke();
                inventoryPosition.GetComponent<RectTransform>().localPosition = new Vector3(0,-27,0);
            }
            
            if(inventoryWindow.activeInHierarchy)
            {
                anvilWindow.SetActive(false);
                ForgeWindow.instance.OnOpenInventory();
                CraftingWindow.instance.OnOpenInventory();
                CookingWindow.instance.OnDisable();
                buildingWindow.SetActive(false);
            }
        }
        else
        {
            inventoryWindow.SetActive(true);
            onOpenInventory.Invoke();
            ClearSelectedItemWindow();
            controller.ToggleCurser(true);
            Needs.gameObject.SetActive(false);
        }
    }
    
    public bool IsOpen()
    {
        return inventoryWindow.activeInHierarchy;
    }
    
    public void AddItem(ItemData item)
    {
        ItemSlot slotToStackTo = null;

        if (item.canStackItem)
        {
            slotToStackTo = GetItemStack(item);
        }

        if (slotToStackTo != null)
        {
            slotToStackTo.quantity++;
            UpdateUI();
            return;
        }
    
        ItemSlot emptySlot = GetEmptySlot();

        if (emptySlot != null)
        {
            emptySlot.item = item;
            emptySlot.quantity = 1;
            UpdateUI();
            QuickSlotManager.instance.AddEquippableItemsToQuickSlots();
            return;
        }
        ThrowItem(item);
    }

    void ThrowItem(ItemData item)
    {
        GameObject droppedItem = Instantiate(item.dropPrefab, dropPosition.position, Quaternion.Euler(new Vector3(0, Random.value * 360.0f, 0)));
        FloatAndPulse floatAndPulse = droppedItem.GetComponent<FloatAndPulse>();
        if (floatAndPulse != null)
        {
            floatAndPulse.enabled = false;
        }
    }


    public void UpdateUI()
    {
        for(int x = 0; x < slots.Length; x++)
        {
            if (slots[x].item != null)
            {
                uiSlots[x].Set(slots[x]);
            }
            else
            {
                uiSlots[x].Clear();
            }
        }
    }

    ItemSlot GetItemStack(ItemData item)
    {
        for (int x = 0; x < slots.Length; x++)
        {
            if(slots[x].item == item && slots[x].quantity < item.maxStackAmount)
            {
                return slots[x];
            }
        }
        return null;
    }

    ItemSlot GetEmptySlot()
    {
        for (int x = 0; x < slots.Length; x++)
        {
            if(slots[x].item == null)
            {
                return slots[x];
            }
        }
        return null;
    }

    public void SelectItem(int index)
    {
        if(slots[index].item == null)
        {
            return;
        }

        selectedItem = slots[index];
        selectedItemIndex = index;
        selectedItemName.text = selectedItem.item.displayName;
        selectedItemDescription.text = selectedItem.item.description;
        selectedItemStatName.text = string.Empty;
        selectedItemStatValue.text = string.Empty;

        for(int x = 0; x < selectedItem.item.consumables.Length;x++)
        {
            selectedItemStatName.text += selectedItem.item.consumables[x].type.ToString() + "\n";
            selectedItemStatValue.text += selectedItem.item.consumables[x].Value.ToString() + "\n";
        }

        useButton.SetActive(selectedItem.item.type == ItemType.Consumable);
        EquipButton.SetActive(selectedItem.item.type == ItemType.Equipable && !uiSlots[index].equipped);
        UnequipButton.SetActive(selectedItem.item.type == ItemType.Equipable && uiSlots[index].equipped);
        dropButton.SetActive(true);
        
        if (StorageCanvas.activeInHierarchy)
        {
            AddButton.SetActive(true);
            AddAllButton.SetActive(true);  
        }
    }

    void ClearSelectedItemWindow()
    {
        selectedItem = null;
        selectedItemName.text = string.Empty;
        selectedItemDescription.text = string.Empty;
        selectedItemStatName.text = string.Empty;
        selectedItemStatValue.text = string.Empty;
        useButton.SetActive(false);
        EquipButton.SetActive(false);
        UnequipButton.SetActive(false);
        dropButton.SetActive(false);
        AddButton.SetActive(false);
        AddAllButton.SetActive(false);
    }

    public void OnUseButton()
    {
        AudioManager.Instance.PlayOneShot("Click");

        if(selectedItem.item.type == ItemType.Consumable)
        {
            for(int x = 0; x < selectedItem.item.consumables.Length; x++)
            {
                switch(selectedItem.item.consumables[x].type)
                {
                    case ConsumableType.Health: needs.Heal(selectedItem.item.consumables[x].Value);break;
                    case ConsumableType.Hunger: needs.Eat(selectedItem.item.consumables[x].Value); 
                        break;
                    case ConsumableType.Thirst: needs.Drink(selectedItem.item.consumables[x].Value); 
                        break;
                    case ConsumableType.Sleep: needs.Sleep(selectedItem.item.consumables[x].Value); break;
                }
            }
        }
        switch (selectedItem.item.id)
        {
            case "Apple":
            case "Apple_2":
            case "Beetroot":
            case "Cabbage":
            case "Can_Of_Tuna":
            case "Canned_Food_1":
            case "Canned_Food_2":
            case "Carrot":
            case "Cooked_Meat":
            case "Pain_Killers":
            case "Sardines":
                AudioManager.Instance.PlayOneShot("Eat");
                break;
            case "Water_Bottle":
            case "Soft Drink":
                AudioManager.Instance.PlayOneShot("Drink");
                break;
        }
        
        RemoveSelectedItem();
    }


    public void OnEquipButton()
    {
        AudioManager.Instance.PlayOneShot("Click");

        if (uiSlots == null || selectedItem == null)
        {
            return;
        }

        if (uiSlots[currentEquipndex].equipped)
        {
            Unequip(currentEquipndex);
        }

        uiSlots[selectedItemIndex].equipped = true;
        currentEquipndex = selectedItemIndex;

        if (EquipManager.instance != null)
        {
            EquipManager.instance.EquipNewItem(selectedItem.item);
        }
        else
        {
            Debug.LogWarning("EquipManager instance is not properly initialized.");
        }
        QuickSlotManager.instance.AddEquippableItemsToQuickSlots();

        UpdateUI();
        SelectItem(selectedItemIndex);
    }


    public void Unequip(int index)
    {
        uiSlots[index].equipped = false;
        EquipManager.instance.UnEquipItem();
        UpdateUI();

        if(selectedItemIndex == index)
        {
            SelectItem(index);
        }
    }

    public void OnUnequipButton()
    {
        AudioManager.Instance.PlayOneShot("Click");

        Unequip(selectedItemIndex);
    }

    public void OnDropButton()
    {
        AudioManager.Instance.PlayOneShot("Click");

        if (IsPermanentItem(selectedItem.item))
        {
            return;
        }
        OnItemRemoved?.Invoke(selectedItem.item);
        QuickSlotManager.instance.RemoveItemFromQuickSlot(selectedItem.item);
        ThrowItem(selectedItem.item);
        RemoveSelectedItem();
    }
    
    public void OnAddButton()
    {
        AudioManager.Instance.PlayOneShot("Click");

        if(!IsPermanentItem(selectedItem.item))
        {
            if(uiSlots[selectedItemIndex].equipped)
                Unequip(selectedItemIndex);
            
            InventoryChest.instance.AddItem(selectedItem.item);
            OnItemRemoved?.Invoke(selectedItem.item);
            QuickSlotManager.instance.RemoveItemFromQuickSlot(selectedItem.item);
            RemoveSelectedItem();
        }
    }
    public void OnAddAllButton()
    {
        AudioManager.Instance.PlayOneShot("Click");

        for (int x = 0; x < slots.Length; x++)
        {
            if (slots[x].item != null)
            {
                if (!IsPermanentItem(slots[x].item))
                {
                    if (uiSlots[x].equipped)
                    {
                        Unequip(x);
                    }

                    for (int y = 0; y < slots[x].quantity; y++)
                    {
                        InventoryChest.instance.AddItem(slots[x].item);
                    }

                    OnItemRemoved?.Invoke(slots[x].item);
                    QuickSlotManager.instance.RemoveItemFromQuickSlot(slots[x].item);
                    slots[x].item = null;
                    slots[x].quantity = 0;
                }
            }
        }
        UpdateUI();
        ClearSelectedItemWindow();
    }
    
    void RemoveSelectedItem()
    {
        selectedItem.quantity--;

        if(selectedItem.quantity == 0)
        {
            if (uiSlots[selectedItemIndex].equipped == true)
            {
                Unequip(selectedItemIndex);
                OnItemRemoved?.Invoke(selectedItem.item);
            }
            if (selectedItem.item == waterBottle)
            {
                selectedItem.item = waterBottleEmpty;
                selectedItem.quantity = 1;
                if(inventoryWindow.activeSelf)
                    Toggle();
                TutorialManager.Instance.TaskCompleted("WATER",0.2f);

            }
            else
            {
                selectedItem.item = null;
            }

            ClearSelectedItemWindow();
        }
        UpdateUI();
    }

    public void RemoveItem(ItemData item)
    {
        if (IsPermanentItem(item))
        {
            return;
        }
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item == item)
            {
                slots[i].quantity--;
            
                if (slots[i].quantity == 0)
                {
                    if (uiSlots[i].equipped == true)
                    {
                        Unequip(i);
                        ClearSelectedItemWindow();
                    }
                    slots[i].item = null;
                    OnItemRemoved?.Invoke(item);
                    UpdateUI();
                    return;
                }
            }
        }
    }

    
    public void ForceRemoveItem(ItemData item, int quantity = 1)
    {
        int removedCount = 0;

        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item == item)
            {
                int itemsToRemove = Mathf.Min(quantity - removedCount, slots[i].quantity);
                slots[i].quantity -= itemsToRemove;
                removedCount += itemsToRemove;

                if (slots[i].quantity == 0)
                {
                    if (uiSlots[i].equipped == true)
                    {
                        Unequip(i);
                    }
                    slots[i].item = null;
                    OnItemRemoved?.Invoke(item);
                    ClearSelectedItemWindow();
                    UpdateUI();

                }

                if (removedCount == quantity)
                {
                    OnItemRemoved?.Invoke(item);
                    UpdateUI();
                    return;
                }
            }
        }

        UpdateUI();
    }
    
    private bool IsPermanentItem(ItemData item)
    {
        return permanentItems.Contains(item);
    }

    
    public bool HasItems(ItemData item, int quantity)
    {

    
        int amount = 0;
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item == item)
            {
                amount += slots[i].quantity;
            }
            if (amount >= quantity)
            {
                return true;
            }
        }
        return false;
    }

    
    public bool TryRemoveItem(ItemData item, int quantity)
    {
        if (!HasItems(item, quantity))
        {
            return false;
        }

        int removedCount = 0;

        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item == item)
            {
                int itemsToRemove = Mathf.Min(quantity - removedCount, slots[i].quantity);
                slots[i].quantity -= itemsToRemove;
                removedCount += itemsToRemove;

                if (slots[i].quantity == 0)
                {
                    if (uiSlots[i].equipped == true)
                    {
                        Unequip(i);
                    }
                    slots[i].item = null;
                    ClearSelectedItemWindow();
                    UpdateUI();
                }

                if (removedCount == quantity)
                {
                    OnItemRemoved?.Invoke(item);
                    UpdateUI();
                    return true;
                }
            }
        }
        UpdateUI();
        return false;
    }
}

public class ItemSlot
{
    public ItemData item;
    public int quantity;
}