using UnityEngine;
using UniStorm;

public class ParticleSystemManager : MonoBehaviour
{
    public GameObject player;
    public float spawnDistance = 10.0f;
    public int timeToSwitchNight = 19;
    public int timeToSwitchDay = 6;

    private GameObject daytimeParticleSystem;
    private GameObject nighttimeParticleSystem;
    private GameObject currentParticleSystem;
    private bool isParticleSystemEnabled = false;

    private void Start()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }

        if (transform.childCount >= 1)
        {
            daytimeParticleSystem = transform.GetChild(0).gameObject;

            if (transform.childCount >= 2)
            {
                nighttimeParticleSystem = transform.GetChild(1).gameObject;
            }
        }
        else
        {
            return;
        }

        if (daytimeParticleSystem == null)
        {
            return;
        }

        currentParticleSystem = daytimeParticleSystem;

        if (nighttimeParticleSystem != null)
        {
            nighttimeParticleSystem.SetActive(false);
        }
    }

    private void Update()
    {
        int currentHour = UniStormSystem.Instance.Hour;

        if (nighttimeParticleSystem != null)
        {
            if (currentHour >= timeToSwitchDay && currentHour < timeToSwitchNight && currentParticleSystem != daytimeParticleSystem)
            {
                daytimeParticleSystem.SetActive(true);
                nighttimeParticleSystem.SetActive(false);
                currentParticleSystem = daytimeParticleSystem;
                isParticleSystemEnabled = false;
            }
            else if (currentHour >= timeToSwitchNight && currentParticleSystem != nighttimeParticleSystem)
            {
                nighttimeParticleSystem.SetActive(true);
                daytimeParticleSystem.SetActive(false);
                currentParticleSystem = nighttimeParticleSystem;
                isParticleSystemEnabled = false;
            }
        }
        else if (currentHour >= timeToSwitchNight && currentParticleSystem != null)
        {
            currentParticleSystem.SetActive(false);
            currentParticleSystem = null;
        }
        else if (currentHour < timeToSwitchNight && currentHour >= timeToSwitchDay && currentParticleSystem == null)
        {
            currentParticleSystem = daytimeParticleSystem;
            isParticleSystemEnabled = false;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        if (currentParticleSystem != null)
        {
            if (distanceToPlayer <= spawnDistance && !isParticleSystemEnabled)
            {
                currentParticleSystem.SetActive(true);
                isParticleSystemEnabled = true;
            }
            else if (distanceToPlayer > spawnDistance && isParticleSystemEnabled)
            {
                currentParticleSystem.SetActive(false);
                isParticleSystemEnabled = false;
            }
        }
    }
}