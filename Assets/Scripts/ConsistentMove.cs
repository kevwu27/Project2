using UnityEngine;

public class ConsistentMove : MonoBehaviour
{
    public GameObject xrOrigin;  // The XR origin or rig
    public float moveSpeed = 3.0f; // Forward speed
    public float rotationSpeed = 45.0f; // How fast it rotates in degrees per second

    void Update()
    {
        if (xrOrigin != null)
        {
            // Get input for rotation (e.g., left/right on a joystick or controller)
            float horizontalInput = Input.GetAxis("Horizontal"); // Usually A/D or Left/Right arrow keys or joystick

            // Rotate the XR Origin on the Y-axis based on input
            xrOrigin.transform.Rotate(0, horizontalInput * rotationSpeed * Time.deltaTime, 0);

            // Move in the direction the XR Origin is facing
            Vector3 forward = xrOrigin.transform.forward;
            forward.Normalize();

            // Apply consistent forward movement
            xrOrigin.transform.position += forward * moveSpeed * Time.deltaTime;
        }
    }
}
