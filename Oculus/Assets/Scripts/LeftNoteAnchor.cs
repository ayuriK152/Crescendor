using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeftNoteAnchor : MonoBehaviour
{
    [SerializeField]
    GameObject anchor;

    static Vector3 HandPos;

    [SerializeField]
    OVRHand ovrhand;


    public void LMove(Vector3 HandPos)
    {
        anchor.transform.position = HandPos;
    }

    public void LeftHandPosition()
    {
        HandPos = ovrhand.PointerPose.transform.position;

        LMove(HandPos);
    }
}