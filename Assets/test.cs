using UnityEngine;

public class test : MonoBehaviour
{
    public float a;  // Scale for normal length
    public Light lightSource;  // Assign a point or directional light in the Inspector
    public float n1 = 1f;  // Refractive index of the first medium (e.g., air)
    public float n2 = 1.5f;  // Refractive index of the second medium (e.g., glass)

    private void OnDrawGizmos()
    {
        if (lightSource == null)
        {
            Debug.LogWarning("Please assign a light source.");
            return;
        }

        // Get the mesh from the MeshFilter
        Mesh mesh = GetComponent<MeshFilter>().sharedMesh;

        if (mesh == null)
            return;

        // Get the vertices and normals of the mesh
        Vector3[] vertices = mesh.vertices;
        Vector3[] normals = mesh.normals;

        // Use only the first vertex to draw the refraction
        if (vertices.Length > 0)
        {
            // Get the world space position of the first vertex
            Vector3 worldVertex = transform.TransformPoint(vertices[0]);

            // Get the world space direction of the normal
            Vector3 worldNormal = transform.TransformDirection(normals[0]);

            // Calculate the end point of the normal vector
            Vector3 normalEnd = worldVertex + worldNormal * a;

            // Draw the normal vector in green
            Gizmos.color = Color.green;
            Gizmos.DrawLine(worldVertex, normalEnd);  // Normal line

            Vector3 lightDirection;
            if (lightSource.type == LightType.Directional)
            {
                // For directional light, the light direction is the forward vector
                lightDirection = -lightSource.transform.forward;  // Reverse to point towards the light
            }
            else if (lightSource.type == LightType.Point)
            {
                // For point light, use the light's position
                lightDirection = (lightSource.transform.position - worldVertex).normalized;  // Vector from vertex to light source
            }
            else
            {
                return;
            }

            // Draw the incident light vector in blue
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(worldVertex, worldVertex + lightDirection * 2f);  // Light direction line, adjust length with * 2f

            // Calculate the angle between the normal and the light direction (incident angle θ1)
            float incidentAngle = Vector3.Angle(worldNormal, -lightDirection);  // θ1 in degrees

            // Use Snell's Law to calculate the refracted angle (θ2)
            float sinIncidentAngle = Mathf.Sin(incidentAngle * Mathf.Deg2Rad);  // sin(θ1)
            float sinRefractedAngle = (n1 / n2) * sinIncidentAngle;  // sin(θ2)

            if (Mathf.Abs(sinRefractedAngle) <= 1)
            {
                // Calculate the refracted angle in degrees (θ2)
                float refractedAngle = Mathf.Asin(sinRefractedAngle) * Mathf.Rad2Deg;

                // Calculate the refracted direction based on θ2
                Vector3 refractedDirection = Quaternion.AngleAxis(-refractedAngle, Vector3.forward) * -worldNormal;

                // Draw the refracted ray in red (only one red line for refraction)
                Gizmos.color = Color.red;
                Gizmos.DrawLine(worldVertex, worldVertex + refractedDirection * 2f);

                // Display the refracted angle at the end of the refracted ray
                UnityEditor.Handles.Label(worldVertex + refractedDirection * 2f, refractedAngle.ToString("F1") + "°");
            }
            else
            {
                // Total internal reflection occurs if sin(θ2) > 1
                Gizmos.color = Color.magenta;
                Vector3 reflectedDirection = Vector3.Reflect(lightDirection, worldNormal);

                // Draw the reflected ray (just in case)
                Gizmos.DrawLine(worldVertex, worldVertex + reflectedDirection * 2f);

                // Display total internal reflection message
                UnityEditor.Handles.Label(worldVertex + reflectedDirection * 2f, "TIR");
            }

            // Display the incident angle at the end of the normal vector
            Gizmos.color = Color.white;
            UnityEditor.Handles.Label(normalEnd, incidentAngle.ToString("F1") + "°");
        }
    }
}
