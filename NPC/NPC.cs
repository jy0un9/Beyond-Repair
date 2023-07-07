using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public enum AIType
{
    Passive,
    Scared,
    Aggressive
}

public enum AIState
{
    Idle,
    Wandering,
    Attacking,
    Fleeing
}

public class NPC : MonoBehaviour, IDamageble
{
    
    public NPCData data;
    public string enemyName;
    public GameObject Weapon;
    
    [Header("Stats")]
    public int health;
    public float walkSpeed;
    public float runSpeed;
    public ItemData[] dropOnDeath;
    
    [Header("Random Item Drops")]
    public ItemData[] randomDropItems;
    
    [Header("AI")]
    public AIType aiType;
    public AIState aiState;
    public float detectDistance;
    public float safeDistance;

    [Header("Wandering")]
    public float minWanderDistance;
    public float maxWanderDistance;
    public float minWanderWaitTime;
    public float maxWanderWaitTime;

    [Header("Combat")]
    public float attackRate;
    private float lastAttackTime;
    public float attackDistance;
    private float playerDistance;
    
    public NavMeshAgent agent;
    private Animator anim;
    private SkinnedMeshRenderer[] meshRenderers;
    public GameObject deathParticle;
    public GameObject spawnPoint;
    public float turnSpeed = 0.1f;
    public float AttackTurnSpeed = 0.5f;
    public bool isAttacking;

    private void Awake()
    {
        InitializeComponents();
    }
    
    private void InitializeComponents()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();
        meshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
    }
    private void Start()
    {
        SetState(AIState.Wandering);
        agent.stoppingDistance = 0.5f;
    }

    private void Update()
    {
        UpdateAnimatorState();

        if (!isAttacking)
        {
            UpdateAIState();
        }
    }
    public void PlayWhooshSound()
    {
        AudioManager.Instance.PlayOneShot("Whoosh");
    }
    private void UpdateAIState()
    {
        switch (aiState)
        {
            case AIState.Idle:
            case AIState.Wandering:
                PassiveUpdate();
                break;
            case AIState.Attacking:
                AttackingUpdate();
                break;
            case AIState.Fleeing:
                FleeingUpdate();
                break;
        }
    }
    
    private void UpdateAnimatorState()
    {
        playerDistance = Vector3.Distance(transform.position, PlayerController.instance.transform.position);
        if (enemyName == "Goblin")
            anim.SetBool("Moving", aiState != AIState.Idle);

        if (enemyName == "Goblin" && anim.GetBool("BetweenAttacks"))
        {
            agent.isStopped = false;
        }

        if (data.id is "Boar" or "Deer" && !IsWalkingOrRunning())
        {
            agent.isStopped = true;
        }
        else if(data.id is "Boar" or "Deer" && IsWalkingOrRunning())
        {
            agent.isStopped = false;
        }
    }
    
    public void EnableWeapon()
    {
        Weapon.GetComponent<BoxCollider>().enabled = true;
    }
    
    public void DisableWeapon()
    {
        Weapon.GetComponent<BoxCollider>().enabled = false;
    }
    
    public AIState CurrentAIState
    {
        get { return aiState; }
    }

    bool IsMoving()
    {
        return agent.velocity.sqrMagnitude > 0.01f;
    }

    private bool IsWalkingOrRunning()
    {
        
        return anim.GetBool("isWalking") || anim.GetBool("isRunning");
    }

    void PassiveUpdate()
    {
        if (aiState == AIState.Wandering)
        {
            bool moving = IsMoving();
            if (enemyName == "Goblin")
                anim.SetBool("Moving", moving);

            if (!moving && agent.remainingDistance < 0.1f)
            {
                
                SetState(AIState.Idle);
                Invoke("WanderToNewLocation", Random.Range(minWanderWaitTime, maxWanderWaitTime));
            }
        }

        if(aiType == AIType.Aggressive && playerDistance < detectDistance)
        {
            SetState(AIState.Attacking);
        }

        else if(aiType == AIType.Scared && playerDistance < detectDistance)
        {
            SetState(AIState.Fleeing);
            agent.SetDestination(GetFleeLocation());
        }
    }

    void AttackingUpdate()
    {
        if (playerDistance > safeDistance)
        {
            SetState(AIState.Wandering);
            return;
        }
        
        if(enemyName == "Goblin")
        {
            if(playerDistance > attackDistance)
            {
                anim.SetBool("BetweenAttacks", false);
                
                if(!anim.GetCurrentAnimatorStateInfo(0).IsName("Attack") && 
                   !anim.GetCurrentAnimatorStateInfo(0).IsName("Attack2") && 
                   !anim.GetCurrentAnimatorStateInfo(0).IsName("Attack3") &&
                   !anim.GetCurrentAnimatorStateInfo(0).IsName("Attack4"))
                {
                    anim.SetInteger("AttackIndex", 0);
                    anim.SetBool("Moving", true);
                    agent.isStopped = false;
                    Vector3 targetDirection = (PlayerController.instance.transform.position - transform.position).normalized;
                    Quaternion targetRotation = Quaternion.LookRotation(new Vector3(targetDirection.x, 0, targetDirection.z));
                    transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, turnSpeed);
                    agent.SetDestination(PlayerController.instance.transform.position);
                    isAttacking = false;
                }
            }
            else
            {
                if (Time.time - lastAttackTime > attackRate)
                {
                    lastAttackTime = Time.time;
                    agent.isStopped = true;
                    Vector3 targetDirection = (PlayerController.instance.transform.position - transform.position).normalized;
                    Quaternion targetRotation = Quaternion.LookRotation(new Vector3(targetDirection.x, 0, targetDirection.z));
                    transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, AttackTurnSpeed);
                    transform.LookAt(PlayerController.instance.transform);
                    int newAttackIndex = Random.Range(1, 6);
                    anim.SetInteger("AttackIndex", newAttackIndex);
                    isAttacking = true;
                }
                else
                {
                    anim.SetBool("BetweenAttacks", true);
                }
                
            }  
        }
        
        if (enemyName == "Ork")
        {
            if(playerDistance > attackDistance)
            {
                if(!anim.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
                {
                    anim.SetBool("Attack", false);
                    anim.SetBool("Moving", true);
                    agent.isStopped = false;
                    Vector3 targetDirection = (PlayerController.instance.transform.position - transform.position).normalized;
                    Quaternion targetRotation = Quaternion.LookRotation(new Vector3(targetDirection.x, 0, targetDirection.z));
                    transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, turnSpeed);
                    agent.SetDestination(PlayerController.instance.transform.position);
                    isAttacking = false;
                }
            }
            else
            {
                if(Time.time - lastAttackTime > attackRate)
                {
                    lastAttackTime = Time.time;
                    agent.isStopped = true;
                    anim.SetTrigger("Attack");
                    isAttacking = true;
                }
            }
        }
    }

    public void ResetIsAttacking()
    {
        isAttacking = false;
    }
    
    void FleeingUpdate()
    {
        if (playerDistance < safeDistance && agent.remainingDistance < agent.stoppingDistance)
        {
            agent.isStopped = true;
            if (data.id != "Goblin")
            {
                anim.SetBool("isRunning", false);
            }

            Invoke("SetNewFleeLocation", 1.0f);
        }
        else if (playerDistance > safeDistance)
        {
            SetState(AIState.Wandering);
        }
    }
    void SetNewFleeLocation()
    {
        agent.SetDestination(GetFleeLocation());
        agent.isStopped = false;

        if (data.id != "Goblin")
        {
            anim.SetBool("isRunning", true);
        }
    }
    
    void SetState(AIState newState)
    {
        aiState = newState;

        switch(aiState)
        {
            case AIState.Idle:
            {
                agent.speed = walkSpeed;
                agent.isStopped = true;
                if (data.id == "Boar")
                {
                    anim.SetBool("isRunning", false);
                    anim.SetBool("isWalking", false);
                    anim.SetBool("isLooking", true);
                }  
                if (data.id == "Deer")
                {
                    anim.SetBool("isRunning", false);
                    anim.SetBool("isWalking", false);
                    anim.SetBool("isEating", true);
                }    
                break;
            }
            case AIState.Wandering:
            {
                agent.speed = walkSpeed;
                agent.isStopped = false;
                if (data.id == "Boar")
                {
                    anim.SetBool("isRunning", false);
                    anim.SetBool("isLooking", false);
                    anim.SetBool("isWalking", true);
                }  
                if (data.id == "Deer")
                {
                    anim.SetBool("isRunning", false);
                    anim.SetBool("isEating", false);
                    anim.SetBool("isWalking", true);
                }                   

                break;
            }
            case AIState.Attacking:
            {
                agent.speed = runSpeed;
                agent.isStopped = false;
                break;
            }
            case AIState.Fleeing:
            {
                agent.speed = runSpeed;
                agent.stoppingDistance = 0.5f;
                agent.isStopped = false;
                if (data.id == "Boar")
                {
                    anim.SetBool("isWalking", false);
                    anim.SetBool("isLooking", false);
                    anim.SetBool("isRunning", true);
                }
                if (data.id == "Deer")
                {
                    anim.SetBool("isWalking", false);
                    anim.SetBool("isEating", false);
                    anim.SetBool("isRunning", true);
                }
                break;
            }
        }
    }

    void WanderToNewLocation()
    {
        if (aiState != AIState.Idle)
            return;
        SetState(AIState.Wandering);
        agent.SetDestination(GetWanderLocation());
    }

    Vector3 GetWanderLocation()
    {
        NavMeshHit hit;
        NavMesh.SamplePosition(transform.position + (Random.onUnitSphere * Random.Range(minWanderDistance, maxWanderDistance)), out hit, maxWanderDistance, NavMesh.AllAreas);
        int i = 0;
        while(Vector3.Distance(transform.position,hit.position) < detectDistance)
        {
            NavMesh.SamplePosition(transform.position + (Random.onUnitSphere * Random.Range(minWanderDistance, maxWanderDistance)), out hit, maxWanderDistance, NavMesh.AllAreas);

            i++;
            if (i == 30)
                break;
        }
        return hit.position;
    }

    Vector3 GetFleeLocation()
    {
        NavMeshHit hit;
        NavMesh.SamplePosition(transform.position + (Random.onUnitSphere * safeDistance), out hit, safeDistance, NavMesh.AllAreas);

        int i = 0;
        while(GetDestinationAngle(hit.position) > 90 || playerDistance < safeDistance)
        {
            NavMesh.SamplePosition(transform.position + (Random.onUnitSphere * safeDistance), out hit, safeDistance, NavMesh.AllAreas);

            i++;
            if (i == 30)
                break;
        }

        return hit.position;
    }

    float GetDestinationAngle(Vector3 targetPos)
    {
        return Vector3.Angle(transform.position - PlayerController.instance.transform.position, transform.position + targetPos);
    }
    
    public void TakePhysicalDamage(int damageAmount)
    {
        health -= damageAmount;

        if (health <= 0)
            Die();
 
        StartCoroutine(DamageFlash());
        
        if(aiType == AIType.Passive)
        {
            SetState(AIState.Fleeing);
        }
    }

    void Die()
    {
        for (int x = 0; x < dropOnDeath.Length; x++)
        {
            GameObject droppedItem = Instantiate(dropOnDeath[x].dropPrefab, transform.position, Quaternion.identity);
            if(droppedItem.GetComponent<FloatingObject>() != null)
                droppedItem.GetComponent<FloatingObject>().enabled = true;
            
        }

        if (randomDropItems.Length > 0)
        {
            int randomIndex = Random.Range(0, randomDropItems.Length);
            GameObject droppedItem = Instantiate(randomDropItems[randomIndex].dropPrefab, transform.position, Quaternion.identity);
            if(droppedItem.GetComponent<FloatingObject>() != null)
                droppedItem.GetComponent<FloatingObject>().enabled = true;

        }
        
        Destroy(Instantiate(deathParticle, spawnPoint.transform.position, Quaternion.identity), 3.0f);
        AudioManager.Instance.PlayOneShot("EnemyDeath");

        Destroy(gameObject);
    }
    
    IEnumerator DamageFlash()
    {
        for(int x = 0; x < meshRenderers.Length; x++)
        {
            meshRenderers[x].material.color = new Color(1.0f, 0.5f, 0.5f);
        }

        yield return new WaitForSeconds(0.1f);

        for (int x = 0; x < meshRenderers.Length; x++)
        {
            meshRenderers[x].material.color = Color.white;
        }
    }
}