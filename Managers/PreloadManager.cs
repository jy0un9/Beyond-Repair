using System.Collections;
using UnityEngine;

public class PreloadManager : MonoBehaviour
{
    public GameObject[] objectsToPreload;

    void Start()
    {
        StartCoroutine(PreloadAssets());
    }

    IEnumerator PreloadAssets()
    {
        foreach (GameObject obj in objectsToPreload)
        {
            GameObject temp = Instantiate(obj, Vector3.one * -1000, Quaternion.identity);
            yield return null;
            Destroy(temp);
        }
    }
}