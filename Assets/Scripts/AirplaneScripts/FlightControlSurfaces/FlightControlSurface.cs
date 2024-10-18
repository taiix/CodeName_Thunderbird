using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

public enum ControlSurfaceType
{
    Rudder,
    Elevator,
    Flap,
    Aileron
}

public class FlightControlSurface : MonoBehaviour
{

    public ControlSurfaceType type = ControlSurfaceType.Rudder;
    public float maxAngle = 30;
    public Transform surfaceTransform;
    public Vector3 rotationAxis = Vector3.right;

    //Manipulate how fast the slerp works 
    public float slerpSpeed;

    private float wantedAngle;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (surfaceTransform)
        {
            Vector3 finalWantedAngle = rotationAxis * wantedAngle;
            surfaceTransform.localRotation = Quaternion.Slerp(surfaceTransform.localRotation, Quaternion.Euler(finalWantedAngle), Time.deltaTime * slerpSpeed);
        }
    }

    public void HandleControlSurface(BaseAirplaneInputs input)
    {
        float inputValue = 0;
        switch (type)
        {
            case ControlSurfaceType.Rudder:
                inputValue = input.Yaw;
                break;

            case ControlSurfaceType.Elevator:
                inputValue = input.Pitch;
                break;
            case ControlSurfaceType.Flap:
                inputValue = input.Flaps;
                break;
            case ControlSurfaceType.Aileron:
                inputValue = input.Roll;
                break;

            default:
                break;
        }
        //input value goes from -1 to 1 
        //Debug.Log(inputValue);
        wantedAngle = maxAngle * inputValue;
    }
}
