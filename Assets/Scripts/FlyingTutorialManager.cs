using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum FlyingTutorialStates
{
    Introduction,
    Banking,
    Rudder,
    RingRoll,
    FinishTutorial,
    None,
}

public class FlyingTutorialManager : MonoBehaviour
{
    public FlyingTutorialStates state;

    public Transform airplaneTransform;
    [SerializeField] private BaseAirplaneInputs _airplaneInputs;

    [SerializeField] private TextMeshProUGUI taskText;
    [SerializeField] private TextMeshProUGUI explanationText;
    [SerializeField] private BarrelRollHandler barrelRollHandler;
    [SerializeField] private RingTracker ringTracker;

    private bool completedBanking = false;
    private bool completedRudder = false;
    private bool completedRingRoll = false;

    private float bankingThreshold = 45f; 
    private float rudderThreshold = 45f; 

    private float initialYaw;
    private float maxBankingAngle = 0f;
    private void Start()
    {
        state = FlyingTutorialStates.Introduction;
        explanationText.transform.parent.gameObject.SetActive(false);

    }

    void Update()
    {
        switch (state)
        {
            case FlyingTutorialStates.Introduction:
                Introduction();
                break;
            case FlyingTutorialStates.Banking:
                Banking();
                break;
            case FlyingTutorialStates.Rudder:
                Rudder();
                break;
            case FlyingTutorialStates.RingRoll:
                RingRoll();
                break;
            case FlyingTutorialStates.FinishTutorial:
                FinishTutorial();
                break;
        }
    }


    void Introduction()
    {
        taskText.text = "Welcome to the flying tutorial.";

        explanationText.transform.parent.gameObject.SetActive(true);
        explanationText.text = "In this tutorial you will learn the basics of flying. How to maneuver the plane in the air" +
            " and how to do some fun stuff with it.";

        StartCoroutine(Wait(FlyingTutorialStates.Banking, 15));
        
    }

    void Banking()
    {
        taskText.color = Color.black;
        taskText.text = "Try turning the plane to the left and right.";

        explanationText.transform.parent.gameObject.SetActive(true);
        explanationText.text = "Banking is when you turn the plane from side to side changing its heading. " +
            "Use A - to bank to the left D - to bank to the right. ";

        float currentBankingAngle = Vector3.Angle(Vector3.up, airplaneTransform.right) - 90f;
        currentBankingAngle = Mathf.Abs(currentBankingAngle); // Ensure it's positive
        maxBankingAngle = Mathf.Max(maxBankingAngle, currentBankingAngle);

        if (maxBankingAngle >= bankingThreshold)
        {
            StartCoroutine(Wait(FlyingTutorialStates.Rudder, 4));
            explanationText.transform.parent.gameObject.SetActive(false);
            initialYaw = airplaneTransform.eulerAngles.y; // Store initial yaw for the rudder stage
        }
    }

    void Rudder()
    {
        taskText.color = Color.black;

        taskText.text = "Use the Rudder to point the plane to the left direction.";

        explanationText.transform.parent.gameObject.SetActive(true);
        explanationText.text = "The rudder controls rotation about the vertical axis of the aircraft. " +
            " This movement is referred to as \"yaw\". \n Use the Left and Right arrow keys to rotate the plane.";



        float currentYaw = Mathf.DeltaAngle(initialYaw, airplaneTransform.eulerAngles.y);
        currentYaw = Mathf.Abs(currentYaw); // Ensure it's positive

        if (currentYaw >= rudderThreshold)
        {
            StartCoroutine(Wait(FlyingTutorialStates.RingRoll, 4));
            explanationText.transform.parent.gameObject.SetActive(false);
        }
    }

    void RingRoll()
    {
        taskText.color = Color.black;

        barrelRollHandler.gameObject.SetActive(true);
        ringTracker.enabled = true;
       

        taskText.text = "Go through all the rings and do some barrel rolls.";

        explanationText.transform.parent.gameObject.SetActive(true);
        explanationText.text = "Using what you learned manouver the plane in order to fly through all of the rings. " +
            "Do some barrel rolls by holding A or D.";


        if (ringTracker.isCompleted && barrelRollHandler.isCompleted)
        {
            StartCoroutine(Wait(FlyingTutorialStates.FinishTutorial, 4));
            explanationText.transform.parent.gameObject.SetActive(false);
        }
    }

    void FinishTutorial()
    {
        taskText.color = Color.blue;

        taskText.text = "Tutorial Finished! ";

        explanationText.transform.parent.gameObject.SetActive(true);
        explanationText.color = Color.blue;
        explanationText.text = "You completed the flying tutorial! Feel free to fly around, do other tutorials or exit.";


    }

    IEnumerator Wait(FlyingTutorialStates _state, int time)
    {
        taskText.color = Color.green;
        yield return new WaitForSeconds(time);
        state = _state;
      
    }


}
