using UnityEngine;

public class TutorialTriggerZone : MonoBehaviour
{
    [SerializeField] private TutorialManager.TriggerZone zone;

    private void OnTriggerEnter(Collider other)
    {
        TutorialManager.Instance.CheckForTrigger(other, zone);
    }
}