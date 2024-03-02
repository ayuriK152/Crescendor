using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RightNoteAnchor : MonoBehaviour
{
    [SerializeField]
    private static Vector3 rightanchor;

    public static Vector3 RightAnchor { get => rightanchor; set => rightanchor = value; }

    [SerializeField]
    OVRHand ovrhand;

    Vector3 HandPos;

    private void Update()
    {
        if (OVRInput.GetDown(OVRInput.Button.One))
        {
            RightHandPosition();
        }
    }

    public void RMove(Vector3 handpos)
    {
        rightanchor = handpos;
        this.transform.position = handpos;
    }

    public void RightHandPosition()
    {
        HandPos = ovrhand.PointerPose.transform.position;

        RMove(HandPos);
    }
}