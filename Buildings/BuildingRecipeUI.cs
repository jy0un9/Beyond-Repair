using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class BuildingRecipeUI : MonoBehaviour
{
    public BuildingRecipe recipe;
    public Image backGroundImage;
    public Image icon;
    public TextMeshProUGUI buildingName;
    public Image[] resourceCosts;

    public Color canBuildColour;
    public Color cannotBuildColour;
    private bool canBuild;
    public GameObject storageChest;
    
    void OnEnable()
    {
        UpdateCanBuild();
    }

    private void Start()
    {
        icon.sprite = recipe.icon;
        buildingName.text = recipe.displayName;

        for (int x = 0; x < resourceCosts.Length; x++)
        {
            if (x < recipe.cost.Length)
            {
                resourceCosts[x].gameObject.SetActive(true);
                resourceCosts[x].sprite = recipe.cost[x].item.icon;
                resourceCosts[x].transform.GetComponentInChildren<TextMeshProUGUI>().text = recipe.cost[x].quantity.ToString();
            }
            else
            {
                resourceCosts[x].gameObject.SetActive(false);
            }
        }
    }

    void UpdateCanBuild()
    {
        canBuild = true;
        
        for (int x = 0; x < recipe.cost.Length; x++)
        {
            if (!Inventory.instance.HasItems(recipe.cost[x].item, recipe.cost[x].quantity))
            {
                canBuild = false;
                break;
            }
        }
        backGroundImage.color = canBuild ? canBuildColour : cannotBuildColour;
    }

    public void onClickButton()
    {
        if(canBuild)
        {
            EquipBuildingKit.instance.SetNewBuildingRecipe(recipe);
            AudioManager.Instance.PlayOneShot("Craft");
            
            if (recipe.displayName == "Chest")
            {
                Destroy(storageChest);
            }
        }
        else
        {
            PlayerController.instance.ToggleCurser(true);
        }
    }
}
