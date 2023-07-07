using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    public bool playerIsWithinSpawnRange;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsWithinSpawnRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsWithinSpawnRange = false;
        }
    }
}