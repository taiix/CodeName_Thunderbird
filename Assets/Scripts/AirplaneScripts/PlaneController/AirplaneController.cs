using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AirplaneAerodynamics))]
public class AirplaneController : RigidBodyController
{

    public BaseAirplaneInputs Input;
    private AirplaneAerodynamics airplaneAerodynamics;
    public Transform centerOfGravity;
    public float airplaneWeight = 800f;


    public List<AirplaneEngine> engines = new List<AirplaneEngine>();
    public List<AirplaneWheels> wheels = new List<AirplaneWheels>();
    public List<FlightControlSurface> controlSurfaces = new List<FlightControlSurface>();


    private bool canControlPlane = false;

    private float currentMSL;
    public float CurrentMSL
    {
        get { return currentMSL; }
    }

    //Above ground level
    private float currentAGL;

    public float CurrentAGL
    {
        get { return currentAGL;}
    }

    void OnEnable()
    {
        airplaneAerodynamics = GetComponent<AirplaneAerodynamics>();
        if (airplaneAerodynamics != null)
        {
            Debug.Log("Subscribing to OnCrash");
            airplaneAerodynamics.OnCrash += HandleCrash;
        }
        else
        {
            Debug.LogError("AirplaneAerodynamics reference is null!");
        }
    }

    void OnDisable()
    {
        if (airplaneAerodynamics != null)
        {
            airplaneAerodynamics.OnCrash -= HandleCrash;
        }
    }

    public override void Start()
    {
        base.Start();

        if (rb)
        {
            rb.mass = airplaneWeight;

            if (centerOfGravity)
            {
                rb.centerOfMass = centerOfGravity.localPosition;
            }

            

            if (airplaneAerodynamics)
            {
                airplaneAerodynamics.InitializeAerodynamics(rb, Input);
            }
        }

        if (wheels != null)
        {
            if (wheels.Count > 0)
            {
                foreach (AirplaneWheels wheel in wheels)
                {
                    wheel.InitializeWheel();
                }
            }
        }


    }
    protected override void HandlePhysics()
    {
        if (!airplaneAerodynamics.IsUnderwater())
        {
            if (Input && canControlPlane)
            {
                HandleEngines();
                HandleAerodynamics();
                HandleWheel();
                HandleControlSurfaces();
                HandleAltitude();
            }
            else
            {
                ApplyHandbrake();
            }
        }
        else
        {
            HandleAltitude();
        }
    }

    void HandleEngines()
    {
        if (engines != null)
        {
            if (engines.Count > 0)
            {
                foreach (AirplaneEngine engine in engines)
                {
                    rb.AddForce(engine.CalculateForce(Input.StickyThrottle));

                }
            }
        }
    }

    void HandleAerodynamics()
    {
        airplaneAerodynamics.UpdateAerodynamics();
    }


    void HandleWheel()
    {
        //bool canBreak = false;
        //if(Input.Break == 1) canBreak = true;
        if (wheels.Count > 0)
        {
            foreach (AirplaneWheels wheel in wheels)
            {
                wheel.HandleWheel(Input, canControlPlane);
            }
        }
    }


    void HandleAltitude()
    {
        //EnterWater();

        //mean sea level - altitude
         currentMSL = transform.position.y;

        RaycastHit hit;

        if(Physics.Raycast(transform.position, Vector3.down, out hit))
        {
            if (hit.transform.CompareTag("Damageable"))
            {
                currentAGL = transform.position.y - hit.point.y;
                //Debug.Log("CurrentAGl = " + currentAGL);
            }
        }
    }

    private void EnterWater()
    {
        if (airplaneAerodynamics.IsUnderwater())
        {


            // Disable engines and adjust aerodynamics
            if (engines != null)
            {
                foreach (AirplaneEngine engine in engines)
                {
                    engine.DisableEngine();
                }
            }
       
            Input.DisableInput();

            airplaneAerodynamics.ApplyUnderwaterPhysics();
        }
    }

    void HandleControlSurfaces()
    {
        if (controlSurfaces.Count > 0)
        {
            foreach (FlightControlSurface controlSurface in controlSurfaces)
            {
                //Debug.Log($"Updating control surface of type: {controlSurface.type} with input Flaps: {Input.Flaps}");
                controlSurface.HandleControlSurface(Input);
            }
        }
    }

    public void ActivateControls()
    {
        canControlPlane = !canControlPlane;
    }

    private void ApplyHandbrake()
    {
        // If the player is not in control of the plane, apply the handbrake to all wheels
        if (wheels.Count > 0)
        {
            foreach (AirplaneWheels wheel in wheels)
            {
                wheel.HandleWheel(Input, false);
            }
        }
    }

    private void HandleCrash()
    {
        Debug.Log("Crash detected. Handling respawn...");
        StartCoroutine(HandleCrashRespawn());
    }

    private IEnumerator HandleCrashRespawn()
    {
        yield return new WaitForSeconds(2f);

        Island lastIsland = GameManager.Instance.GetLastIsland();

        if (lastIsland != null)
        {
            Transform respawnPoint = lastIsland.respawnPoint;
            airplaneAerodynamics.ResetPhysics();
            foreach(AirplaneEngine engine in engines)
            {
                engine.EnableEngine();
            }

            transform.position = respawnPoint.position;
            transform.rotation = respawnPoint.rotation;

            Debug.Log("Respawned at last island: " + lastIsland.name);
            Input.EnableInput();
        }
        else
        {
            Debug.LogWarning("No island to respawn to!");
        }
    }
}
