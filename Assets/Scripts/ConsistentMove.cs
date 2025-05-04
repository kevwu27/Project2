using UnityEngine;

public class ConsistentMove : MonoBehaviour
{
    public GameObject xrOrigin; // Assign this in the Inspector
    public float moveSpeed = 3.0f; // Units per second

    void Update()
    {
        if (xrOrigin != null)
        {
            Vector3 forward = xrOrigin.transform.forward;
            forward.Normalize(); // Keep full 3D forward (no zeroing y-axis like before)

            xrOrigin.transform.position += forward * moveSpeed * Time.deltaTime;
        }
    }
}