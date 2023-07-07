using UnityEngine;

public class ItemObject : MonoBehaviour, IInteractable
{
    public ItemData item;
    public int amount = 1;

    public string GetInteractPrompt()
    {
        return string.Format("Pickup {0}", item.displayName);
    }

    public void OnInteract()
    {
        for (int i = 0; i < amount; i++)
        {
            Inventory.instance.AddItem(item);
        }
        
        AudioManager.Instance.PlayOneShot("Collect");
        Destroy(gameObject);
    }
}
