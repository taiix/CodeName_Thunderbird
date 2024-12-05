using System.Collections;
using System.Collections.Generic;
using System.Net;
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

            airplaneAerodynamics = GetComponent<AirplaneAerodynamics>();

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
                    rb.AddForce(engine.CalculateForce(Input.Throttle));

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
        EnterWater();
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
}
