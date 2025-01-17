using UnityEngine;
using TMPro;

public class RingTracker : MonoBehaviour
{
    public Transform airplaneTransform;
    public float distanceInFrontOfPlane = 2500f;
    public GameObject ringsParent;
    public TextMeshProUGUI ringCounterText;
    private int totalRings;
    public int ringsPassed = 0;

    public bool isCompleted = false;



    private void Start()
    {
        totalRings = GameObject.FindGameObjectsWithTag("Ring").Length;
        UpdateRingCounter();
    }

    private void OnEnable()
    {
        airplaneTransform = GetComponent<Transform>();
        

        if (airplaneTransform != null && ringsParent != null)
        {
            Vector3 forwardPosition = airplaneTransform.position + airplaneTransform.forward * distanceInFrontOfPlane;
            forwardPosition.y = airplaneTransform.position.y; 
            ringsParent.transform.position = forwardPosition;

            ringsParent.transform.rotation = Quaternion.LookRotation(airplaneTransform.forward);
        }
        ringsParent.SetActive(true);
        ringCounterText.gameObject.SetActive(true);
    }

    private void OnTriggerEnter(Collider other)
    {

            // Check if the airplane collider is the one entering the ring's trigger collider
            if (other.CompareTag("Ring"))
            {
                Ring ring = other.GetComponentInParent<Ring>();

                if (ring != null && !ring.isPassedThrough)
                {
                    ring.isPassedThrough = true;
                    ringsPassed++;
                    UpdateRingCounter();
                    Destroy(other.transform.parent.gameObject);
                }
            }
    }

    // Update the ring counter display.
    private void UpdateRingCounter()
    {
        if (ringCounterText != null)
        {
            ringCounterText.text = $"{ringsPassed}/{totalRings} Rings Passed";

            if (ringsPassed == totalRings)
            {
                isCompleted = true;
            }
        }
    }

    public void ResetRingsPos()
    {
        if (airplaneTransform != null && ringsParent != null)
        {
            Vector3 forwardPosition = airplaneTransform.position + airplaneTransform.forward * distanceInFrontOfPlane;
            forwardPosition.y = airplaneTransform.position.y;
            ringsParent.transform.position = forwardPosition;

            ringsParent.transform.rotation = Quaternion.LookRotation(airplaneTransform.forward);
        }
    }
}