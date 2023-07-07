using UnityEngine;

public class ObjectManager : MonoBehaviour
{
    [HideInInspector]
    public ItemData[] items;
    [HideInInspector]
    public fp_Resources[] fpResources;
    [HideInInspector]
    public fp_Mine[] fpMines;
    [HideInInspector]
    public fp_Stone[] fpStones;
    [HideInInspector]
    public BuildingData[] building;
    [HideInInspector]
    public NPCData[] npcs;
    
    public static ObjectManager Instance;

    private void Awake()
    {
        Instance = this;
        items = Resources.LoadAll<ItemData>("Items");        
        building = Resources.LoadAll<BuildingData>("Buildings");        
        npcs = Resources.LoadAll<NPCData>("NPCs");
    }

    public void Start()
    {
        fpResources = FindObjectsOfType<fp_Resources>();
        fpMines = FindObjectsOfType<fp_Mine>();
        fpStones = FindObjectsOfType<fp_Stone>();
    }
    
    public ItemData GetItemByID(string id)
    {
        for (int x = 0; x < items.Length; x++)
        {
            if (items[x].id == id)
            {
                return items[x];
            }
        }
        return null;
    }
    
    public BuildingData GetBuildingByID(string id)
    {
        for (int x = 0; x < building.Length; x++)
        {
            if (building[x].id == id)
            {
                return building[x];
            }
        }
        return null;
    }
    
    public NPCData GetNPCByID(string id)
    {
        for (int x = 0; x < npcs.Length; x++)
        {
            if (npcs[x].id == id)
            {
                return npcs[x];
            }
        }
        return null;
    }
}