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
    private float leftpos;
    private float rightpos;

    private float pianoWidth;

    public float GetWidth()
    {
        leftpos = LeftNoteAnchor.LeftAnchor.x;
        rightpos = RightNoteAnchor.RightAnchor.x;

        pianoWidth = Mathf.Abs(leftpos - rightpos);
        
        Debug.Log("피아노 너비: " + pianoWidth);

        return pianoWidth;
    }
}