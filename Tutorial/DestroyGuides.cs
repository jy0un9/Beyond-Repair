
using UnityEngine;

public class DestroyGuides : MonoBehaviour
{
    public GameObject ItemToDestroy;
    //destroy itemToDestroy if player enters trigger
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Destroy(ItemToDestroy);
        }
    }
}
