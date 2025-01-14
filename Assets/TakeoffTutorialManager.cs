using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class TakeoffTutorialManager : MonoBehaviour
{
    public TutorialState state;

    [SerializeField] private Rigidbody _planeRb;

    [SerializeField] private AirplaneAltimeter _airplaneAltimeter;
    [SerializeField] private AirplaneTachometer _airplaneTachometer;
    [SerializeField] private AirplaneThrottleLever _airplaneThrottleLever;
    [Space]
    [SerializeField] private TextMeshProUGUI _heightText;
    [SerializeField] private TextMeshProUGUI _explanationText;

    /// <summary>
    /// Height in meters
    /// </summary>
    [SerializeField] private float desiredHeight;
    private float desiredAcceleration = 190;
    private float desiredThrottle = 0.9f;


    //[SerializeField] private bool isCompleted = false;

    [SerializeField] private Animation _fadeEffectAnim;

    void Start()
    {
        state = TutorialState.PowerUpEngines;
        _explanationText.transform.parent.gameObject.SetActive(false);
    }

    void Update()
    {
        switch (state)
        {
            case TutorialState.PowerUpEngines:
                PowerUpEngines();
                break;
            case TutorialState.ReachAcceleration:
                ReachAcceleration();
                break;
            case TutorialState.Lifting:
                Lifting();
                break;
            case TutorialState.ReachHeight:
                ReachHeight();
                break;
            case TutorialState.None:
                _fadeEffectAnim.Play();
                break;
        }
    }
    private void PowerUpEngines()
    {
        //Use the Up and down arrow to control the engine throttle
        //increase it to 100%

        float throttle = _airplaneThrottleLever.GetThrottle();

        _explanationText.transform.parent.gameObject.SetActive(true);
        _explanationText.text = "Use Arrow Up to increase the throttle and Arrow Down to decrease it";

        _heightText.text = $"Increase the throttle to 100% " +
            $"current throttle level {(throttle * 100).ToString("0")}%";

        if (throttle >= desiredThrottle)
        {
            StartCoroutine(Wait(TutorialState.ReachAcceleration, 1));
            _explanationText.transform.parent.gameObject.SetActive(false);
        }
    }

    void ReachAcceleration()
    {
        float currentAcceleration = _airplaneTachometer.GetRotation();

        _heightText.text = $"Keep the RPM between 18 and 27";

        if (IsRotationBetween(130f, 190f))
        {
            StartCoroutine(Wait(TutorialState.ReachHeight, 3));
        }
    }

    private void Lifting()
    {

    }

    void ReachHeight()
    {
        float planeHeight = _airplaneAltimeter.GetAltitude();

        _heightText.color = Color.white;
        _heightText.text = $"Reach height of {(int)desiredHeight} meters" +
            $"  Current height: {planeHeight.ToString("0")}";

        if (planeHeight >= desiredHeight)
        {
            StartCoroutine(Wait(TutorialState.None, 5));
        }
    }

    public bool IsRotationBetween(float minAngle, float maxAngle)
    {
        float rotationZ = _airplaneTachometer.GetRotation();
        return rotationZ >= minAngle && rotationZ <= maxAngle;
    }

    IEnumerator Wait(TutorialState _state, int time)
    {
        _heightText.color = Color.green;
        yield return new WaitForSeconds(time);
        state = _state;
        //isCompleted = _isCompleted;
    }

    public enum TutorialState
    {
        PowerUpEngines,
        Lifting,
        ReachAcceleration,
        ReachHeight,
        None
    }
}
