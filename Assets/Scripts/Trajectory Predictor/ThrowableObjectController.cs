using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ThrowableObjectController : MonoBehaviour
{
    [Header("Throw Settings")]
    public float throwForce = 10f;
    public Transform throwOrigin;

    private TrajectoryPredictor trajectoryPredictor;
    private TrajectoryVisualizer trajectoryVisualizer;
    private Rigidbody rb;

    //private bool isEquipped = false;
    private bool isThrown = false;

    private void Start()
    {
        trajectoryPredictor = GetComponent<TrajectoryPredictor>();
        trajectoryVisualizer = FindObjectOfType<TrajectoryVisualizer>(); // Ensure only one visualizer
        rb = GetComponent<Rigidbody>();    }

    private void Update()
    {
        if (isThrown) return;

        if (gameObject.GetComponent<ItemInteractable>().isHeld)
        {
            if (Input.GetMouseButton(0))
            {
                ShowTrajectoryPreview();
            }
            else if (Input.GetMouseButtonUp(0))
            {
                ThrowObject();
                trajectoryVisualizer.HideTrajectory();
            }
        }
    }

    private void ShowTrajectoryPreview()
    {
        Vector3 initialVelocity = CalculateThrowVelocity();
        List<Vector3> trajectoryPoints = trajectoryPredictor.CalculateTrajectory(throwOrigin.position, initialVelocity);
        trajectoryVisualizer.ShowTrajectory(trajectoryPoints);
    }

    private Vector3 CalculateThrowVelocity()
    {
        Vector3 throwDirection = (transform.forward + transform.up).normalized;
        return throwDirection * throwForce;
    }

    private void ThrowObject()
    {
        rb.isKinematic = false;
        rb.gameObject.GetComponent<Collider>().enabled = true;
        rb.velocity = CalculateThrowVelocity();
        isThrown = true;
    }
}