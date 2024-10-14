using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.DefaultInputActions;

public class CharacterMovement : MonoBehaviour
{
    private Rigidbody rb;
    float speed;
    public float sensitivity;
    public float maxForce;

    public float sprintSpeed;
    public float normalSpeed;

    private Vector2 move;
    private Vector2 look;
    private float lookRotation;

    private InputActionMap player;
    private PlayerInput playerInput;
    private InputAction jumpAction;
    [SerializeField] private float groundCheckDistance = 0.1f;
    [SerializeField] int jumpForce = 2;
    private bool isGrounded;


    private bool activateControls = true;

    public static event Action OnDisableControls;
    public static event Action OnEnableControls;

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

    //[SerializeField] bool canJump = false;

    private void Awake()
    {
        playerInput = this.GetComponent<PlayerInput>();
        player = playerInput.currentActionMap;
    }
    // Start is called before the first frame update
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

        OnDisableControls += DisableControls;
        OnEnableControls += EnableControls;
    }

    private void OnDisable()
    {
        jumpAction.Disable();

        jumpAction.performed -= Jump;

        OnDisableControls -= DisableControls;
        OnEnableControls -= EnableControls;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (activateControls)
        {
            Movement();
        }
            GroundCheck();

    }

    private void LateUpdate()
    {
        if (activateControls)
        {
            Look();
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
}
