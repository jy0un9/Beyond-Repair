using System.Collections;
using UnityEngine;

public class CookingFire : MonoBehaviour, IInteractable
{
    [SerializeField] private ItemData fuelItem;
    [SerializeField] private int fuelQuantity = 1;
    private Inventory playerInventory;
    private bool isLit = false;
    public GameObject pointLight;

    private void Start()
    {
        playerInventory = FindObjectOfType<Inventory>();
    }

    public string GetInteractPrompt()
    {
        return isLit ? "Campfire" : "Light fire";
    }

    public void OnInteract()
    {
        if (!isLit && playerInventory != null && playerInventory.HasItems(fuelItem, fuelQuantity))
        {
            for (int i = 0; i < fuelQuantity; i++)
            {
                playerInventory.TryRemoveItem(fuelItem, 1);
            }

            StartCoroutine(LightCampfire());
        }
    }

    private IEnumerator LightCampfire()
    {
        isLit = true;
        pointLight.SetActive(true);
        AudioManager.Instance.Play("Fire",transform);
        yield return new WaitForSeconds(60f);
        pointLight.SetActive(false);
        AudioManager.Instance.Stop("Fire");
        isLit = false;
    }

    public bool IsLit()
    {
        return isLit;
    }
}