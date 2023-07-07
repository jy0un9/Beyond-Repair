using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CampFire : Buildings, IInteractable
{
    public GameObject particle;
    public GameObject particle2;
    new public GameObject light;
    private bool isOn;
    private Vector3 lightStartPos;
    public int damage;
    public float damageRate;
    public bool FireAudioStarted;
    private List<IDamageble> thingsThatDoDamage = new List<IDamageble>();
    
    private void Start()
    {
        lightStartPos = light.transform.localPosition;
        StartCoroutine(DealDamage());
    }

    IEnumerator DealDamage()
    {
        while(true)
        {
            if(isOn)
            {
                for(int x = 0; x < thingsThatDoDamage.Count; x++)
                {
                    thingsThatDoDamage[x].TakePhysicalDamage(damage);
                }
            }
            yield return new WaitForSeconds(damageRate);
        }
    }

    public string GetInteractPrompt()
    {
        return isOn ? "Turn Off" : "Turn On";
    }

    public void OnInteract()
    {
        isOn = !isOn;
        particle.SetActive(isOn);
        particle2.SetActive(isOn);
        light.SetActive(isOn);
    }

    private void Update()
    {
        if(isOn)
        {
            float x = Mathf.PerlinNoise(Time.time * 3.0f, 0.0f) / 5.0f;
            float z = Mathf.PerlinNoise(0.0f, Time.time * 3.0f) / 5.0f;

            light.transform.localPosition = lightStartPos + new Vector3(x, 0.0f, z);
            if(FireAudioStarted == false)
            {
                FireAudioStarted = true;
                FireAudio();
            }
        }
        else
        {
            FireAudioStarted = false;
            Transform parentTransform = transform;

            for (int i = 0; i < parentTransform.childCount; i++)
            {
                Destroy(parentTransform.GetChild(i).gameObject);
            }            
        }
    }

    private void FireAudio()
    {
        AudioManager.Instance.Play("Fire", transform);
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.GetComponent<IDamageble>() != null)
        {
            thingsThatDoDamage.Add(other.gameObject.GetComponent<IDamageble>());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<IDamageble>() != null)
        {
            thingsThatDoDamage.Remove(other.gameObject.GetComponent<IDamageble>());
        }
    }
    
    public override string GetCustomProperties()
    {
        return isOn.ToString();
    }
    
    public override void RecieveCustomProperties(string props)
    {
        isOn = props == "True" ? true : false;
        
        particle.SetActive(isOn);
        particle2.SetActive(isOn);
        light.SetActive(isOn);
    }
}