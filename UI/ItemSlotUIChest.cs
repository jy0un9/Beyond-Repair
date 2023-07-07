using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemSlotUIChest : MonoBehaviour
{
    public Button button;
    public Image icon;
    public TextMeshProUGUI quantityText;
    private ItemSlotChest currentSlot;
    private Outline outline;
    public int index;
    public bool equipped;

    private void Awake()
    {
        outline = GetComponent<Outline>();
    }

    private void OnEnable()
    {
        outline.enabled = equipped;
    }

    public void Set(ItemSlotChest slot)
    {
        currentSlot = slot;
        icon.gameObject.SetActive(true);
        icon.sprite = slot.item.icon;
        quantityText.text = slot.quantity > 1 ? slot.quantity.ToString() : string.Empty;
        
        if(outline != null)
        {
            outline.enabled = equipped;
        }
    }

    public void Clear()
    {
        currentSlot = null;
        icon.gameObject.SetActive(false);
        quantityText.text = string.Empty;
    }

    public void OnClickButton()
    {
        InventoryChest.instance.SelectItem(index);
    }
}