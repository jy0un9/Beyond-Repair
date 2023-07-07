using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CookingWindow : MonoBehaviour
{
    public CookingRecipeUI[] recipeUIs;
    //singelton
    public static CookingWindow instance;
    private PlayerController player;
    public GameObject Needs;
    public GameObject inventoryWindow;

    private Slider slider;
    public GameObject sliderObject;
    public float FillTime = 1.0f;
    public Image img;
    public GameObject craftedItemContainer;
    public ParticleSystem confetti;
    private bool isCraftingInProgress = false;
    
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
        
        if(sliderObject.activeInHierarchy)
        {
            if (slider == null)
            {
                slider = sliderObject.GetComponent<Slider>();
                Reset();
            }

            slider.value = Time.time;
        }
    }
    
    public void Reset()
    {
        slider = sliderObject.GetComponent<Slider>();
        slider.minValue = Time.time;
        slider.maxValue = Time.time + FillTime;
    }
    public void ResetAll()
    {
        if (slider != null)
        {
            slider.value = 0;
        }
        sliderObject.SetActive(false);
        craftedItemContainer.SetActive(false);
        isCraftingInProgress = false;
    }

    public void close()
    {
        ResetAll();
        gameObject.SetActive(false);
        player.ToggleCurser(false);
        Needs.gameObject.SetActive(true);
    }
    private void Awake()
    {
        instance = this;
    }

    void OnEnable()
    {
        Inventory.instance.onOpenInventory.AddListener(OnOpenInventory);
        Needs.gameObject.SetActive(false);
        AudioManager.Instance.PlayOneShot("Bag");
        AudioManager.Instance.Play("Boil",transform);
        
        if(sliderObject.activeInHierarchy)
            Reset();
        
        if (inventoryWindow.activeInHierarchy)
        {
            Inventory.instance.Toggle();
        }
    }

    public void OnDisable()
    {
        Inventory.instance.onOpenInventory.RemoveListener(OnOpenInventory);
        AudioManager.Instance.Stop("Boil");
        AudioManager.Instance.PlayOneShot("Bag");
        isCraftingInProgress = false;
    }

    public void OnOpenInventory()
    {
        Inventory.instance.onOpenInventory.RemoveListener(OnOpenInventory);
        gameObject.SetActive(false);
        ResetAll();
    }


    public void Craft(CraftingRecipe recipe)
    {
        if (isCraftingInProgress)
        {
            return;
        }
        
        Reset();
        StartCoroutine(CraftDelay());
        AudioManager.Instance.PlayOneShot("Craft");
        
        for (int i = 0; i < recipe.cost.Length; i++)
        {
            Inventory.instance.ForceRemoveItem(recipe.cost[i].item, recipe.cost[i].quantity);
        }

        Inventory.instance.AddItem(recipe.itemToCraft);
        img.sprite = recipe.itemToCraft.icon;

        for (int i = 0; i < recipeUIs.Length; i++)
        {
            recipeUIs[i].UpdateCanCraft();
        }
    }
    
    IEnumerator CraftDelay()
    {        
        isCraftingInProgress = true;
        sliderObject.SetActive(true);
        yield return new WaitForSeconds(FillTime);
        sliderObject.SetActive(false);
        craftedItemContainer.SetActive(true);
        confetti.Play();
        AudioManager.Instance.PlayOneShot("Success");
        yield return new WaitForSeconds(2);
        craftedItemContainer.SetActive(false);
        close();
        isCraftingInProgress = false;
    }
}
