using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RightNoteAnchor : MonoBehaviour
{
    [SerializeField]
    GameObject anchor;

    static Vector3 HandPos;

    [SerializeField]
    OVRHand ovrhand;


    public void RMove(Vector3 HandPos)
    {
        anchor.transform.position = HandPos;
    }

    public void RightHandPosition()
    {
        HandPos = ovrhand.PointerPose.transform.position;

        RMove(HandPos);
    }
}