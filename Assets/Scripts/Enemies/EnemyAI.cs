using System;
using System.Xml;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class EnemyAI : MonoBehaviour
{

    //SERIALIZED
    [SerializeField] private Light lightSource;

    //PUBLIC
    public Transform player;
    public EnemyData enemyData;
    public bool isDead = false;

    //PRIVATE 
    private NavMeshAgent agent;
    private Animator anim;
    private float sunExposureTimer = 0f;
    private bool needsToRetreat = false;
    private Vector3 lastKnownShadowPosition;
    private int currentHealth;
    private State currentState;
    private float enemyHeight = 2.0f;

    //ACTIONS
    public UnityAction<int> OnHealthChanged;
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        currentState = new PatrolState(this.gameObject, agent, anim, player);
        currentState.Enter();
    }

    public void ChangeCurrentState(State state)
    {
        if (isDead) return;

        if (currentState != null)
            currentState.Exit();
        currentState = state;
        currentState.Enter();
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
            ChangeCurrentState(new ReturnToShadowState(this.gameObject, agent, anim, player, lastKnownShadowPosition));
        }
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        OnHealthChanged?.Invoke(currentHealth);

        if (currentHealth <= 0)
        {
            ChangeCurrentState(new DeadState(gameObject, agent, anim, player));
            isDead = true;
        }
    }

    void Update()
    {
        UpdateSunExposure();
        if (currentState != null)
        {
            currentState.Process();
        }
        //Debug.Log(currentState.name);
    }
}
