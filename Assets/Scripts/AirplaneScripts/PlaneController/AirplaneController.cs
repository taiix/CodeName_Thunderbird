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
        if (Input && canControlPlane)
        {
            HandleEngines();
            HandleAerodynamics();
            HandleWheel();
            HandleAltitude();
        }
        else
        {
            ApplyHandbrake();
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
        if(wheels.Count > 0)
        {
            foreach(AirplaneWheels wheel in wheels)
            {
                wheel.HandleWheel(Input, canControlPlane);
            }
        }
    }


    void HandleAltitude()
    {
        //TODO
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
                wheel.HandleWheel(Input, false); // Force the handbrake to be applied
            }
        }
    }
}
