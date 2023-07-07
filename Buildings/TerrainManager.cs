using UnityEngine;

public class TerrainManager : MonoBehaviour
{
    public static TerrainManager Instance;

    public Terrain terrain;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

}