using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Collections;
using System;

public class AirplaneAerodynamics : MonoBehaviour
{
    //FOR TESTING
    public List<PlanePart> parts = new List<PlanePart>();
    public float healthThreshold = 50f;
    public TextMeshProUGUI speedText;
    //

    public float forwardSpeed;
    public float kph;
    public float maxKph = 178f;
    private float maxMPS;
    private float normalizedKph;

    //UNDERWATER
    [SerializeField] private float waterHeight = 7f;
    [SerializeField] private float underwaterDrag = 2f;
    [SerializeField] private float underwaterAngularDrag = 4f;
    [SerializeField] private float buoyancyForce = 8f;
    [SerializeField] private AnimationCurve buoyancyCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);

    private bool isUnderwater = false;

    //LIFT
    //public float baseLiftPower = 800f;
    private float currentLiftPower;
    public float maxLiftPower = 4100f;
    public AnimationCurve liftCurve = AnimationCurve.EaseInOut(0, 0, 1f, 1f);

    public float pitchSpeed = 1000f;
    public float rollSpeed = 1000f;
    public float yawSpeed = 1000f;
    public float lerpSpeed = 0.03f;

    private BaseAirplaneInputs airplaneInputs;

    //DRAG
    public float dragFactor = 0.01f;
    public float flapDragFactor = 0.005f;

    //ANGLE OF ATTACK
    private float angleOfAttack;
    private float pitchAngle;
    private float rollAngle;


    private Rigidbody rb;
    private float startDrag;
    private float startAngularDrag;

    const float mpsToKph = 3.6f;

    public List<float> altitudeThresholds = new List<float> {}; 
    public List<float> requiredLiftPower = new List<float> { 3500f, 3700f, 4000f };

    public event Action OnCrash;

    public void InitializeAerodynamics(Rigidbody rigidBody, BaseAirplaneInputs inputs)
    {
        airplaneInputs = inputs;
        rb = rigidBody;
        startDrag = rb.drag;
        startAngularDrag = rb.angularDrag;

        maxMPS = maxKph / mpsToKph;
    }

    public void UpdateAerodynamics()
    {
        if (rb)
        {
            //CheckIfUnderwater();
            ForwardSpeed();
            Lift();
            Drag();
            Pitch();
            Roll();
            Yaw();
            Banking();
            RigibodyTransform();
            HandleAltitude(); 
        }
    }
    void ForwardSpeed()
    {
        Vector3 localVelocity = transform.InverseTransformDirection(rb.velocity);
        forwardSpeed = Mathf.Max(0, localVelocity.z);
        forwardSpeed = Mathf.Clamp(forwardSpeed, 0, maxKph);

        kph = forwardSpeed * mpsToKph;
        kph = Mathf.Clamp(kph, 0, maxKph);
        normalizedKph = Mathf.InverseLerp(0, maxKph, kph);


        //speedText.text = "KPH: " + kph;
    }

    bool CanGenerateLift()
    {
        // Find left and right wings in the parts list
        PlanePart leftWing = parts.Find(part => part.name.Contains("LeftWing"));
        PlanePart rightWing = parts.Find(part => part.name.Contains("RightWing"));

        // Ensure both wings are present and check if they are above the health threshold
        if (leftWing != null && rightWing != null)
        {
            return leftWing.CurrentHealth > healthThreshold && rightWing.CurrentHealth > healthThreshold;
        }

        return false;
    }

    void Lift()
    {
        if (kph > 10 && CanGenerateLift())
        {

            //get the angle of attack
            angleOfAttack = Vector3.Dot(rb.velocity.normalized, transform.forward);

            //Make it more of a curve instead of a linear value for a more realistic feel
            angleOfAttack *= angleOfAttack;


            //Debug.Log("Angle of attack: " + angleOfAttack);
            Vector3 liftDirection = transform.up;

            //Scalar value that we can use for the lift direction
            currentLiftPower = liftCurve.Evaluate(normalizedKph) * maxLiftPower;
            //Debug.Log(liftPower);

            

            Vector3 finalLiftForce = liftDirection * currentLiftPower * angleOfAttack;

            rb.AddForce(finalLiftForce);
        }
        else
        {
            //Debug.Log("Cannot generate lift: one or both wings are too damaged.");
        }
    }


    //Drag force to keep the plane stable in the air 
    void Drag()
    {
        //Speed drag
        float speedDrag = forwardSpeed * dragFactor * flapDragFactor;

        //Flap drag
        float flapDrag = airplaneInputs.Flaps * flapDragFactor;


        float finalDrag = startDrag + speedDrag + flapDrag;

        rb.drag = finalDrag;
        rb.angularDrag = startAngularDrag * forwardSpeed;
    }

    //Rotate the rigidbody towards the direction the engine is pulling it  
    void RigibodyTransform()
    {

        if (rb.velocity.magnitude > 1f)
        {

            Vector3 updatedVelocity = Vector3.Lerp(rb.velocity, transform.forward * forwardSpeed, forwardSpeed * angleOfAttack * Time.deltaTime * lerpSpeed);
            rb.velocity = updatedVelocity;

            Quaternion updatedRotation = Quaternion.Slerp(rb.rotation, Quaternion.LookRotation(rb.velocity, transform.up), Time.deltaTime * lerpSpeed);
            rb.MoveRotation(updatedRotation);
        }
        else if (rb.velocity.magnitude < 0.1f)
        {
            rb.velocity = Vector3.zero;
        }
    }

    void Pitch()
    {
        Vector3 flatForward = transform.forward;
        flatForward.y = 0;
        flatForward = flatForward.normalized;

        pitchAngle = Vector3.Angle(transform.forward, flatForward);

        //Debug.Log("Pitch Angle: " + pitchAngle);

        //
        Vector3 pitchTorque = airplaneInputs.Pitch * pitchSpeed * transform.right;

        rb.AddTorque(pitchTorque);
    }

    void Roll()
    {
        Vector3 flatRight = transform.right;
        flatRight.y = 0;
        flatRight = flatRight.normalized;

        rollAngle = Vector3.SignedAngle(transform.right, flatRight, transform.forward);

        Vector3 rollTorque = -airplaneInputs.Roll * rollSpeed * transform.forward;

        rb.AddTorque(rollTorque);
    }

    void Yaw()
    {
        Vector3 yawTorque = airplaneInputs.Yaw * yawSpeed * transform.up;

        rb.AddTorque(yawTorque);
    }

    void Banking()
    {
        //Get a value between 0 and 1 based on how much we are rolling to the left and right 
        float bankSide = Mathf.InverseLerp(-90, 90, rollAngle);
        float bankAmount = Mathf.Lerp(-1, 1, bankSide);
        Vector3 bankTorque = bankAmount * rollSpeed * transform.up;
        rb.AddTorque(bankTorque);
    }
    void CheckIfUnderwater()
    {
        if (transform.position.y < waterHeight)
        {
            Debug.Log("Plane is underwater");
            OnCrash?.Invoke();
            isUnderwater = true;
        }
        else
        {
            isUnderwater = false;
        }
    }
    public void ApplyUnderwaterPhysics()
    {
        rb.drag = underwaterDrag;
        rb.angularDrag = underwaterAngularDrag;

        // Simulate buoyancy
        float depth = Mathf.Abs(transform.position.y - waterHeight);
        float normalizedDepth = Mathf.InverseLerp(0, 10f, depth); 
        float buoyancy = buoyancyCurve.Evaluate(normalizedDepth) * buoyancyForce;

        Vector3 buoyancyVector = Vector3.up * buoyancy;
        rb.AddForce(buoyancyVector, ForceMode.Acceleration);

        float dampingFactor = Mathf.Lerp(1f, 0.1f, normalizedDepth);
        rb.velocity = Vector3.Lerp(rb.velocity, Vector3.zero, Time.deltaTime * (1f - dampingFactor));

        rb.drag = Mathf.Lerp(startDrag, underwaterDrag, normalizedDepth);
        rb.angularDrag = Mathf.Lerp(startAngularDrag, underwaterAngularDrag, normalizedDepth);
    }

    void HandleAltitude()
    {
        // Altitude thresholds and required lift power values
        for (int i = 0; i < altitudeThresholds.Count; i++)
        {
            if (rb.position.y > altitudeThresholds[i])
            {
                // Check if the current lift power is sufficient for this altitude
                if (currentLiftPower < requiredLiftPower[i])
                {
                    Debug.Log("LIMIT ALTITUDE");
                    // Limit altitude by zeroing vertical velocity and applying downward force
                    rb.velocity = new Vector3(rb.velocity.x, Mathf.Min(rb.velocity.y, 0f), rb.velocity.z);
                    rb.AddForce(Vector3.down * 10f, ForceMode.Acceleration); 
                    break; 
                }
            }
        }
    }

    public bool IsUnderwater()
    {
        return isUnderwater;
    }

    public void ResetPhysics()
    {
        foreach(PlanePart part in parts)
        {
            part.ResetHealth();
        }
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        isUnderwater = false;
    }
}