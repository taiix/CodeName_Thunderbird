using System.Collections;
using System.Collections.Generic;
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

    private float stickyThrottle;


    public float yawSpeed = 50f;
    private float pitch = 0f;
    private float roll = 0f;
    private float yaw = 0f;
    private float throttle = 0f;
    private float breaks = 0f;

    private int maxFlapsIncrement = 3;
    private int flaps = 0;

    private bool positiveFlapsPressedLastFrame = false;
    private bool negativeFlapsPressedLastFrame = false;

    private void OnEnable()
    {
        // Get the "Flight" action map
        var flightActionMap = inputActions.FindActionMap("Flight");

        // Get the yaw and throttle actions from the action map

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
        get{ return pitch;}
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
        get { return throttle;}
    }
    public int Flaps
    {
        get { return flaps;}
    }
    public float Break
    {
        get { return breaks; }
    }
    public float StickyThrottle
    {
        get { return stickyThrottle;}
    }
    #endregion

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
 

        HandleInput();
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
            flaps = Mathf.Clamp(flaps + 1, 0, maxFlapsIncrement);
        }

        if (negativeFlapsPressed && !negativeFlapsPressedLastFrame)
        {
            flaps = Mathf.Clamp(flaps - 1, 0, maxFlapsIncrement);
        }

        // Update the last frame states
        positiveFlapsPressedLastFrame = positiveFlapsPressed;
        negativeFlapsPressedLastFrame = negativeFlapsPressed;

        //Debug.Log("Throttle input value: " + throttle);
    }

    void StickyThrottleControl()
    {
        stickyThrottle = stickyThrottle + (throttle * throttleSpeed * Time.deltaTime);
        stickyThrottle = Mathf.Clamp01(stickyThrottle);
        //Debug.Log("Sticky Throttle = " + stickyThrottle);
    }
}
