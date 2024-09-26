using UnityEngine;
using UnityEngine.InputSystem;

public class PlaneReset : MonoBehaviour
{
    private Vector3 initialPosition;
    private Quaternion initialRotation;

    private BaseAirplaneInputs airplaneInputs;

    public InputActionAsset planeActions;

    private InputAction resetAction;


    void Start()
    {
        initialPosition = transform.position;
        initialRotation = transform.rotation;

        airplaneInputs = GetComponent<BaseAirplaneInputs>();
    }

    private void OnEnable()
    {
        resetAction = planeActions.FindAction("ResetPlane");
        resetAction.Enable();
        resetAction.performed += ResetPlanePosition;

    }

    private void OnDisable()
    {
        resetAction.performed -= ResetPlanePosition;
        resetAction.Disable();
    }

    void Update()
    {

        //if (Input.GetKeyDown(KeyCode.R))
        //{
        //   // ResetPlanePosition();
        //}
    }


    public void ResetPlanePosition(InputAction.CallbackContext context)
    {
        transform.position = initialPosition;
        transform.rotation = initialRotation;


        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        AirplaneAerodynamics aerodynamics = GetComponent<AirplaneAerodynamics>();
        if (aerodynamics != null)
        {
            aerodynamics.forwardSpeed = 0f;
        }

        if (airplaneInputs != null)
        {
            airplaneInputs.ResetStickyThrottle();
        }
    }
}