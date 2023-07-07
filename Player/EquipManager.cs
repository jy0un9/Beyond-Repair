using UnityEngine;
using UnityEngine.InputSystem;

public class EquipManager : MonoBehaviour
{
    private Equip currentEquip;
    public Transform equipParent;
    private PlayerController controller;
    public delegate void ItemEquippedHandler(ItemData item);
    public event ItemEquippedHandler OnItemEquipped;
    public static EquipManager instance;

    private void Awake()
    {
        instance = this;
        controller = GetComponent<PlayerController>();
    }

    public void OnAttackInput(InputAction.CallbackContext context)
    {
        if(context.phase == InputActionPhase.Performed && currentEquip != null && controller.canLook == true)
        {
            currentEquip.OnAttackInput();
        }
    }

    public void OnAltAttackInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed && currentEquip != null && controller.canLook == true)
        {
            currentEquip.OnAltAttackInput();
        }
    }

    public void EquipNewItem(ItemData item)
    {
        OnItemEquipped?.Invoke(item);
        QuickSlotManager.instance.UpdateEquippedItem(item);

        UnEquipItem();
        currentEquip = Instantiate(item.equipPrefabs, equipParent).GetComponent<Equip>();
    }

    public void UnEquipItem()
    {
        if(currentEquip != null)
        {
            Destroy(currentEquip.gameObject);
            currentEquip = null;
        }
    }
}
