using System;
using System.Xml;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class EnemyAI : MonoBehaviour
{

    //SERIALIZED
    [SerializeField] private TimeController timeController;


    //PUBLIC
    public Transform shelterLocation;
    public Transform player;
    public EnemyData enemyData;
    public bool isDead = false;

    public IAttackStrategy attackStrategy;

    //PRIVATE 
    private Light lightSource;
    private NavMeshAgent agent;
    private Animator anim;
    private float sunExposureTimer = 0f;
    private Vector3 lastKnownShadowPosition;
    private int currentHealth;
    private State currentState;
    private float enemyHeight = 2.0f;
    private bool needsToRetreat = false;
    private bool returnToShelter = false;

    TimeSpan morningTime = TimeSpan.FromHours(9.5f);
    TimeSpan eveningTime = TimeSpan.FromHours(20f);


    //ACTIONS
    public UnityAction<int> OnHealthChanged;
    void Start()
    {
        lightSource = timeController.Sun;
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        currentHealth = enemyData.health;
        //Debug.Log(enemyData.enemyName + " current health = " + currentHealth);
        currentState = new PatrolState(this.gameObject, agent, anim, player);
        currentState.Enter();

        if (enemyData.enemyType == EnemyType.Ranged)
        {
            attackStrategy = new RangeAttackStrategy();  
        }
        else if (enemyData.enemyType == EnemyType.Melee)
        {
            attackStrategy = new MeleeAttackStrategy();  
        }
    }

    public void ChangeCurrentState(State state)
    {
        if (isDead) return;

        if (currentState != null)
        {

            currentState.Exit();
            currentState = state;
            currentState.Enter();

            Debug.Log(enemyData.enemyName + " is in state: " + currentState.name);
        }
    }

    public bool IsInShadow()
    {
        return IsPointInShadow(transform.position);
    }

    public int GetHealth()
    {
        return currentHealth;
    }

    public bool NeedsToRetreat()
    {
        return needsToRetreat;
    }

    public bool IsPointInShadow(Vector3 point)
    {
        Vector3 lightDirection = -lightSource.transform.forward;
        Vector3 adjustedPoint = point + Vector3.up * enemyHeight;

        RaycastHit hit;
        if (Physics.Raycast(adjustedPoint, lightDirection, out hit))
        {
            if (hit.collider != null && hit.collider.gameObject != gameObject)
            {
                return true;
            }
        }
        return false;
    }

    private void UpdateSunExposure()
    {
        bool currentlyInShadow = IsInShadow();
        if (currentlyInShadow)
        {
            lastKnownShadowPosition = transform.position - lightSource.transform.forward * 2f;
            sunExposureTimer = 0f;
            needsToRetreat = false;
        }
        else
        {
            sunExposureTimer += Time.deltaTime;
            if (sunExposureTimer >= enemyData.timeInSun)
            {
                needsToRetreat = true;
            }
        }
        if (needsToRetreat && !(currentState is ReturnToShadowState))
        {
            ChangeCurrentState(new ReturnToShadowState(this.gameObject, agent, anim, shelterLocation, player, lastKnownShadowPosition, returnToShelter));
        }
    }
    
    private void CheckCurrentTime()
    {
        if (timeController.CurrentTime.TimeOfDay > morningTime && !returnToShelter)
        {
            returnToShelter = true;
            Debug.Log("It's after 9:30 AM! Time to take action.");
            ChangeCurrentState(new ReturnToShelter(this.gameObject, agent, anim, player, shelterLocation));
        }
        if(timeController.CurrentTime.TimeOfDay > eveningTime)
        {
            returnToShelter = false;
        }
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        OnHealthChanged?.Invoke(currentHealth);
        Debug.Log(enemyData.enemyName + " health = " + currentHealth);

        if (currentHealth <= 0)
        {
            ChangeCurrentState(new DeadState(gameObject, agent, anim, player));
            isDead = true;
        }
    }

    public void ChooseAttackState()
    {
        if (enemyData.enemyType == EnemyType.Ranged)
        {
            ChangeCurrentState(new RangeAttackState(this.gameObject, agent, anim, player));
        }
        else if (enemyData.enemyType == EnemyType.Melee)
        {
            ChangeCurrentState(new MeleeAttackState(this.gameObject, agent, anim, player));
        }
    }

    public void PerformAttack()
    {
        if (attackStrategy != null)
        {
            attackStrategy.PerformAttack(this); 
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (attackStrategy is MeleeAttackStrategy meleeAttack)
        {
            meleeAttack.OnTriggerEnter(other);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (attackStrategy is MeleeAttackStrategy meleeAttack)
        {
            meleeAttack.OnTriggerExit(other);
        }
    }

    void Update()
    {
        UpdateSunExposure();
        CheckCurrentTime();
        if (currentState != null)
        {
            currentState.Process();
        }
    }
}
