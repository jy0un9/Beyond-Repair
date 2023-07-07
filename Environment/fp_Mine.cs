using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class fp_Mine : MonoBehaviour
{
    public ItemData itemToGive;
    public int capacity;
    public int quantityPerHit = 1;
    public GameObject hitParticle;
    public GameObject[] ore;
    private Queue<GameObject> oreQueue;
    ShakeHit shakeHit;
    public GameObject decal;
    private void Awake()
    {
        shakeHit = this.GetComponent<ShakeHit>();
        oreQueue = new Queue<GameObject>(ore);
    }

    public void Gather(Vector3 hitPoint, Vector3 hitNormal, int efficiency)
    {
        int orePerHit = quantityPerHit * efficiency;
        for (int i = 0; i < orePerHit; i++)
        {
            if (capacity > 0)
            {
                shakeHit.Begin();
                capacity -= 1;
                GameObject randomOre = oreQueue.Dequeue();
                Destroy(randomOre);
                Inventory.instance.AddItem(itemToGive);
            }
            else
            {
                Destroy(gameObject);
                break;
            }
        }
        
        GameObject decalInstance = Instantiate(decal, hitPoint , Quaternion.identity);
        decalInstance.transform.rotation = Quaternion.LookRotation(hitNormal);
        DecalProjector decalProjector = decalInstance.GetComponent<DecalProjector>();
        
        if(decalProjector != null)
        {
            StartCoroutine(FadeAndDestroy(decalProjector, 1.0f));
        }else
        {
            Destroy(decalInstance, 1.0f);
        }

        Destroy(Instantiate(hitParticle, hitPoint, Quaternion.LookRotation(hitPoint, Vector3.up)), 1.0f);
    }
    
    IEnumerator FadeAndDestroy(DecalProjector decalProjector, float delay) {
        
        float elapsedTime = 0f;
        float originalOpacity = decalProjector.fadeFactor;
        while (elapsedTime < delay) {
            elapsedTime += Time.deltaTime;
            decalProjector.fadeFactor = Mathf.Lerp(originalOpacity, 0f, elapsedTime / delay);
            yield return null;
        }
        Destroy(decalProjector.gameObject);
    }
}