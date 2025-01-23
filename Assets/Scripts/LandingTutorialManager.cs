using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum LandingTutorialState
{
    LowerAltitude,
    LowerRPMS2000,
    LowerFlaps30,
    GetCloseEnoughToRunway,
    LowerRPMS1500,
    LowerFlaps60,
    PassTheTreshold,
    PowerDown,
    FinishTutorial,
    None
}

public class LandingTutorialManager : MonoBehaviour
{
    public LandingTutorialState state;
    public Transform airplaneTransform;
    public Transform runway;
   // public Transform thresholdMarks;

    private float distanceToRunway = 0;

    [SerializeField] TextMeshProUGUI distanceToRunwayText;

    [SerializeField] private Rigidbody _planeRb;

    [Space]
    [SerializeField] private AirplaneAltimeter _airplaneAltimeter;
    [SerializeField] private AirplaneTachometer _airplaneTachometer;
    [SerializeField] private BaseAirplaneInputs _airplaneInputs;
    [Space]

    [SerializeField] private TextMeshProUGUI heightText;
    [SerializeField] private TextMeshProUGUI explanationText;

    [SerializeField] private float desiredHeight = 1000;
    private float desiredRPMS1 = 2000;
    private float desiredRPMS2 = 1800;
    private int desiredFlaps30 = 1;
    private int desiredFlaps60 = 2;
    private float desiredDistance = 3000;

    private bool hasPassedThresholdMarks = false;


    void Start()
    {
        state = LandingTutorialState.LowerAltitude;
        explanationText.transform.parent.gameObject.SetActive(false);
        Cursor.visible = false;
    }

    void LowerAltitude()
    {
        float altitude = _airplaneAltimeter.GetAltitude();

        explanationText.transform.parent.gameObject.SetActive(true);
        explanationText.text = "Use W and S on the keyboard to pitch the plane up or down. S - up W - down";

        heightText.text = $"Lower your height to 1000 meters " +
            $"current height: " + altitude;

        //Debug.Log(altitude);
        if (altitude != 0 && altitude <= desiredHeight)
        {
            StartCoroutine(Wait(LandingTutorialState.LowerRPMS2000, 1));
            explanationText.transform.parent.gameObject.SetActive(false);
        }
    }

    void LowerRPMS2000()
    {
        float rpms = _airplaneTachometer.engine.CurrentRPM;

        heightText.color = Color.black; 
        explanationText.transform.parent.gameObject.SetActive(true);
        explanationText.text = "Use Up and Down arrow keys on the keyboard to throttle down and lower the RPMS";

        heightText.text = $"Lower your RPMS to 2000." +
            $"current RPMS: " + rpms;

        if (rpms <= desiredRPMS1)
        {
            StartCoroutine(Wait(LandingTutorialState.LowerFlaps30, 1));
            explanationText.transform.parent.gameObject.SetActive(false);
        }
    }

    void LowerFlaps30()
    {
        int flaps = _airplaneInputs.Flaps;

        heightText.color = Color.black;
        explanationText.transform.parent.gameObject.SetActive(true);
        explanationText.text = "Use V on the keyboard to lower your flaps in order to start slowing down the plane." +
            "You should also lower your RPMS to 1800.";

        heightText.text = $"Lower the planes flaps down to 30°";

        if (flaps >= desiredFlaps30)
        {
            StartCoroutine(Wait(LandingTutorialState.GetCloseEnoughToRunway, 1));
            explanationText.transform.parent.gameObject.SetActive(false);
        }
    }

    void DesiredDistance()
    {
        heightText.color = Color.black;
        explanationText.transform.parent.gameObject.SetActive(true);
        explanationText.text = "Soon you should start seeing the runway. When you see it start aligning your plane " +
            "such that the runway is in front of you. Use A and D to turn the plane and position it correctly";

        heightText.text = "Look ahead and spot the runway";

        if (distanceToRunway < desiredDistance)
        {
            StartCoroutine(Wait(LandingTutorialState.LowerRPMS1500, 1));
            explanationText.transform.parent.gameObject.SetActive(false);
        }
    }

    void LowerRPMS1500()
    {
        float rpms = _airplaneTachometer.engine.CurrentRPM;

        heightText.color = Color.black;
        explanationText.transform.parent.gameObject.SetActive(true);
        explanationText.text = "Use Up and Down arrow keys on the keyboard to throttle down and lower the RPMS";

        heightText.text = $"Lower your RPMS to 1500." +
            $"current RPMS: " + rpms;

        if (rpms <= desiredRPMS2)
        {
            StartCoroutine(Wait(LandingTutorialState.LowerFlaps60, 1));
            explanationText.transform.parent.gameObject.SetActive(false);
        }
    }

    void LowerFlaps60()
    {
        heightText.color = Color.black;
        int flaps = _airplaneInputs.Flaps;

        explanationText.transform.parent.gameObject.SetActive(true);
        explanationText.text = "You are almost there. Use V on the keyboard to lower your flaps in order to start slowing down for the landing";

        heightText.text = "Lower the planes flaps down to 60°";

        if (flaps >= desiredFlaps60)
        {
            StartCoroutine(Wait(LandingTutorialState.PassTheTreshold, 2));
            explanationText.transform.parent.gameObject.SetActive(false);
        }
    }

    void PassThresholdMarkings()
    {
        heightText.color = Color.black;
        

        explanationText.transform.parent.gameObject.SetActive(true);
        explanationText.text = "Look out for the white marks at the start of the runway. Once you pass them you should power down " +
            "the engine to 0%.";

        heightText.text = "Watch out for the threshold marks and power down after passing them.";

        if (hasPassedThresholdMarks && _airplaneInputs.StickyThrottle == 0)
        {
            StartCoroutine(Wait(LandingTutorialState.PowerDown, 1));
            explanationText.transform.parent.gameObject.SetActive(false);
        }
    }

    void PowerDown()
    {
        heightText.color = Color.black;
        explanationText.transform.parent.gameObject.SetActive(true);
        explanationText.text = " Manouver carefully in order to land the plane. Use spacebar to apply the breaks for a full" +
            "stop after the wheels are on the ground.";

        heightText.text = "Land the plane and stop completely.";

        if(_planeRb.velocity.magnitude <= 1)
        {
            StartCoroutine(Wait(LandingTutorialState.FinishTutorial, 1));
            explanationText.transform.parent.gameObject.SetActive(false);
        }

    }

    void FinishTutorial()
    {
        heightText.color = Color.black;
        explanationText.transform.parent.gameObject.SetActive(true);
        explanationText.text = "You now know all the basics for landing an airplane. Good luck on your adventures!";

        heightText.text = "Landing tutorial completed!";
    }

    void Update()
    {
        GetDistanceToRunway();

        switch (state)
        {
            case LandingTutorialState.LowerAltitude:
                LowerAltitude();
                break;
            case LandingTutorialState.LowerRPMS2000:
                LowerRPMS2000();
                break;
            case LandingTutorialState.LowerFlaps30:
                LowerFlaps30();
                break;
            case LandingTutorialState.GetCloseEnoughToRunway:
                DesiredDistance();
                break;
            case LandingTutorialState.LowerRPMS1500:
                LowerRPMS1500();
                break;
            case LandingTutorialState.LowerFlaps60:
                LowerFlaps60();
                break;
            case LandingTutorialState.PassTheTreshold:
                PassThresholdMarkings();
                break;
            case LandingTutorialState.PowerDown:
                PowerDown();
                break;
            case LandingTutorialState.FinishTutorial:
                FinishTutorial();
                break;
            case LandingTutorialState.None:
                break;
        }
    }

    IEnumerator Wait(LandingTutorialState _state, int time)
    {
        heightText.color = Color.green;
        yield return new WaitForSeconds(time);
        state = _state;
        //isCompleted = _isCompleted;
    }

    void GetDistanceToRunway()
    {
           distanceToRunway = Vector3.Distance(airplaneTransform.position, runway.position);
           distanceToRunwayText.text = "Distance to runway: " + distanceToRunway;  
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Plane")){
            hasPassedThresholdMarks = true;
        }
    }
}
