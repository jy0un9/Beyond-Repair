using UnityEngine;

[System.Serializable]
public class SaveData
{
    public SVec3 playerPosition;
    public SVec3 playerRotation;
    public SVec3 playerLook;
    public float hunger;
    public float thirst;
    public float health;
    public float sleep;
    public SInventorySlot[] inventory;
    public SDroppedItem[] droppedItems;
    public SBuilding[] buildings;
    public SResource[] resources;
    public Sfp_Mine[] fp_Mine;
    public Sfp_Stone[] fp_Stone;
    public float timeOfDay;
    public SNPC[] npcs;
    
}
[System.Serializable]
public struct SVec3
{
    public float x;
    public float y;
    public float z;

    public SVec3(Vector3 vector)
    {
        x = vector.x;
        y = vector.y;
        z = vector.z;
    }
    
    public Vector3 GetVector3()
    {
        return new Vector3(x, y, z);
    }
}
[System.Serializable]
public struct SInventorySlot
{
    public bool occupied;
    public string itemId;
    public int quantity;
    public bool equipped;
}

[System.Serializable]
public struct SDroppedItem
{
    public string itemId;
    public SVec3 position;
    public SVec3 rotation;
}

[System.Serializable]
public struct SBuilding
{
    public string buildingId;
    public SVec3 position;
    public SVec3 rotation;
    public string customProperties;
}

[System.Serializable]
public struct SResource
{
    public int index;
    public bool destroyed;
    public int capacity;
}

[System.Serializable]
public struct Sfp_Mine
{
    public int index;
    public bool destroyed;
    public int capacity;
}

[System.Serializable]
public struct Sfp_Stone
{
    public int index;
    public bool destroyed;
    public int capacity;
}

[System.Serializable]
public struct SNPC
{
    public string npcId;
    public SVec3 position;
    public SVec3 rotation;
    public int aiState;
    public bool hasAgentDestination;
    public SVec3 agentDestination;
}