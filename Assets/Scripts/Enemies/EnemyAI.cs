using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{


    NavMeshAgent agent;
    Animator anim;
    public Transform player;
    State currentState;

    private bool canPatrol = true;
    [SerializeField] private bool canAttack = false;
    [SerializeField] private bool canRunAway = false;
    [SerializeField] private bool canPursue;
    [SerializeField] private bool canRandomPatrol = false;

    [SerializeField] Light lightSource;
    bool isInShadow = false;


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
    public bool CanBeScared()
    {
        return canRunAway;
    }

    public bool CanPatrol()
    {
        return canPatrol;
    }



    public bool CanAttack()
    {
        return canAttack;
    }

    void TurnOnRandomPatrol()
    {
        canRandomPatrol = !canRandomPatrol;
    }

    private bool IsInShadow()
    {
        Vector3 lightDirection = -lightSource.transform.forward; // Negative because we want the direction the light is shining towards

        RaycastHit hit;

        // Cast the ray from the enemy's position in the direction opposite to the light
        if (Physics.Raycast(transform.position, lightDirection, out hit))
        {
            if (hit.collider != null && hit.collider.gameObject != gameObject) // Ensure it doesn't hit the enemy itself
            {
                isInShadow = true;
                Debug.Log("Enemy is in shadow");
            }
            else
            {
                isInShadow = false;
            }
        }

        return isInShadow;
    }

    public bool IsPointInShadow(Vector3 point)
    {
        // Use the light's forward direction for directional light
        Vector3 lightDirection = -lightSource.transform.forward;

        RaycastHit hit;
        // Cast the ray from the given point in the direction opposite to the light
        if (Physics.Raycast(point, lightDirection, out hit))
        {
            if (hit.collider != null && hit.collider.gameObject != gameObject)
            {
                return true; // Point is in shadow
            }
        }

        return false; // Point is not in shadow
    }

    // Update is called once per frame
    void Update()
    {
        IsInShadow();

        if (currentState != null)
        {
            currentState.Update();
        }
    }
}
