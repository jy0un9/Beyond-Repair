using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class FishController : MonoBehaviour
{
    private NavMeshAgent agent;
    private Renderer fishRenderer;
    private Color[] colors = new Color[]
    {
        Color.white,
        new Color(0.5f,1f,0f,1f),
        new Color(0.7f,1f,0f,1f),
        new Color(0f, 1f, 0.7f, 1f)
    };

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        fishRenderer = GetComponentInChildren<Renderer>();
        EnableDisable(false);

        NavAgentController parentScript = GetComponentInParent<NavAgentController>();
        if (parentScript != null)
        {
            parentScript.OnEnableDisable += EnableDisable;
        }
        gameObject.layer = LayerMask.NameToLayer("Interactable");

    }

    void OnDestroy()
    {
        NavAgentController parentScript = GetComponentInParent<NavAgentController>();
        if (parentScript != null)
        {
            parentScript.OnEnableDisable -= EnableDisable;
        }
    }

    private void EnableDisable(bool enable)
    {
        agent.enabled = enable;
        gameObject.SetActive(enable);

        if (enable)
        {
            RandomizeColor();
            RandomizeBaseOffset();
        }
    }

    private void RandomizeColor()
    {
        if (fishRenderer != null) 
        {
            int randomIndex = Random.Range(0, colors.Length);
            fishRenderer.material.SetColor("_BaseColor", colors[randomIndex]);
        }
    }

    private void RandomizeBaseOffset()
    {
        if (agent != null)
        {
            agent.baseOffset = Random.Range(1.1f, 2.7f);
        }
    }
}