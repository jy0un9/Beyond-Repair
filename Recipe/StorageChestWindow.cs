using UnityEngine;

public class StorageChestWindow : MonoBehaviour
{
    public static StorageChestWindow instance;

    private void Awake()
    {
        instance = this;
    }

    void OnEnable()
    {
        InventoryChest.instance.onOpenInventory.AddListener(OnOpenInventory);
    }

    void OnDisable()
    {
        InventoryChest.instance.onOpenInventory.RemoveListener(OnOpenInventory);
    }

    void OnOpenInventory()
    {
        gameObject.SetActive(false);
    }
}
