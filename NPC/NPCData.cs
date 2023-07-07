using UnityEngine;

[CreateAssetMenu(fileName = "NPC Data", menuName = "New NPC Data")]

public class NPCData : ScriptableObject
{
    public string id;
    public GameObject SpawnPrefab;
}
