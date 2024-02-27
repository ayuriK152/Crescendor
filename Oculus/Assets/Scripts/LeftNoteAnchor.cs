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

    private void Update()
    {
        if (OVRInput.GetDown(OVRInput.Button.One))
        {
            LeftHandPosition();
        }
    }


    public void LMove(Vector3 handpos)
    {
        anchor.transform.position = handpos;
    }

    public void LeftHandPosition()
    {
        HandPos = ovrhand.PointerPose.transform.position;

        LMove(HandPos);
    }
}