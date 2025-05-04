using UnityEngine;
using System.Collections.Generic;

public class ArrowPointer : MonoBehaviour
{
    public Camera xrCamera;
    public RectTransform arrowImage;
    public List<Transform> checkpoints;
    
    private int currentCheckpoint = 0;

    void Update()
    {
        if (!xrCamera) return;

        // Step 1: Choose a position in front of the camera
        Vector3 offset = xrCamera.transform.forward * 0.5f + xrCamera.transform.up * -0.25f; 
        transform.position = xrCamera.transform.position + offset;

        // Step 2: Make the canvas face the camera (but keep it upright)
        Vector3 forward = xrCamera.transform.forward;
        forward.y = 0; // remove pitch
        if (forward != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(forward, Vector3.up);
        }

        // Step 3: Point the arrow at the next checkpoint (if any)
        if (checkpoints != null && checkpoints.Count > currentCheckpoint)
        {
            Vector3 toCheckpoint = checkpoints[currentCheckpoint].position - xrCamera.transform.position;
            toCheckpoint.y = 0;

            Vector3 cameraForwardFlat = xrCamera.transform.forward;
            cameraForwardFlat.y = 0;

            float angle = Vector3.SignedAngle(cameraForwardFlat.normalized, toCheckpoint.normalized, Vector3.up);
            arrowImage.localEulerAngles = new Vector3(0, 0, -angle);
        }
    }

    public void AdvanceToNextCheckpoint()
    {
        if (currentCheckpoint < checkpoints.Count - 1)
        {
            currentCheckpoint++;
        }
    }
}
