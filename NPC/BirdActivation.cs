using UniStorm;
using UnityEngine;

public class BirdActivation : MonoBehaviour
{
    public float activationDistance = 50f;

    private GameObject player;
    private SeagulController birdFlyInCircle;
    private SkinnedMeshRenderer birdMeshRenderer;
    private float randomDisableTime;
    private float randomEnableTime;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        birdFlyInCircle = GetComponent<SeagulController>();
        birdMeshRenderer = transform.GetChild(0).GetComponent<SkinnedMeshRenderer>();
        randomDisableTime = Random.Range(19, 20);
        randomEnableTime = Random.Range(5.5f, 6.5f);
    }

    void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);

        if (distanceToPlayer <= activationDistance)
        {
            if (!birdFlyInCircle.enabled)
            {
                birdFlyInCircle.enabled = true;
                birdMeshRenderer.enabled = true;
            }
        }
        else
        {
            if (birdFlyInCircle.enabled)
            {
                birdFlyInCircle.enabled = false;
                birdMeshRenderer.enabled = false;
            }
        }

        float currentHour = UniStormSystem.Instance.Hour;

        if (currentHour >= randomEnableTime && currentHour < 6.5f && !birdMeshRenderer.enabled)
        {
            birdMeshRenderer.enabled = true;
        }
        else if (currentHour >= randomDisableTime && currentHour < 20 && birdMeshRenderer.enabled)
        {
            birdMeshRenderer.enabled = false;
        }
    }
}