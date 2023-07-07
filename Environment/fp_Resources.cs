using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class fp_Resources : MonoBehaviour
{
    public ItemData itemToGive;
    public int capacity;
    public int quantityPerHit = 1;
    public GameObject hitParticle;
    public GameObject hitParticleLeaves;
    public GameObject ItemToDestroy;
    public GameObject Log;
    public GameObject decal;
    ShakeHit shakeHit;
    
    private void Awake()
    {
        shakeHit = this.GetComponent<ShakeHit>();
    }

    public void Gather(Vector3 hitPoint, Vector3 hitNormal, int efficiency)
    {
        int logsPerHit = quantityPerHit * efficiency;
        for (int i = 0; i < logsPerHit; i++)
        {
            if(capacity <= 0)
            {
                break;
            }
            shakeHit.Begin();
            capacity -= 1;

            GameObject clone = Instantiate(Log, hitPoint + new Vector3(Random.Range(0.5f, -0.5f), 0, -0.25f), Quaternion.LookRotation(hitNormal, Vector3.up));
            clone.GetComponent<FloatingObject>().enabled = true;
            
            if (capacity<= 0)
            {
                Destroy(ItemToDestroy);
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

        Destroy(Instantiate(hitParticle, hitPoint, Quaternion.LookRotation(hitPoint, Vector3.up)), 1f);
        Destroy(Instantiate(hitParticleLeaves, hitPoint + new Vector3(0, 2, 0), hitParticleLeaves.transform.rotation), 2f);
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