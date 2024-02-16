using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveNote : MonoBehaviour
{
    [SerializeField]
    NoteAnchor anchor;

    [SerializeField]
    OVRHand ovrhand;

    Vector3 HandPos;

    public void HandPosition()
    {
        HandPos = ovrhand.PointerPose.transform.position;

        anchor.Move(HandPos);
    }
}
