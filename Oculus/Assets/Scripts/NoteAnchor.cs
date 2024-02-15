using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteAnchor : MonoBehaviour
{
    [SerializeField]
    GameObject anchor;

    public void Move(Vector3 HandPos)
    {
        anchor.transform.position = HandPos;
    }
}