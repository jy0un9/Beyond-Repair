using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class RemoveGrassUnder : MonoBehaviour
{
    private Terrain terrain;
    public int detailPrototypeIndex = 0;
    public float grassRemovalRadius = 1.0f;
    private int[,] originalGrassData;
    private int xStart, zStart, xEnd, zEnd;

    private void OnEnable() => StartCoroutine(FindTerrainCoroutine());

    private IEnumerator FindTerrainCoroutine()
    {
        while (terrain == null || !terrain.isActiveAndEnabled)
        {
            FindTerrain();
            yield return new WaitForSeconds(0.1f);
        }
        StoreGrassData();
        RemoveGrass();
    }

    private void FindTerrain()
    {
        terrain = TerrainManager.Instance?.terrain;
    }

    private void StoreGrassData()
    {
        if (terrain == null) { Debug.LogError("Terrain not assigned."); return; }

        TerrainData terrainData = terrain.terrainData;
        Vector3 bedPosition = transform.position;
        Vector3 terrainPos = terrain.transform.position;

        int x = (int)((bedPosition.x - terrainPos.x) / terrainData.size.x * terrainData.detailWidth);
        int z = (int)((bedPosition.z - terrainPos.z) / terrainData.size.z * terrainData.detailHeight);
        int removalRadiusInData = Mathf.RoundToInt(grassRemovalRadius / terrainData.size.x * terrainData.detailWidth);

        xStart = Mathf.Clamp(x - removalRadiusInData, 0, terrainData.detailWidth);
        xEnd = Mathf.Clamp(x + removalRadiusInData, 0, terrainData.detailWidth);
        zStart = Mathf.Clamp(z - removalRadiusInData, 0, terrainData.detailHeight);
        zEnd = Mathf.Clamp(z + removalRadiusInData, 0, terrainData.detailHeight);

        originalGrassData = terrainData.GetDetailLayer(xStart, zStart, xEnd - xStart, zEnd - zStart, detailPrototypeIndex);
    }

    private void OnDestroy() => RestoreGrass();

    private void OnApplicationQuit()
    {
        RestoreGrass();
    }

    private void RestoreGrass()
    {
        if (terrain != null)
            terrain.terrainData.SetDetailLayer(xStart, zStart, detailPrototypeIndex, originalGrassData);
    }

    private void RemoveGrass()
    {
        if (terrain == null) { Debug.LogError("Terrain not assigned."); return; }

        TerrainData terrainData = terrain.terrainData;
        Vector3 bedPosition = transform.position;
        Vector3 terrainPos = terrain.transform.position;

        int x = (int)((bedPosition.x - terrainPos.x) / terrainData.size.x * terrainData.detailWidth);
        int z = (int)((bedPosition.z - terrainPos.z) / terrainData.size.z * terrainData.detailHeight);
        int removalRadiusInData = Mathf.RoundToInt(grassRemovalRadius / terrainData.size.x * terrainData.detailWidth);

        int xStart = Mathf.Clamp(x - removalRadiusInData, 0, terrainData.detailWidth);
        int xEnd = Mathf.Clamp(x + removalRadiusInData, 0, terrainData.detailWidth);
        int zStart = Mathf.Clamp(z - removalRadiusInData, 0, terrainData.detailHeight);
        int zEnd = Mathf.Clamp(z + removalRadiusInData, 0, terrainData.detailHeight);

        DetailPrototype detailPrototype = terrainData.detailPrototypes[detailPrototypeIndex];

        if (detailPrototype.usePrototypeMesh)
        {
            List<int[,]> detailLayers = new List<int[,]>();
            for (int i = 0; i < terrainData.detailPrototypes.Length; i++)
            {
                detailLayers.Add(terrainData.GetDetailLayer(xStart, zStart, xEnd - xStart, zEnd - zStart, i));
            }

            for (int i = 0; i < detailLayers[detailPrototypeIndex].GetLength(0); i++)
            {
                for (int j = 0; j < detailLayers[detailPrototypeIndex].GetLength(1); j++)
                {
                    detailLayers[detailPrototypeIndex][i, j] = 0;
                }
            }

            for (int i = 0; i < terrainData.detailPrototypes.Length; i++)
            {
                terrainData.SetDetailLayer(xStart, zStart, i, detailLayers[i]);
            }
        }
    }
}