using System.Collections;
using UnityEngine;

public class Chest : MonoBehaviour, IInteractable
{
    private Animator animator;
    public ParticleSystem confetti;
    public GameObject[] itemList;
    public Transform spawnPoint;
    public float force = 5f;
    private bool hasBeenOpened = false;
    public float maxDistance = 5f;
    public float delay = 0.5f;
    private bool allItemsSpawned = false;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public string GetInteractPrompt()
    {
        return animator.GetBool("Open") ? "Close Chest" : "Open Chest";
    }

    public void OnInteract() {
        if (!hasBeenOpened) {
            OpenBox();
        } 
        else if (allItemsSpawned) 
        {
            ToggleBox();
        }
    }

    private void OpenBox() {
        if (!animator.GetBool("Open")) {
            animator.SetBool("Open", true);
            AudioManager.Instance.PlayOneShot("Loot");
            confetti.Play();
            hasBeenOpened = true;
            StartCoroutine(SpawnDelay());
        }
    }

    private void ToggleBox() {
        animator.SetBool("Open", !animator.GetBool("Open"));
    }
    
    private IEnumerator SpawnDelay()
    {
        int toggle = 1;
        int numItemsToSpawn = 4;
        int[] itemIndices = GetRandomItemIndices(numItemsToSpawn);

        for (int i = 0; i < itemIndices.Length; i++)
        {
            yield return new WaitForSeconds(delay);
            AudioManager.Instance.PlayOneShot("Spawn");
            GameObject newItem = Instantiate(itemList[itemIndices[i]], spawnPoint.position, spawnPoint.rotation);
            Vector3 localForceVector = Vector3.up * force + new Vector3(0, 0, toggle) * force;
            Vector3 worldForceVector = transform.rotation * localForceVector;
            Vector3 clampedForceVector = Vector3.ClampMagnitude(worldForceVector, maxDistance);
            newItem.GetComponent<Rigidbody>().AddForce(clampedForceVector, ForceMode.Impulse);
            toggle *= -1;
        }
        allItemsSpawned = true;
    }

    private int[] GetRandomItemIndices(int numItemsToSelect)
    {
        int[] itemIndices = new int[numItemsToSelect];
        for (int i = 0; i < numItemsToSelect; i++)
        {
            int index;
            do
            {
                index = Random.Range(0, itemList.Length);
            } while (System.Array.IndexOf(itemIndices, index) >= 0);
            itemIndices[i] = index;
        }
        return itemIndices;
    }
}