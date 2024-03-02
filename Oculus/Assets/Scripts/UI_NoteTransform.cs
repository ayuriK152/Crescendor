using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_NoteTransform : MonoBehaviour
{
    public Button LeftBtn;

    GameObject leftNoteAnchor;

    private void Start()
    {
        leftNoteAnchor = GameObject.Find("LeftNoteAnchor");
        LeftBtn.onClick.AddListener(LeftClicked);
    }

    public void LeftClicked()
    {
        float x = leftNoteAnchor.transform.position.x;

        x -= 0.5f;

        Debug.Log("x: " + x);
    }
}
