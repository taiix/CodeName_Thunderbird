using System;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class EnemyAI : MonoBehaviour
{

    //SERIALIZED
    [SerializeField] private TimeController timeController;


    //PUBLIC
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


    private bool returnToShelter = false;
    private bool isTransitioning = false;
    private bool isReturningToShelter = false;
    private bool isPatrolling = true;
    private bool isRetreating = false;
    private Vector3 shelterLocation;

    private Dictionary<EnemyType, IAttackStrategy> attackStrategies;

    TimeSpan morningTime = TimeSpan.FromHours(9.5f);
    TimeSpan eveningTime = TimeSpan.FromHours(20f);


    //ACTIONS
    public UnityAction<int> OnHealthChanged;
    void Start()
    {
        timeController = FindObjectOfType<TimeController>();
        player = FindObjectOfType<PlayerHealth>().gameObject.transform;
        shelterLocation = gameObject.transform.position;
        lightSource = timeController.Sun;
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        currentHealth = enemyData.health;
        //Debug.Log(enemyData.enemyName + " current health = " + currentHealth);

        attackStrategy = AttackStrategyFactory.GetStrategy(enemyData.enemyType);

        if (attackStrategy == null)
        {
            Debug.LogWarning("No attack strategy found for enemy type: " + enemyData.enemyType);
        }
        currentState = new PatrolState(this.gameObject, agent, anim, player);
        ChangeCurrentState(currentState);
    }

    public void ChangeCurrentState(State state)
    {
        if (isDead || currentState == state) return;
        isTransitioning = true;

        if (currentState != null)
        {
            currentState.Exit();
        }

        currentState = state;
        currentState.Enter();

        Debug.Log(enemyData.enemyName + " is in state: " + currentState.name);
        isTransitioning = false;
    }

    public bool IsInShadow()
    {
        return IsPointInShadow(transform.position);
    }

    public int GetHealth()
    {
        return currentHealth;
    }

    public State GetCurrentState()
    {
        return currentState;
    }

    public bool IsPointInShadow(Vector3 point)
    {
        Vector3 lightDirection = -lightSource.transform.forward;
        Vector3 adjustedPoint = point + Vector3.up * enemyHeight/2;

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
        //if (needsToRetreat) return;

        bool currentlyInShadow = IsInShadow();
        if (currentlyInShadow)
        {
            lastKnownShadowPosition = transform.position - lightSource.transform.forward * 2f;
            sunExposureTimer = 0f;
            isRetreating = false;
        }
        else
        {
            sunExposureTimer += Time.deltaTime;
            if (sunExposureTimer >= enemyData.timeInSun && isRetreating)
            {
                isRetreating = true;
                ChangeCurrentState(new ReturnToShadowState(this.gameObject, agent, anim, shelterLocation, player, lastKnownShadowPosition, isReturningToShelter));
            }
        }
    }
    
    private void CheckCurrentTime()
    {
        TimeSpan currentTime = timeController.CurrentTime.TimeOfDay;

        if (currentTime > morningTime && currentTime <= eveningTime && !isReturningToShelter)
        {
            Debug.Log("It's after 9:30 AM! Time to take action.");
            
            isReturningToShelter = true;
            isPatrolling = false;
            ChangeCurrentState(new ReturnToShelter(this.gameObject, agent, anim, player, shelterLocation));
        }
        if(currentTime > eveningTime && !isPatrolling)
        {
            ChangeCurrentState(new PatrolState(this.gameObject, agent, anim, player));
            isPatrolling = true;
            isReturningToShelter = false;
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
        try
        {
            State attackState = AttackStateFactory.GetAttackState(enemyData.enemyType, gameObject, agent, anim, player);
            ChangeCurrentState(attackState);
        }
        catch (System.ArgumentException ex)
        {
            Debug.LogError(ex.Message);
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

    private void FixedUpdate()
    {
        
        UpdateSunExposure();
        CheckCurrentTime();
        if (currentState != null)
        {
            currentState.Process();
        }
    }

    void Update()
    {
    }
}
