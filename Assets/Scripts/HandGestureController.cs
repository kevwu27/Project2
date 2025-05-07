using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Hands;

public class HandGestureController : MonoBehaviour
{
    public XRHandSubsystem handSubsystem;
    public Transform drone;

    void Start()
    {   
        Debug.Log("starting hand gesuter controller");
        var subsystems = new List<XRHandSubsystem>();
        SubsystemManager.GetSubsystems(subsystems);

        if (subsystems.Count > 0)
            handSubsystem = subsystems[0]; // Use the first available
    }

    void Update()
    {
        if (handSubsystem == null) return;

        XRHand left = handSubsystem.leftHand;
        XRHand right = handSubsystem.rightHand;

        if (!left.isTracked || !right.isTracked)
            return;

        // if (right.isTracked)
        // {   
        //     Debug.Log("track right hand");
        //     if (IsThumbPointingOut(right, true))
        //     {
        //         // Right thumb out → do something
        //         Debug.Log("thumb left!");
        //         drone.Rotate(Vector3.up, -60f * Time.deltaTime); // turn left
        //     }

        //     if (IsHandUp(right))
        //     {
        //         // Right hand raised → do something
        //         Debug.Log("right hand up!");
        //         drone.position += Vector3.up * Time.deltaTime * 3f;
        //     }
        // }

        // if (left.isTracked){
        //     Debug.Log("track left hand");
        //     if (IsThumbPointingOut(left, false))
        //     {
        //         // Left thumb out → do something
        //         Debug.Log("thumb right!");
        //         drone.Rotate(Vector3.up, 60f * Time.deltaTime); // turn right
        //     }

        //     if (IsHandUp(left))
        //     {
        //         // Left hand raised → do something
        //         Debug.Log("left hand up!");
        //         drone.position += Vector3.down * Time.deltaTime * 3f;
        //     }
        // }
      
        if (IsFingerGunDown(right))
        {
            Debug.Log("finger gun up!");
            drone.position += Vector3.up * Time.deltaTime * 3f;
        }
        else if (IsFingerGunDown(left))
        {
            Debug.Log("finger gun down!");
            drone.position += Vector3.down * Time.deltaTime * 3f;
        }
        else if (IsRightThumbPointingLeft(right))
        {
            Debug.Log("thumb left!");
            drone.Rotate(Vector3.up, -60f * Time.deltaTime); // turn left
        }
        else if (IsLeftThumbPointingRight(left))
        {
            Debug.Log("thumb right!");
            drone.Rotate(Vector3.up, 60f * Time.deltaTime); // turn right
        }

    }

    bool IsThumbPointingOut(XRHand hand, bool isRightHand)
    {
        if (
            hand.GetJoint(XRHandJointID.ThumbTip).TryGetPose(out var tip) &&
            hand.GetJoint(XRHandJointID.ThumbMetacarpal).TryGetPose(out var baseJoint)
        )
        {
            Vector3 dir = (tip.position - baseJoint.position).normalized;

            // Right hand thumb should point right (x > 0.6), Left hand thumb should point left (x < -0.6)
            return isRightHand ? dir.x > 0.6f : dir.x < -0.6f;
        }

        return false;
    }

    bool IsHandUp(XRHand hand)
    {
        if (
            hand.GetJoint(XRHandJointID.Palm).TryGetPose(out var palmPose) &&
            hand.GetJoint(XRHandJointID.Wrist).TryGetPose(out var wristPose)
        )
        {
            // Hand is above wrist
            bool isAbove = palmPose.position.y > wristPose.position.y + 0.05f;

            // Palm is roughly facing forward
            Vector3 palmNormal = palmPose.rotation * Vector3.forward;
            bool isFacingForward = Vector3.Dot(palmNormal, Vector3.forward) > 0.5f;

            return isAbove && isFacingForward;
        }

        return false;
    }

    bool IsFingerGunUp(XRHand hand)
    {
        if (
            hand.GetJoint(XRHandJointID.IndexTip).TryGetPose(out var indexTipPose) &&
            hand.GetJoint(XRHandJointID.IndexIntermediate).TryGetPose(out var indexBasePose) &&
            hand.GetJoint(XRHandJointID.ThumbTip).TryGetPose(out var thumbTipPose) &&
            hand.GetJoint(XRHandJointID.ThumbMetacarpal).TryGetPose(out var thumbBasePose) &&
            hand.GetJoint(XRHandJointID.Wrist).TryGetPose(out var wristPose)
        )
        {
            // Check if the index finger is extended and pointing outward
            Vector3 indexDir = (indexTipPose.position - indexBasePose.position).normalized;
            bool isIndexExtended = indexDir.y > 0.8f;  // Checking for upward extension

            // Optional: Check that the thumb is in a relaxed or neutral position
            Vector3 thumbDir = (thumbTipPose.position - thumbBasePose.position).normalized;
            bool isThumbNeutral = Mathf.Abs(thumbDir.x) < 0.3f && Mathf.Abs(thumbDir.y) < 0.3f;

            return isIndexExtended && isThumbNeutral;
        }

        return false;
    }



    bool IsFingerGunDown(XRHand hand)
    {
        if (
            hand.GetJoint(XRHandJointID.IndexTip).TryGetPose(out var tipPose) &&
            hand.GetJoint(XRHandJointID.IndexIntermediate).TryGetPose(out var middlePose) &&
            hand.GetJoint(XRHandJointID.MiddleTip).TryGetPose(out var middleTipPose) &&
            hand.GetJoint(XRHandJointID.RingTip).TryGetPose(out var ringTipPose) &&
            hand.GetJoint(XRHandJointID.LittleTip).TryGetPose(out var pinkyTipPose) &&
            hand.GetJoint(XRHandJointID.Wrist).TryGetPose(out var wristPose)
        )
        {
            Vector3 indexDir = (tipPose.position - middlePose.position).normalized;

            bool othersFolded =
                Vector3.Distance(middleTipPose.position, wristPose.position) < 0.08f &&
                Vector3.Distance(ringTipPose.position, wristPose.position) < 0.08f &&
                Vector3.Distance(pinkyTipPose.position, wristPose.position) < 0.08f;

            return indexDir.y < -0.6f && othersFolded;
        }

        return false;
    }


    bool IsLeftThumbPointingRight(XRHand hand)
    {
        if (
            hand.GetJoint(XRHandJointID.ThumbTip).TryGetPose(out var thumbTipPose) &&
            hand.GetJoint(XRHandJointID.ThumbMetacarpal).TryGetPose(out var thumbBasePose) &&
            hand.GetJoint(XRHandJointID.Wrist).TryGetPose(out var wristPose)
        )
        {
            Vector3 thumbDir = (thumbTipPose.position - thumbBasePose.position).normalized;

            // Left hand, thumb pointing to your right → X direction positive
            return thumbDir.x > 0.6f && Mathf.Abs(thumbDir.y) < 0.4f;
        }

        return false;
    }


    bool IsRightThumbPointingLeft(XRHand hand)
    {
        if (
            hand.GetJoint(XRHandJointID.ThumbTip).TryGetPose(out var thumbTipPose) &&
            hand.GetJoint(XRHandJointID.ThumbMetacarpal).TryGetPose(out var thumbBasePose) &&
            hand.GetJoint(XRHandJointID.Wrist).TryGetPose(out var wristPose)
        )
        {
            Vector3 thumbDir = (thumbTipPose.position - thumbBasePose.position).normalized;

            // Right hand, thumb pointing to your left → X direction negative
            return thumbDir.x < -0.6f && Mathf.Abs(thumbDir.y) < 0.4f;
        }

        return false;
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

    bool IsThumbOutward(XRHand hand)
    {
        if (hand.GetJoint(XRHandJointID.ThumbTip).TryGetPose(out var tipPose) &&
            hand.GetJoint(XRHandJointID.ThumbMetacarpal).TryGetPose(out var basePose))
        {
            float dist = Vector3.Distance(tipPose.position, basePose.position);
            return dist > 0.03f;
        }
        return false;
    }
}
