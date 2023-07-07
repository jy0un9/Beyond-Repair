using UnityEngine;

public enum ItemType
{
    Resource,
    Equipable,
    Consumable
}

public enum ConsumableType
{
    Hunger,
    Thirst,
    Health,
    Sleep
}

[CreateAssetMenu(fileName = "Item", menuName = "New Item")]
public class ItemData : ScriptableObject
{
    public string id;
    [Header("Information")]
    public string displayName;
    public string description;
    public ItemType type;
    public Sprite icon;
    public GameObject dropPrefab;

    [Header("Stacking Items")]
    public bool canStackItem;
    public int maxStackAmount;

    [Header("Consumable")]
    public ItemDataConsumable[] consumables;

    [Header("Equip Items")]
    public GameObject equipPrefabs;

}

[System.Serializable]


public class ItemDataConsumable
{
    public ConsumableType type;
    public float Value;
}