using UnityEngine;
using TMPro;

public class InventoryChestReferences : MonoBehaviour
{
    public GameObject inventoryWindow;
    public Transform dropPosition;
    public TextMeshProUGUI selectedItemName;
    public TextMeshProUGUI selectedItemDescription;
    public TextMeshProUGUI selectedItemStatName;
    public TextMeshProUGUI selectedItemStatValue;
    public GameObject takeButton;
    public GameObject takeAllButton;
    public ItemSlotUIChest[] uiSlots;
    private static InventoryChestReferences _instance;

    public static InventoryChestReferences Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<InventoryChestReferences>();
            }

            return _instance;
        }
    }
}
