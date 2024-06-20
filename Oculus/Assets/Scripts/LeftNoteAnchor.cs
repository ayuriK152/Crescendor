using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeftNoteAnchor : MonoBehaviour
{
    [SerializeField]
    private GameObject leftanchor;

    [SerializeField]
    OVRHand ovrhand;

    Vector3 HandPos;

    private void Update()
    {
        if (OVRInput.GetDown(OVRInput.Button.Three))
        {
            LeftHandPosition();
        }
    }

    public void LMove(Vector3 handpos)
    {
        leftanchor.transform.position = handpos;

        PlayerPrefs.SetFloat("trans_LeftAnchor", handpos.x);
        PlayerPrefs.SetFloat("trans_LeftY", handpos.y);
        PlayerPrefs.SetFloat("trans_LeftZ", handpos.z);
    }

    public void LeftHandPosition()
    {
        HandPos = ovrhand.PointerPose.transform.position;

        LMove(HandPos);
    }
}