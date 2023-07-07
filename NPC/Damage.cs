using UnityEngine;

public class Damage : MonoBehaviour, IDamageble
{
    private  GameObject player;
    public int damage;
    private float lastCollisionTime;

    private void Start()
    {
        player = GameObject.FindWithTag("Player");
    }

    private void OnTriggerEnter(Collider other)
    {
        if(player != null)
        {
            if(other.gameObject == player)
            {
                AudioManager.Instance.PlayOneShot("SwordHit");
                DamageTrigger();
                lastCollisionTime = Time.time;
            }
        }    
    }

    void DamageTrigger()
    {
        PlayerController.instance.GetComponent<IDamageble>().TakePhysicalDamage(damage);
    }

    public void TakePhysicalDamage(int damageAmount)
    {
        throw new System.NotImplementedException();
    }
}