using UnityEngine;

[CreateAssetMenu(fileName ="Crafting Recipe", menuName = "New Crafting Recipe")]

public class CraftingRecipe : ScriptableObject
{
    public ItemData itemToCraft;
    public ResourceCost[] cost;
    public int craftQuantity = 1;
}

[System.Serializable]

public class ResourceCost
{
    public ItemData item;
    public int quantity;
}
