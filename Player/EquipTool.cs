using UnityEngine;

public class EquipTool : Equip
{
    public float attackRate;
    public float attackDistance;
    private bool attacking;

    [Header("Combat")]
    public bool doesDealDamage;
    public int damage;

    [Header("Resource Gathering")]
    public bool doesGatherResources;
    public bool doesGatherStone;

    public bool doesGatherOre;
    [Header("Resource Gathering")]
    public int efficiency = 1;

    private Animator anim;
    private Camera cam;

    public bool IsAttacking
    {
        get { return attacking; }
    }

    private void Awake()
    {
        anim = GetComponent<Animator>();
        cam = Camera.main;
    }

    public override void OnAttackInput()
    {
        if (!attacking)
        {
            attacking = true;
            anim.SetTrigger("Attack");
            Invoke("OnCanAttack", attackRate);
        }
    }

    public void PlayWhooshSound()
    {
        AudioManager.Instance.PlayOneShot("Whoosh");
    }

    void OnCanAttack()
    {
        attacking = false;
    }
    public GameObject hitParticle;

    public void OnHit()
    {
        Ray ray = cam.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hit;

        if(Physics.Raycast(ray,out hit, attackDistance))
        {
            if(doesGatherResources && hit.collider.GetComponent<fp_Resources>())
            {
                hit.collider.GetComponent<fp_Resources>().Gather(hit.point, hit.normal, efficiency);
                AudioManager.Instance.PlayRandomOneShot("Chop", "Chop2");
            }
            
            if(doesGatherStone && hit.collider.GetComponent<fp_Stone>())
            {
                hit.collider.GetComponent<fp_Stone>().Gather(hit.point, hit.normal, efficiency);
                AudioManager.Instance.PlayRandomOneShot("Stone", "Stone2");
            }

            if (doesGatherOre && hit.collider.GetComponent<fp_Mine>())
            {
                hit.collider.GetComponent<fp_Mine>().Gather(hit.point, hit.normal, efficiency);
                AudioManager.Instance.PlayRandomOneShot("Mine", "Mine2");
            }

            if (doesDealDamage && hit.collider.GetComponent<IDamageble>() != null)
            {
                hit.collider.GetComponent<IDamageble>().TakePhysicalDamage(damage);
                Destroy(Instantiate(hitParticle, hit.point, Quaternion.LookRotation(hit.normal, Vector3.up)), 1.0f);
                AudioManager.Instance.PlayOneShot("SwordHit");

            }
        }
    }
}