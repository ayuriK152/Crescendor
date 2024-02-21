using Meta.WitAi.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using UnityEngine.SceneManagement;

public class PianoWidth : MonoBehaviour
{
    GameObject leftNoteAnchor;
    GameObject rightNoteAnchor;

    public float pianoWidth;

    private void Awake()
    {
        leftNoteAnchor = GameObject.Find("LeftNoteAnchor");
        rightNoteAnchor = GameObject.Find("RightNoteAnchor");

        DontDestroyOnLoad(leftNoteAnchor);
        DontDestroyOnLoad(rightNoteAnchor);
    }

    public float GetWidth()
    {
        leftNoteAnchor = GameObject.Find("LeftNoteAnchor");
        rightNoteAnchor = GameObject.Find("RightNoteAnchor");

        Transform leftpos = leftNoteAnchor.transform;
        Transform rightpos = rightNoteAnchor.transform;

        pianoWidth = Mathf.Abs(leftpos.position.x - rightpos.position.x);

        return pianoWidth;
    }

    public void LoadScene()
    {
        SceneManager.LoadScene("ActualModScene");
    }
}


