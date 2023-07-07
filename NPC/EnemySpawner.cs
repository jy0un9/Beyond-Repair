using System.Collections.Generic;
using UnityEngine;
using UniStorm;

public class EnemySpawner : MonoBehaviour
{
    public List<Transform> spawnLocations;
    public List<GameObject> enemyPrefabs;
    public float spawnHour = 23;
    public float destroyHour = 6;
    private List<KeyValuePair<GameObject, int>> spawnedEnemies = new List<KeyValuePair<GameObject, int>>();
    private List<int> spawnedEnemiesPerSpawnPoint = new List<int>();

    private void Start()
    {
        for (int i = 0; i < spawnLocations.Count; i++)
        {
            spawnedEnemiesPerSpawnPoint.Add(0);
        }
    }

    private void Update()
    {
        float currentHour = UniStormSystem.Instance.Hour;

        if (currentHour >= spawnHour || currentHour < destroyHour)
        {
            CheckSpawnPoints();
        }
        else if (currentHour >= destroyHour)
        {
            DestroyNonAttackingEnemies();
        }
    }

    private void CheckSpawnPoints()
    {
        for (int i = 0; i < spawnLocations.Count; i++)
        {
            SpawnPoint spawnPoint = spawnLocations[i].GetComponent<SpawnPoint>();
            if (spawnPoint.playerIsWithinSpawnRange && spawnedEnemiesPerSpawnPoint[i] == 0)
            {
                SpawnEnemy(spawnLocations[i], i);
            }
        }
    }

    private void SpawnEnemy(Transform spawnLocation, int spawnPointIndex)
    {
        GameObject enemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Count)];
        GameObject enemy = Instantiate(enemyPrefab, spawnLocation.position, spawnLocation.rotation);

        spawnedEnemies.Add(new KeyValuePair<GameObject, int>(enemy, spawnPointIndex));
        spawnedEnemiesPerSpawnPoint[spawnPointIndex]++;
    }
    
    private void DestroyNonAttackingEnemies()
    {
        for (int i = spawnedEnemies.Count - 1; i >= 0; i--)
        {
            if (spawnedEnemies[i].Key != null)
            {
                NPC npc = spawnedEnemies[i].Key.GetComponent<NPC>();

                if (npc.CurrentAIState != AIState.Attacking)
                {
                    GameObject enemyToDestroy = spawnedEnemies[i].Key;
                    int spawnPointIndex = spawnedEnemies[i].Value;

                    spawnedEnemies.RemoveAt(i);
                    spawnedEnemiesPerSpawnPoint[spawnPointIndex]--;

                    Destroy(enemyToDestroy);
                }
            }
            else
            {
                spawnedEnemies.RemoveAt(i);
            }
        }
    }
    
    private int GetSpawnPointIndex(Vector3 enemyPosition)
    {
        for (int i = 0; i < spawnLocations.Count; i++)
        {
            if (Vector3.Distance(enemyPosition, spawnLocations[i].position) < 5.0f)
            {
                return i;
            }
        }
    
        return -1;
    }
}
