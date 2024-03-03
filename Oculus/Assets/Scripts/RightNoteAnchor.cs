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
        Debug.Log("¿À¸¥ÂÊ ¾ÞÄ¿: " + handpos.x);
    }

    public void RightHandPosition()
    {
        HandPos = ovrhand.PointerPose.transform.position;

        RMove(HandPos);
    }
}