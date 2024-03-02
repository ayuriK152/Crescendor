using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeftNoteAnchor : MonoBehaviour
{
    [SerializeField]
    private static Vector3 leftanchor;

    public static Vector3 LeftAnchor { get => leftanchor; set => leftanchor = value; }

    [SerializeField]
    OVRHand ovrhand;

    Vector3 HandPos;
    private void Update()
    {
        if (OVRInput.GetDown(OVRInput.Button.One))
        {
            LeftHandPosition();
        }
    }

    public void LMove(Vector3 handpos)
    {
        leftanchor = handpos;
        Debug.Log("¾ÞÄ¿ ÀÌµ¿: " + leftanchor);
        this.transform.position = handpos;
    }

    public void LeftHandPosition()
    {
        HandPos = ovrhand.PointerPose.transform.position;

        LMove(HandPos);
    }
}