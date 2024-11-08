using UnityEngine;
using UnityEngine.AI;


public class EnemyAI : MonoBehaviour
{
    NavMeshAgent agent;
    Animator anim;
    public Transform player;
    State currentState;

    public EnemyData enemyData;

    [SerializeField] private GameObject stonePrefab; // Prefab for the stone projectile
    [SerializeField] private float attackSpeed = 2f;
    [SerializeField] private float spottingRange = 15f; 
    [SerializeField] private float meleeRange = 2f; 
    [SerializeField] private float rangedAttackRange = 10f; 
    [SerializeField] private float tauntRange = 3f;

    [SerializeField] Light lightSource;

    private float sunExposureTimer = 0f;
    private bool needsToRetreat = false;

    private Vector3 lastKnownShadowPosition;
    [SerializeField]
    private float enemyHeight = 2.0f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        currentState = new PatrolState(this.gameObject, agent, anim, player);
        currentState.Enter();
    }

    public void ChangeCurrentState(State state)
    {
        if (currentState != null)
            currentState.Exit();
        currentState = state;
        currentState.Enter();
    }

    public bool IsInShadow()
    {
        return IsPointInShadow(transform.position);
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
            // Update the last known shadow position when the enemy is in shadow
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

    void Update()
    {
        UpdateSunExposure();



        if (currentState != null)
        {
            currentState.Process();
        }
        //Debug.Log(currentState.ToString());
    }
}
