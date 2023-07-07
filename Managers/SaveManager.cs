using System.Collections;
using UniStorm;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance;
    public GameObject SavePanel;

    private void Start()
    {
        if(SavePanel != null)
        {
            SavePanel.SetActive(false);
        }

        StartCoroutine(LoadGame());
    }

    IEnumerator LoadGame()
    {
        yield return new WaitForEndOfFrame();
        if(PlayerPrefs.HasKey("Save"))
        {
            Load();
        }
    }

    public void Save()
    {
        SaveData data = new SaveData();
        
        data.playerPosition = new SVec3(PlayerController.instance.transform.position);
        data.playerRotation = new SVec3(PlayerController.instance.transform.eulerAngles);
        data.playerLook = new SVec3(PlayerController.instance.cameraContainer.transform.eulerAngles);
        
        data.hunger = PlayerNeeds.instance.hunger.currentValue;
        data.thirst = PlayerNeeds.instance.thirst.currentValue;
        data.health = PlayerNeeds.instance.health.currentValue;
        data.sleep = PlayerNeeds.instance.sleep.currentValue;
        
        data.inventory = new SInventorySlot[Inventory.instance.slots.Length];
        for (int i = 0; i < Inventory.instance.slots.Length; i++)
        {
            data.inventory[i] = new SInventorySlot();
            data.inventory[i].occupied = Inventory.instance.slots[i].item != null;
            if (!data.inventory[i].occupied)
            {
                continue;
            }
            data.inventory[i].itemId = Inventory.instance.slots[i].item.id;
            data.inventory[i].quantity = Inventory.instance.slots[i].quantity;
            data.inventory[i].equipped = Inventory.instance.uiSlots[i].equipped;
        }
        
        ItemObject[] droppedItems = FindObjectsOfType<ItemObject>();
        data.droppedItems = new SDroppedItem[droppedItems.Length];
        for (int i = 0; i < droppedItems.Length; i++)
        {
            if (data.droppedItems[i].itemId != "Plant_Fiber")
            {
                data.droppedItems[i] = new SDroppedItem();
                data.droppedItems[i].itemId = droppedItems[i].item.id;
                data.droppedItems[i].position = new SVec3(droppedItems[i].transform.position);
                data.droppedItems[i].rotation = new SVec3(droppedItems[i].transform.eulerAngles);
            }
        }

        Buildings[] buildingObjects = FindObjectsOfType<Buildings>();
        data.buildings = new SBuilding[buildingObjects.Length];
        
        for (int i = 0; i < buildingObjects.Length; i++)
        {
            data.buildings[i] = new SBuilding();
            data.buildings[i].buildingId = buildingObjects[i].data.id;
            data.buildings[i].position = new SVec3(buildingObjects[i].transform.position);
            data.buildings[i].rotation = new SVec3(buildingObjects[i].transform.eulerAngles);
            data.buildings[i].customProperties = buildingObjects[i].GetCustomProperties();
        }
        
        data.resources = new SResource[ObjectManager.Instance.fpResources.Length];
        for (int i = 0; i < ObjectManager.Instance.fpResources.Length; i++)
        {
            data.resources[i] = new SResource();
            data.resources[i].index = i;
            data.resources[i].destroyed = ObjectManager.Instance.fpResources[i] == null;
            if (!data.resources[i].destroyed)
            {
                data.resources[i].capacity = ObjectManager.Instance.fpResources[i].capacity;
            }
        }
        
        data.fp_Mine = new Sfp_Mine[ObjectManager.Instance.fpMines.Length];
        for (int i = 0; i < ObjectManager.Instance.fpMines.Length; i++)
        {
            data.fp_Mine[i] = new Sfp_Mine();
            data.fp_Mine[i].index = i;
            data.fp_Mine[i].destroyed = ObjectManager.Instance.fpMines[i] == null;
            if (!data.fp_Mine[i].destroyed)
            {
                data.fp_Mine[i].capacity = ObjectManager.Instance.fpMines[i].capacity;
            }
        }
        
        data.fp_Stone = new Sfp_Stone[ObjectManager.Instance.fpStones.Length];
        for (int i = 0; i < ObjectManager.Instance.fpStones.Length; i++)
        {
            data.fp_Stone[i] = new Sfp_Stone();
            data.fp_Stone[i].index = i;
            data.fp_Stone[i].destroyed = ObjectManager.Instance.fpStones[i] == null;
            if (!data.fp_Stone[i].destroyed)
            {
                data.fp_Stone[i].capacity = ObjectManager.Instance.fpStones[i].capacity;
            }
        }
        
        NPC[] npcs = FindObjectsOfType<NPC>();
        data.npcs = new SNPC[npcs.Length];
        for (int i = 0; i < npcs.Length; i++)
        {
            data.npcs[i] = new SNPC();
            data.npcs[i].npcId = npcs[i].data.id;
            data.npcs[i].position = new SVec3(npcs[i].transform.position);
            data.npcs[i].rotation = new SVec3(npcs[i].transform.eulerAngles);
            data.npcs[i].aiState = (int)npcs[i].aiState;
            data.npcs[i].hasAgentDestination = !npcs[i].agent.isStopped;
            data.npcs[i].agentDestination = new SVec3(npcs[i].agent.destination);
        }
        
        data.timeOfDay = UniStormSystem.Instance.Hour;
        string rawData = JsonUtility.ToJson(data);
        PlayerPrefs.SetString("Save", rawData);
        
        if(SavePanel != null)
        {
            SavePanel.SetActive(true);
            StartCoroutine(SavePanelTimer());        
        }
    }
    IEnumerator SavePanelTimer()
    {
        yield return new WaitForSeconds(0.1f);
        SavePanel.SetActive(false);
    }
    public void Load()
    {
        if (!PlayerPrefs.HasKey("Save")) return;

        SaveData data = JsonUtility.FromJson<SaveData>(PlayerPrefs.GetString("Save"));
        if (data == null)
        {
            Debug.LogError("Failed to load save data. Save data might be corrupted.");
            return;
        }
        PlayerController.instance.transform.position = data.playerPosition.GetVector3();
        PlayerController.instance.transform.eulerAngles = data.playerRotation.GetVector3();
        PlayerController.instance.cameraContainer.transform.eulerAngles = data.playerLook.GetVector3();
        
        PlayerNeeds.instance.hunger.currentValue = data.hunger;
        PlayerNeeds.instance.thirst.currentValue = data.thirst;
        PlayerNeeds.instance.health.currentValue = data.health;
        PlayerNeeds.instance.sleep.currentValue = data.sleep;
        
        int equippedIndex = 999;
        
        for (int i = 0; i < data.inventory.Length; i++)
        {
            if (!data.inventory[i].occupied)
            {
                continue;
            }
            Inventory.instance.slots[i].item = ObjectManager.Instance.GetItemByID(data.inventory[i].itemId);
            Inventory.instance.slots[i].quantity = data.inventory[i].quantity;
            
            if (data.inventory[i].equipped)
            {
                equippedIndex = i;
            }
        }
        
        if (equippedIndex != 999)
        {
            Inventory.instance.SelectItem(equippedIndex);
            Inventory.instance.OnEquipButton();
        }
        
        ItemObject[] droppedItems = FindObjectsOfType<ItemObject>();
        
        for (int i = 0; i < droppedItems.Length; i++)
        {
            if (droppedItems[i].item.id != "Plant_Fiber")
            {
                Destroy(droppedItems[i].gameObject);
            }
        }
        
        for (int i = 0; i < data.droppedItems.Length; i++)
        {
            if (data.droppedItems[i].itemId != "Plant_Fiber")
            {
                GameObject prefab = ObjectManager.Instance.GetItemByID(data.droppedItems[i].itemId).dropPrefab;
                Instantiate(prefab, data.droppedItems[i].position.GetVector3(), Quaternion.Euler(data.droppedItems[i].rotation.GetVector3()));
            }
        }

        
        for (int i = 0; i < data.buildings.Length; i++)
        {
            GameObject prefab = ObjectManager.Instance.GetBuildingByID(data.buildings[i].buildingId).SpawnPrefab;
            GameObject building = Instantiate(prefab, data.buildings[i].position.GetVector3(), Quaternion.Euler(data.buildings[i].rotation.GetVector3()));
        }
        
        if (ObjectManager.Instance != null && ObjectManager.Instance.fpResources != null)
        {
            for (int i = 0; i < ObjectManager.Instance.fpResources.Length; i++)
            {
                if (data.resources[i].destroyed)
                { 
                    if(ObjectManager.Instance.fpResources[i] != null) 
                        Destroy(ObjectManager.Instance.fpResources[i].gameObject);
                    continue;              
                }
                ObjectManager.Instance.fpResources[i].capacity = data.resources[i].capacity;
            }
        }

        if (ObjectManager.Instance != null && ObjectManager.Instance.fpMines != null)
        {
            for (int i = 0; i < ObjectManager.Instance.fpMines.Length; i++)
            {
                if (data.fp_Mine[i].destroyed)
                { 
                    if(ObjectManager.Instance.fpMines[i] != null) 
                        Destroy(ObjectManager.Instance.fpMines[i].gameObject);
                    continue;              
                }

                ObjectManager.Instance.fpMines[i].capacity = data.fp_Mine[i].capacity;
            }
        }

        if (ObjectManager.Instance != null && ObjectManager.Instance.fpStones != null)
        {
            for (int i = 0; i < ObjectManager.Instance.fpStones.Length; i++)
            {
                if (data.fp_Stone[i].destroyed)
                { 
                    if(ObjectManager.Instance.fpStones[i] != null) 
                        Destroy(ObjectManager.Instance.fpStones[i].gameObject);
                    continue;              
                }

                ObjectManager.Instance.fpStones[i].capacity = data.fp_Stone[i].capacity;
            }
        }
        
        NPC[] npcs = FindObjectsOfType<NPC>();
        
        for (int i = 0; i < npcs.Length; i++)
        {
            Destroy(npcs[i].gameObject);
        }
        
        for (int i = 0; i < data.npcs.Length; i++)
        {
            GameObject prefab = ObjectManager.Instance.GetNPCByID(data.npcs[i].npcId).SpawnPrefab;
            GameObject npcObj = Instantiate(prefab, data.npcs[i].position.GetVector3(), Quaternion.Euler(data.npcs[i].rotation.GetVector3()));
            NPC npc = npcObj.GetComponent<NPC>();

            if (npc != null) {
                npc.aiState = (AIState)data.npcs[i].aiState;
                npc.agent.isStopped = !data.npcs[i].hasAgentDestination;

                if (!npc.agent.isStopped)
                {
                    npc.agent.SetDestination(data.npcs[i].agentDestination.GetVector3());
                }
            } else {
                Debug.LogError("NPC component not found on the instantiated object.");
            }
        }   

        UniStormManager.Instance.SetTime(Mathf.RoundToInt(data.timeOfDay), 0);
    }
}