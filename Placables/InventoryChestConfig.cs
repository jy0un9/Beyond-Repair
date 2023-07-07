using UnityEngine;
using TMPro;

[CreateAssetMenu(fileName = "InventoryChestConfig", menuName = "InventoryChest/InventoryChestConfig")]
public class InventoryChestConfig : ScriptableObject
{
    public ItemSlotUIChest[] uiSlots;
    public ItemSlotChest[] slots;
    public GameObject inventoryWindow;
    public Transform dropPosition;
    public TextMeshProUGUI selectedItemName;
    public TextMeshProUGUI selectedItemDescription;
    public TextMeshProUGUI selectedItemStatName;
    public TextMeshProUGUI selectedItemStatValue;
    public GameObject takeButton;
    public GameObject takeAllButton;
    public int uiSlotsLength;
}
