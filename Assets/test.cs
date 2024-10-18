using UnityEngine;

public class test : MonoBehaviour
{
    public Transform rayStart;    // Starting point of the ray
    public Transform objPosition; // Object to check for intersection

    public float radius = 1f;     // Radius of the object (treated as a sphere)
    public float maxDistance = 10f;  // Maximum distance to march the ray
    public float stepSize = 0.5f;    // Size of each step along the ray
    public int steps = 20;           // Number of steps (marches)

    void OnDrawGizmos()
    {
        if (rayStart == null || objPosition == null) return;

        Vector3 rayOrigin = rayStart.position;  // Origin of the ray (from the object)
        Vector3 rayDir = transform.forward;     // Direction of the ray (forward from the object)

        // Visualize the ray itself as a line
        Gizmos.color = Color.red;
        Gizmos.DrawLine(rayOrigin, rayOrigin + rayDir * maxDistance);

        // Visualize the object as a sphere
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(objPosition.position, radius);

        // Perform ray marching and stop when it reaches the object
        RayMarch(rayOrigin, rayDir);
    }

    // Ray marching function to visualize sample points and check for intersections
    void RayMarch(Vector3 origin, Vector3 direction)
    {
        float distanceTraveled = 0f;  // Tracks how far we've marched along the ray
        Vector3 currentPosition = origin;

        // March the ray along the direction in small steps
        for (int i = 0; i < steps; i++)
        {
            // Move the ray position forward by the step size
            currentPosition = origin + direction * distanceTraveled;

            // Visualize each sample point with a small sphere
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(currentPosition, 0.1f);  // Draw a small sphere at each sample point

            // Check distance between the current sample point and the object
            float distanceToObj = Vector3.Distance(currentPosition, objPosition.position);

            // If the distance is less than the radius of the object, we hit the object
            if (distanceToObj < radius)
            {
                // Visualize the intersection point in yellow
                Gizmos.color = Color.yellow;
                Gizmos.DrawSphere(currentPosition, 0.15f);  // Mark the intersection point
                break;  // Stop marching since we intersected the object
            }

            // Update the distance traveled
            distanceTraveled += stepSize;

            // Stop if the maximum distance is reached
            if (distanceTraveled > maxDistance)
                break;
        }
    }

    // Function to calculate the distance between the ray start and object
    float GetDistCameraToObject()
    {
        float dist = (rayStart.position - objPosition.position).magnitude;
        return dist - radius;
    }
}
