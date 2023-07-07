using UnityEngine;

public class NavAgentController : MonoBehaviour
{
    public float activationDistance = 10f;
    private SphereCollider triggerCollider;

    public delegate void EnableDisableAction(bool enable);
    public event EnableDisableAction OnEnableDisable;

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        triggerCollider = gameObject.AddComponent<SphereCollider>();
        triggerCollider.isTrigger = true;
        triggerCollider.radius = activationDistance;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            OnEnableDisable?.Invoke(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            OnEnableDisable?.Invoke(false);
        }
    }
}