using Meta.WitAi.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using UnityEngine.SceneManagement;
using Oculus.Interaction.Throw;
using UnityEngine.InputSystem;

// Option Manager 생성 이후 구조 변경 필요

public class PianoWidthController : MonoBehaviour
{
    private float leftpos;
    private float rightpos;

    private float pianoWidth;

    private void Awake()
    {
        leftpos = GameObject.Find("LeftNoteAnchor").transform.position.x;
        rightpos = GameObject.Find("RightNoteAnchor").transform.position.x;
    }

    public void GetWidth()
    {
        // confirm 버튼과 연결된 함수

        leftpos = PlayerPrefs.GetFloat("trans_LeftAnchor");
        rightpos = PlayerPrefs.GetFloat("trans_RightAnchor");

        pianoWidth = Mathf.Abs(leftpos - rightpos);

        PlayerPrefs.SetFloat("trans_VPianoWidth", pianoWidth);
    }
}