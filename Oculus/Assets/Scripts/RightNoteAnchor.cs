using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Simulation;

public class RightNoteAnchor : MonoBehaviour
{
    [SerializeField]
    GameObject anchor;

    static Vector3 HandPos;

    [SerializeField]
    OVRHand ovrhand;

    private void Update()
    {
        if(OVRInput.GetDown(OVRInput.Button.One))
        {
            RightHandPosition();
        }
    }

    public void RMove(Vector3 handpos)
    {
        anchor.transform.position = handpos;
    }

    public void RightHandPosition()
    {
        HandPos = ovrhand.PointerPose.transform.position;

        RMove(HandPos);
    }
}