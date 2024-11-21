using System.Collections.Generic;
using UnityEngine;

public class TrajectoryPredictor : MonoBehaviour
{
    [Header("Prediction Settings")]
    public int resolution = 30; // Number of points in the trajectory path
    public float timeStep = 0.1f; // Time interval between each prediction point
    public float gravity = -9.81f; // Custom gravity for the throwable object

    // Calculates trajectory points based on an initial position and velocity
    public List<Vector3> CalculateTrajectory(Vector3 startPosition, Vector3 initialVelocity)
    {
        List<Vector3> trajectoryPoints = new List<Vector3>();
        Vector3 currentPosition = startPosition;
        Vector3 currentVelocity = initialVelocity;

        for (int i = 0; i < resolution; i++)
        {
            trajectoryPoints.Add(currentPosition);
            currentVelocity += Vector3.up * gravity * timeStep; // Update velocity due to gravity
            currentPosition += currentVelocity * timeStep; // Update position
        }

        return trajectoryPoints;
    }
}