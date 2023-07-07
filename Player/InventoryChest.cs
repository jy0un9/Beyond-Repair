using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class InventoryChest : MonoBehaviour
{
    public ItemSlotUIChest[] uiSlots;
    public ItemSlotChest[] slots;

    public GameObject inventoryWindow;
    public Transform dropPosition;

    [Header("Selected item")]
    private ItemSlotChest selectedItem;
    private int selectedItemIndex;
    public TextMeshProUGUI selectedItemName;
    public TextMeshProUGUI selectedItemDescription;
    public TextMeshProUGUI selectedItemStatName;
    public TextMeshProUGUI selectedItemStatValue;
    public GameObject takeButton;
    public GameObject takeAllButton;
    
    private int currentEquipndex;

    private PlayerController controller;
    private PlayerNeeds needs;

    [Header("Events")]
    public UnityEvent onOpenInventory;
    public UnityEvent onCloseInventory;

    public static InventoryChest instance;
    
    private void Awake()
    {
        instance = this;
        controller = GetComponent<PlayerController>();
        needs = GetComponent<PlayerNeeds>();

        inventoryWindow = InventoryChestReferences.Instance.inventoryWindow;
        dropPosition = InventoryChestReferences.Instance.dropPosition;
        selectedItemName = InventoryChestReferences.Instance.selectedItemName;
        selectedItemDescription = InventoryChestReferences.Instance.selectedItemDescription;
        selectedItemStatName = InventoryChestReferences.Instance.selectedItemStatName;
        selectedItemStatValue = InventoryChestReferences.Instance.selectedItemStatValue;
        takeButton = InventoryChestReferences.Instance.takeButton;
        takeAllButton = InventoryChestReferences.Instance.takeAllButton;
        uiSlots = InventoryChestReferences.Instance.uiSlots;
    }


    private void Start()
    {
        inventoryWindow.SetActive(false);
        slots = new ItemSlotChest[uiSlots.Length];

        for (int x = 0; x< slots.Length;x++)
        {
            slots[x] = new ItemSlotChest();
            uiSlots[x].index = x;
            uiSlots[x].Clear();
        }

        ClearSelectedItemWindow();
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
        if(inventoryWindow.activeInHierarchy)
        {
            inventoryWindow.SetActive(false);
            onCloseInventory.Invoke();
            controller.ToggleCurser(false);
        }
        else
        {
            inventoryWindow.SetActive(true);
            onOpenInventory.Invoke();
            ClearSelectedItemWindow();
            controller.ToggleCurser(true);
        }
    }

    public bool IsOpen()
    {
        return inventoryWindow.activeInHierarchy;
    }

    public void AddItem(ItemData item)
    {
        if(item.canStackItem)
        {
            ItemSlotChest slotToStackTo = GetItemStack(item);
            if(slotToStackTo != null)
            {
                slotToStackTo.quantity++;
                UpdateUI();
                return;
            }
        }
        ItemSlotChest emptySlot = GetEmptySlot();
        if(emptySlot != null)
        {
            emptySlot.item = item;
            emptySlot.quantity = 1;
            UpdateUI();
            return;
        }
        ThrowItem(item);
    }

    void ThrowItem(ItemData item)
    {
        Instantiate(item.dropPrefab, dropPosition.position, Quaternion.Euler(Vector3.one * Random.value * 360.0f));
    }

    void UpdateUI()
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

    ItemSlotChest GetItemStack(ItemData item)
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

    ItemSlotChest GetEmptySlot()
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

        takeButton.SetActive(true);
        takeAllButton.SetActive(true);
        
        takeButton = GameObject.FindGameObjectWithTag("TakeButton");
        takeButton.GetComponent<Button>().onClick.RemoveAllListeners();
        takeButton.GetComponent<Button>().onClick.AddListener(OnTakeButton);

        takeAllButton = GameObject.FindGameObjectWithTag("TakeAllButton");
        takeAllButton.GetComponent<Button>().onClick.RemoveAllListeners();
        takeAllButton.GetComponent<Button>().onClick.AddListener(OnTakeAllButton);
    }

    void ClearSelectedItemWindow()
    {
        selectedItem = null;
        selectedItemName.text = string.Empty;
        selectedItemDescription.text = string.Empty;
        selectedItemStatName.text = string.Empty;
        selectedItemStatValue.text = string.Empty;
        takeButton.SetActive(false);
        takeAllButton.SetActive(false);
    }

    public void OnUseButton()
    {
        if(selectedItem.item.type == ItemType.Consumable)
        {
            for(int x = 0; x < selectedItem.item.consumables.Length; x++)
            {
                switch(selectedItem.item.consumables[x].type)
                {
                    case ConsumableType.Health: needs.Heal(selectedItem.item.consumables[x].Value);break;
                    case ConsumableType.Hunger: needs.Eat(selectedItem.item.consumables[x].Value); break;
                    case ConsumableType.Thirst: needs.Drink(selectedItem.item.consumables[x].Value); break;
                    case ConsumableType.Sleep: needs.Sleep(selectedItem.item.consumables[x].Value); break;
                }
            }
        }

        RemoveSelectedItem();
    }

    public void OnEquipButton()
    {
        if(uiSlots[currentEquipndex].equipped)
        {
            Unequip(currentEquipndex);
        }

        uiSlots[selectedItemIndex].equipped = true;
        currentEquipndex = selectedItemIndex;
        EquipManager.instance.EquipNewItem(selectedItem.item);
        UpdateUI();
        SelectItem(selectedItemIndex);
    }

    void Unequip(int index)
    {
        AudioManager.Instance.PlayOneShot("Click");

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

        ThrowItem(selectedItem.item);
        RemoveSelectedItem();
    }
    
    public void OnTakeButton()
    {
        AudioManager.Instance.PlayOneShot("Click");

        Inventory.instance.AddItem(selectedItem.item);
        RemoveSelectedItem();
    }

    public void OnTakeAllButton()
    {
        AudioManager.Instance.PlayOneShot("Click");

        for(int x = 0; x < slots.Length; x++)
        {
            if(slots[x].item != null)
            {
                for(int y = 0; y < slots[x].quantity; y++)
                {
                    if(uiSlots[x].equipped)
                        Unequip(x);
                    
                    Inventory.instance.AddItem(slots[x].item);
                }
                slots[x].item = null;
                slots[x].quantity = 0;
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
            }
            selectedItem.item = null;
            ClearSelectedItemWindow();
        }

        UpdateUI();
    }
    
    public void RemoveItem(ItemData item)
    {
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
                        slots[i].item = null;
                        ClearSelectedItemWindow();
                    }

                    UpdateUI();
                    return;
                }
            }
        }
    }

    public bool HasItems(ItemData item,int quantity)
    {
        int amount = 0;
        
        for(int i = 0; i < slots.Length; i++)
        {
            
            if(slots[i].item == item)
            {
                amount += slots[i].quantity;
            }
            
            if(amount >= quantity)
            {
                return true;
            }
        }
        return false;
    }
}

public class ItemSlotChest
{
    public ItemData item;
    public int quantity;
}