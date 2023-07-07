using UnityEngine;
using UnityEngine.InputSystem;

public class BuildingWindow : MonoBehaviour
{
    private PlayerController player;
    public GameObject Needs;
    public GameObject inventoryWindow;

    private void Start()
    {
        player = FindObjectOfType<PlayerController>();
        
    }
    private void Update()
    {
        if (Keyboard.current.tabKey.wasPressedThisFrame || Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            close();
        }
    }
    public void close()
    {
        gameObject.SetActive(false);
        player.ToggleCurser(false);
        Needs.gameObject.SetActive(true);
    }
    void OnEnable()
    {
        Inventory.instance.onOpenInventory.AddListener(OnOpenInventory);
        Needs.gameObject.SetActive(false);
        
        if (inventoryWindow.activeInHierarchy)
        {
            Inventory.instance.Toggle();
        }
    }

    void OnDisable()
    {
        Inventory.instance.onOpenInventory.RemoveListener(OnOpenInventory);
        Needs.gameObject.SetActive(true);
    }

    void OnOpenInventory()
    {
        gameObject.SetActive(false);
    }
}