using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.DefaultInputActions;


public interface IPlayer
{
    InputActionMap PlayerAction { get; set; }
}

public class CharacterMovement : MonoBehaviour
{
    public Rigidbody rb;
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
    private InputAction openInventory;

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

    [SerializeField] int jumpForce = 2;
    [SerializeField] bool canJump = false;

    private bool isInventoryOpen = false;

    private void Awake()
    {
        playerInput = this.GetComponent<PlayerInput>();
        player = playerInput.currentActionMap;
    }
    // Start is called before the first frame update
    void Start()
    {
        speed = normalSpeed;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void OnEnable()
    {
        openInventory = player.FindAction("Inventory");

        openInventory.performed += DisableControls;

        openInventory.Enable();
    }

    private void OnDisable()
    {
        openInventory.Disable();

        openInventory.performed -= DisableControls;

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Movement();

    }

    private void LateUpdate()
    {
        if (!isInventoryOpen)
        {
            Look();
        }
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

    private void Jump()
    {
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
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

    private void DisableControls(InputAction.CallbackContext context)
    {
        isInventoryOpen = !isInventoryOpen;
    }
}
