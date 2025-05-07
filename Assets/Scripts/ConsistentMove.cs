using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Hands;

public class ConsistentMove : MonoBehaviour
{
    public GameObject xrOrigin;  // The XR origin or rig
    public float moveSpeed = 3.0f; // Forward speed
    public float rotationSpeed = 45.0f; // How fast it rotates in degrees per second

    XRHandSubsystem handSubsystem;
    bool shouldMove = true;
    
    void Start()
    {
        var subsystems = new List<XRHandSubsystem>();
        SubsystemManager.GetSubsystems(subsystems);
        if (subsystems.Count > 0)
            handSubsystem = subsystems[0];
    }

    void Update()
    {
        if (xrOrigin == null || handSubsystem == null)
            return;

        XRHand left = handSubsystem.leftHand;
        XRHand right = handSubsystem.rightHand;

        if (left.isTracked && right.isTracked)
        {
            bool leftHandUp = IsFist(left);
            bool rightHandUp = IsFist(right);

            // If both hands are raised, stop movement
            shouldMove = !(leftHandUp && rightHandUp);
        }

        // Apply rotation (optional: can tie this to gestures too)
        float horizontalInput = Input.GetAxis("Horizontal");
        xrOrigin.transform.Rotate(0, horizontalInput * rotationSpeed * Time.deltaTime, 0);

        // Move forward only if allowed
        if (shouldMove)
        {
            Vector3 forward = xrOrigin.transform.forward.normalized;
            xrOrigin.transform.position += forward * moveSpeed * Time.deltaTime;
        }
    }

    XRHandJointID GetBaseJointForTip(XRHandJointID tip)
    {
        return tip switch
        {
            XRHandJointID.IndexTip => XRHandJointID.IndexIntermediate,
            XRHandJointID.MiddleTip => XRHandJointID.MiddleIntermediate,
            XRHandJointID.RingTip => XRHandJointID.RingIntermediate,
            XRHandJointID.LittleTip => XRHandJointID.LittleIntermediate,
            _ => XRHandJointID.Invalid
        };
    }

    bool IsFingerExtended(XRHand hand, XRHandJointID tip)
    {
        // Finger is extended if tip is far from its base in the local Y/upward direction
        if (hand.GetJoint(tip).TryGetPose(out var tipPose))
        {
            XRHandJointID baseJoint = GetBaseJointForTip(tip);
            if (hand.GetJoint(baseJoint).TryGetPose(out var basePose))
            {
                float dist = Vector3.Distance(tipPose.position, basePose.position);
                return dist > 0.03f;
            }
        }
        return false;
    }
    bool IsFist(XRHand hand)
    {
        return !IsFingerExtended(hand, XRHandJointID.IndexTip) &&
            !IsFingerExtended(hand, XRHandJointID.MiddleTip) &&
            !IsFingerExtended(hand, XRHandJointID.RingTip) &&
            !IsFingerExtended(hand, XRHandJointID.LittleTip);
    }
}
