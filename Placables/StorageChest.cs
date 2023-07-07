using UnityEngine;
using UnityEngine.InputSystem;
public class StorageChest : Buildings, IInteractable
{
    private StorageChestWindow storageChestWindow;
    private PlayerController player;
    private GameObject inventoryPosition;
    private GameObject Needs;
    
    private void Start()
    {
        storageChestWindow = GameObject.FindObjectOfType<StorageChestWindow>(true);
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        Needs = GameObject.FindGameObjectWithTag("Needs");
    }


    public void Update()
    {
        if (storageChestWindow.gameObject.activeSelf && (Keyboard.current.escapeKey.wasPressedThisFrame || Keyboard.current.tabKey.wasPressedThisFrame))
        {
            storageChestWindow.gameObject.SetActive(false);
            Needs.gameObject.SetActive(true);
            player.GetComponent<Inventory>().inventoryWindow.SetActive(false);
            player.ToggleCurser(false);
            inventoryPosition.GetComponent<RectTransform>().localPosition = new Vector3(0,-27,0);
        }    
    }

    public string GetInteractPrompt()
    {
        return "Storage Chest";
    }

    public void OnInteract()
    {
        storageChestWindow.gameObject.SetActive(true);
        player.ToggleCurser(true);
        player.GetComponent<Inventory>().inventoryWindow.SetActive(true);
        Needs.gameObject.SetActive(false);
        inventoryPosition = GameObject.FindGameObjectWithTag("InventoryBG");
        inventoryPosition.GetComponent<RectTransform>().localPosition = Vector3.right * 375;
    }
}