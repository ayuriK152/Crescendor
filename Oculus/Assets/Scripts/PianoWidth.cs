using Meta.WitAi.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using UnityEngine.SceneManagement;
using Oculus.Interaction.Throw;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PianoWidth : MonoBehaviour
{
    private GameObject leftNoteAnchor;
    private GameObject rightNoteAnchor;

    Transform leftpos;
    Transform rightpos;

    public float pianoWidth;

    public float GetWidth()
    {
        leftNoteAnchor = Resources.Load<GameObject>("Prefabs/LeftNoteAnchor");
        rightNoteAnchor = Resources.Load<GameObject>("Prefabs/RightNoteAnchor");

        leftpos = leftNoteAnchor.transform;
        rightpos = rightNoteAnchor.transform;

        Debug.Log("leftpos: " + leftpos.position.x);
        Debug.Log("rightpos: " + rightpos.position.x);

        pianoWidth = Mathf.Abs(leftpos.position.x - rightpos.position.x);
        
        Debug.Log("피아노 너비: " + pianoWidth);

        return pianoWidth;
    }

    public void LeftClicked()
    {
        float x = leftNoteAnchor.transform.position.x;
        
        x -= 0.5f;

        Debug.Log(x);
    }
}