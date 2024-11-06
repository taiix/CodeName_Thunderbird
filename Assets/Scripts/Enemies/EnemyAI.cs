using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{


    NavMeshAgent agent;
    Animator anim;
    public Transform player;
    State currentState;

    private bool canPatrol = true;

    [SerializeField] private GameObject stonePrefab; // Prefab for the stone projectile
    [SerializeField] private float attackSpeed = 2f;
    [SerializeField] private float spottingRange = 15f; 
    [SerializeField] private float meleeRange = 2f; 
    [SerializeField] private float rangedAttackRange = 10f; 
    [SerializeField] private float tauntRange = 3f;

    [SerializeField] Light lightSource;
    bool isInShadow = false;

    [SerializeField]
    private float enemyHeight = 2.0f;
    void Start()
    {
       
       

    }

    private void OnEnable()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        Debug.Log("Enemy is enabled");
        currentState = new PatrolState(this.gameObject, agent, anim, player);
        currentState.Enter();
    }

    public void ChangeCurrentState(State state)
    {
        currentState.Exit();
        currentState = state;
        currentState.Enter();
    }
    public float SpottingRange()
    {
        return spottingRange;
    }

    public float MeleeRange()
    {
        return meleeRange;
    }

    public float RangedAttackRange()
    {
        return rangedAttackRange;
    }

    public float TauntRange()
    {
        return tauntRange;
    }

    public bool IsInShadow()
    {
        return IsPointInShadow(transform.position);
    }

    public GameObject StonePrefab()
    {
        return stonePrefab;
    }

    public float AttackSpeed()
    {
        return attackSpeed;
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

    void Update()
    {
        IsInShadow();
        if (currentState != null)
        {
            currentState.Update();
        }
    }
}
