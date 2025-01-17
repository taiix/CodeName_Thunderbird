using UnityEngine;
using UnityEngine.InputSystem;
public class BaseAirplaneInputs : MonoBehaviour
{

    public InputActionAsset inputActions;
    #region Input Actions

    private InputAction pitchAction;
    private InputAction rollAction;
    private InputAction yawAction;
    private InputAction throttleAction;
    private InputAction breakAction;
    private InputAction flapsAction;

    #endregion

    public float throttleSpeed = 0.1f;

    public bool needsPlayer = true;

    private float stickyThrottle;

    public float yawSpeed = 50f;
    private float pitch = 0f;
    private float roll = 0f;
    private float yaw = 0f;
    private float throttle = 0f;
    private float breaks = 0f;

    public int maxFlapsIncrement = 2;
    private int flaps = 0;

    private bool positiveFlapsPressedLastFrame = false;
    private bool negativeFlapsPressedLastFrame = false;

    [SerializeField] private bool disableInput = false;
    private void OnEnable()
    {
        // Get the "Flight" action map
        var flightActionMap = inputActions.FindActionMap("Flight");

        // Get actions from the action map

        pitchAction = flightActionMap.FindAction("Pitch");
        rollAction = flightActionMap.FindAction("Roll");
        yawAction = flightActionMap.FindAction("Yaw");
        throttleAction = flightActionMap.FindAction("Throttle");
        breakAction = flightActionMap.FindAction("Break");
        flapsAction = flightActionMap.FindAction("Flaps");

        // Enable the actions
        pitchAction.Enable();
        rollAction.Enable();
        yawAction.Enable();
        throttleAction.Enable();
        breakAction.Enable();
        flapsAction.Enable();
    }

    private void OnDisable()
    {

        // Disable the actions
        pitchAction.Disable();
        yawAction.Disable();
        throttleAction.Disable();
        breakAction.Disable();
        flapsAction.Disable();
    }

    #region Properties
    public float Pitch
    {
        get { return pitch; }
        set { }
    }
    public float Roll
    {
        get { return roll; }
    }
    public float Yaw
    {
        get { return yaw; }
    }
    public float Throttle
    {
        get { return throttle; }
    }
    public int Flaps
    {
        get { return flaps; }
    }
    public float NormalizedFlaps
    {
        get
        {
            return (float)flaps / maxFlapsIncrement;
        }
    }
    public float Break
    {
        get { return breaks; }
    }
    public float StickyThrottle
    {
        get { return stickyThrottle; }
        set { stickyThrottle = value; }
    }
    #endregion

    // Update is called once per frame
    void Update()
    {
        if (needsPlayer && GameManager.Instance.IsPLayerInPlane() && !disableInput)
        {
            HandleInput();
        }
        else
        {
            HandleInput();
        }
    }


    void HandleInput()
    {
        pitch = pitchAction.ReadValue<float>();
        roll = rollAction.ReadValue<float>();
        yaw = yawAction.ReadValue<float>();
        throttle = throttleAction.ReadValue<float>();
        StickyThrottleControl();

        breaks = breakAction.IsPressed() ? 1f : 0f;

        // Get the current state of the flaps action controls
        bool positiveFlapsPressed = flapsAction.controls[0].IsPressed();
        bool negativeFlapsPressed = flapsAction.controls[1].IsPressed();

        if (positiveFlapsPressed && !positiveFlapsPressedLastFrame)
        {
            flaps = Mathf.Clamp(flaps + 1, -1, maxFlapsIncrement);
        }

        if (negativeFlapsPressed && !negativeFlapsPressedLastFrame)
        {
            flaps = Mathf.Clamp(flaps - 1, -1, maxFlapsIncrement);
        }

        // Update the last frame states
        positiveFlapsPressedLastFrame = positiveFlapsPressed;
        negativeFlapsPressedLastFrame = negativeFlapsPressed;

        //Debug.Log("Current Flaps value: " + flaps);
    }

    void StickyThrottleControl()
    {
        stickyThrottle = stickyThrottle + (throttle * throttleSpeed * Time.deltaTime);
        stickyThrottle = Mathf.Clamp01(stickyThrottle);
        //Debug.Log("Sticky Throttle = " + stickyThrottle);
    }

    public void DisableInput()
    {
        disableInput = true;
    }

    public void EnableInput()
    {
        disableInput = false;
    }

    public void ResetStickyThrottle()
    {
        stickyThrottle = 0f;
    }
}
