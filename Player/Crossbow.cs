using UnityEngine;

public class Crossbow : MonoBehaviour
{
    public GameObject arrowPrefab;
    public Transform  arrowSpawnPoint;
    private Transform  arrowSpawnPointUpdate;
    public float arrowSpeed = 20f;
    public float arrowReloadTime = 1f;
    public float arrowReloadTimer = 0f;
    public bool arrowReloading = false;
    public bool fire = false;
    public bool hasAmmo = false;
    public ItemObject arrowData;
    public bool loadAnimationFinished = false;

    public void PlayDrawBowSound()
    {
        AudioManager.Instance.PlayOneShot("DrawBow");
    }
    
    private bool IsLoadAnimationPlaying()
    {
        return GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Load");
    }
    
    public void OnLoadAnimationFinished()
    {
        loadAnimationFinished = true;
    }

    private void UpdateAmmoStatus()
    {
        hasAmmo = false;
        foreach (var item in Inventory.instance.slots)
        {
            if (item.item == arrowData.item && item.quantity > 0)
            {
                hasAmmo = true;
                break;
            }
        }
    }

    void Update()
    {
        fire = PlayerController.instance.fire;
        arrowSpawnPointUpdate = arrowSpawnPoint;
        UpdateAmmoStatus();
        GetComponent<Animator>().SetBool("HasAmmo", hasAmmo);
    
        if (arrowReloading)
        {
            arrowReloadTimer += Time.deltaTime;
            if (arrowReloadTimer >= arrowReloadTime)
            {
                arrowReloading = false;
                arrowReloadTimer = 0f;
            }
        }
    
        if (hasAmmo && !arrowReloading && !IsLoadAnimationPlaying())
        {
            GetComponent<Animator>().SetTrigger("Load");
        }
    
        bool fireAnimationNotPlaying = !GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Fire");

        if (fire && !arrowReloading && !IsLoadAnimationPlaying() && loadAnimationFinished)
        {
            if (!arrowReloading)
            {
                bool arrowFired = false;
                foreach (var item in Inventory.instance.slots)
                {
                    if (item.item == arrowData.item && item.quantity > 0 && !arrowFired)
                    {
                        arrowReloading = true;
                        GetComponent<Animator>().SetTrigger("Fire");
                        GameObject arrow = Instantiate(arrowPrefab, arrowSpawnPointUpdate.position, arrowSpawnPointUpdate.rotation);
                        arrow.GetComponent<Rigidbody>().AddForce(arrowSpawnPointUpdate.forward * arrowSpeed, ForceMode.Impulse);
                        arrow.AddComponent<Arrow>();
                        PlayerController.instance.fire = false;    
                        Inventory.instance.RemoveItem(arrowData.item);
                        Inventory.instance.UpdateUI();
                        AudioManager.Instance.PlayOneShot("FireBow");
                        loadAnimationFinished = false;
                        arrowFired = true;
                    }

                    if (item.item == arrowData.item && item.quantity == 0)
                    {
                        item.item = null;
                        item.quantity = 0;
                        Inventory.instance.UpdateUI();
                        PlayerController.instance.fire = false;
                        Inventory.instance.UpdateUI();
                    }
                }
            }
        }
    }


    public class Arrow : MonoBehaviour
    {
        private float destroyTimer = 0f;
        private bool collided = false;

        private void Start()
        {
            destroyTimer = Time.time + 3f;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("Head"))
            {
                collision.gameObject.GetComponentInParent<IDamageble>().TakePhysicalDamage(100);
                Destroy(gameObject);
            }
            
            if (collision.gameObject.CompareTag("Enemy"))
            {
                collision.gameObject.GetComponent<IDamageble>().TakePhysicalDamage(50);
                Destroy(gameObject);
            }              
            
            if (collision.gameObject.layer == 7)
            {
                Destroy(gameObject);
            }       
            
            transform.parent = collision.transform;
            GetComponent<Rigidbody>().isKinematic = true;
            GetComponent<Collider>().enabled = false;

            transform.GetChild(0).gameObject.SetActive(false);
            transform.GetChild(1).gameObject.GetComponent<ParticleSystem>().Play();

            if (collision.gameObject.GetComponent<ShakeHit>() != null)
                collision.gameObject.GetComponent<ShakeHit>().Begin();

            gameObject.AddComponent<ShakeHit>();
            gameObject.GetComponent<ShakeHit>().shakeDuration = 0.8f;
            gameObject.GetComponent<ShakeHit>().Begin();
            collided = true;
        }

        private void Update()
        {
            if (!collided && Time.time >= destroyTimer)
            {
                Destroy(gameObject);
            }
        }
    }
}