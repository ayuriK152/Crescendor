using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RightNoteAnchor : MonoBehaviour
{
    [SerializeField]
    private GameObject rightanchor;

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
        rightanchor.transform.position = handpos;

        PlayerPrefs.SetFloat("trans_RightAnchor", handpos.x);
        PlayerPrefs.SetFloat("trans_ypos", handpos.y);
        PlayerPrefs.SetFloat("trans_zpos", handpos.z);
    }

    public void RightHandPosition()
    {
        HandPos = ovrhand.PointerPose.transform.position;

        RMove(HandPos);
    }
}