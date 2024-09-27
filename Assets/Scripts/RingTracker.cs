using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RingTracker : MonoBehaviour
{
    public TextMeshProUGUI ringCounterText; 
    private int totalRings; 
    private int ringsPassed = 0; 

    private void Start()
    {
    
        totalRings = GameObject.FindGameObjectsWithTag("Ring").Length;
        UpdateRingCounter();
    }

    private void OnTriggerEnter(Collider other)
    {
        
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

            if(ringsPassed == totalRings)
            {
                ringCounterText.text = "Good Job! \n Now land :) ";
            }
        }
    }
}