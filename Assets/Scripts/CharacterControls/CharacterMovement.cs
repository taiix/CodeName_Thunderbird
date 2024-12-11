using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterMovement : MonoBehaviour
{
    public float sensitivity;
    public float maxForce;

    private Rigidbody rb;
    private float speed;

    public float sprintSpeed;
    public float normalSpeed;

    private Vector2 move;
    private Vector2 look;
    private float lookRotation;

    private PlayerInput playerInput;
    private InputActionMap player;
    private InputAction jumpAction;
    private InputAction sprintAction;
    [SerializeField] private float groundCheckDistance = 0.1f;

    [SerializeField] private float maxSlopeAngle = 30f;

    [SerializeField] int jumpForce = 2;
    private bool isGrounded;
    private bool isSprinting;


    private bool activateControls = true;

    //public static event Action OnDisableControls;
    //public static event Action OnEnableControls;

    bool isMoving;
    public InputActionMap PlayerAction
    {
        get
        {
            // Ensure PlayerAction is initialized before accessing it.
            if (player == null && playerInput != null)
            {
                player = playerInput.currentActionMap;
            }
            return player;
        }
    }

    private void Awake()
    {
        playerInput = this.GetComponent<PlayerInput>();
        player = playerInput.currentActionMap;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        speed = normalSpeed;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void OnEnable()
    {
        jumpAction = player.FindAction("Jump");
        jumpAction.performed += Jump;
        jumpAction.Enable();


        sprintAction = player.FindAction("Sprint");
        sprintAction.performed += StartSprinting;
        sprintAction.canceled += StopSprinting;
        sprintAction.Enable();
        //OnEnableControls += EnableControls;
        //OnDisableControls += DisableControls;


        DialogueManager.OnDialogueStarted += DisableControls;
        DialogueManager.OnDialogueEnded += EnableControls;

    }

    private void OnDisable()
    {
        jumpAction.Disable();

        jumpAction.performed -= Jump;

        sprintAction.Disable();
        sprintAction.performed -= StartSprinting;
        sprintAction.canceled -= StopSprinting;


        //OnDisableControls -= DisableControls;
        //OnEnableControls -= EnableControls;

        DialogueManager.OnDialogueStarted -= DisableControls;
        DialogueManager.OnDialogueEnded -= EnableControls;
    }

    void FixedUpdate()
    {
        //Debug.Log(IsOnSteepSlope());

        if (IsOnSteepSlope() && activateControls)
        {
            Movement();
        }

        GroundCheck();

        FootstepsFX();
    }

    private void LateUpdate()
    {
        if (activateControls)
        {
            Look();
        }
    }

    bool IsOnSteepSlope()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity))
        {
            float angle = Vector3.Angle(hit.normal, Vector3.up);
            return angle < maxSlopeAngle;
        }

        return false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        // Draw the raycast
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * Mathf.Infinity);

        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, Mathf.Infinity))
        {
            // Draw a sphere at the hit point
            Gizmos.DrawSphere(hit.point, 0.1f);

            // Display the slope angle as text in the Scene view
            #if UNITY_EDITOR
            UnityEditor.Handles.Label(hit.point, $"Angle: {Vector3.Angle(hit.normal, Vector3.up):F1}°");
            #endif

            // Change the Gizmos color if the slope is too steep
            if (Vector3.Angle(hit.normal, Vector3.up) > maxSlopeAngle)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawRay(hit.point, hit.normal); // Draw the normal vector
            }
            else
            {
                Gizmos.color = Color.green;
                Gizmos.DrawRay(hit.point, hit.normal);
            }
        }
    }

    private void GroundCheck()
    {
        Vector3 origin = transform.position + Vector3.up * 0.1f;


        isGrounded = Physics.Raycast(origin, Vector3.down, groundCheckDistance);

        Debug.DrawRay(origin, Vector3.down * groundCheckDistance, Color.red);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        move = context.ReadValue<Vector2>();
        isMoving = context.performed;
    }

    private void StartSprinting(InputAction.CallbackContext context)
    {
        isSprinting = true;
        speed = sprintSpeed;
    }

    private void StopSprinting(InputAction.CallbackContext context)
    {
        isSprinting = false;
        speed = normalSpeed;
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        look = context.ReadValue<Vector2>();
    }

    void Movement()
    {
        Vector3 currentVelocity = rb.velocity;
        //Find target velocity
        Vector3 targetVelocity = new Vector3(move.x, 0, move.y);
        targetVelocity *= speed;

        //Align directions
        targetVelocity = transform.TransformDirection(targetVelocity);

        //Calculate force
        Vector3 velocityChange = (targetVelocity - currentVelocity);
        velocityChange = new Vector3(velocityChange.x, 0, velocityChange.z);

        //Limit force
        Vector3.ClampMagnitude(velocityChange, maxForce);

        rb.AddForce(velocityChange, ForceMode.VelocityChange);

    }

    private void Jump(InputAction.CallbackContext context)
    {
        if (isGrounded)
        {

            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    private void Look()
    {
        //Turn player
        transform.Rotate(Vector3.up * look.x * sensitivity);

        //Look up and down
        lookRotation += (-look.y * sensitivity);
        lookRotation = Mathf.Clamp(lookRotation, -90, 90);
        playerInput.camera.transform.eulerAngles = new Vector3(lookRotation, playerInput.camera.transform.eulerAngles.y, playerInput.camera.transform.eulerAngles.z);
    }

    public void DisableControls()
    {
        activateControls = false;
    }

    public void EnableControls()
    {
        activateControls = true;
    }


    void FootstepsFX()
    {
        if (isGrounded && isMoving)
        {
            AudioManager.instance?.onPlayFootstep?.Invoke(0, speed);
        }
    }
}
