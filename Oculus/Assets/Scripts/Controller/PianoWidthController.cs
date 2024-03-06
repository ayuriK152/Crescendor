using Meta.WitAi.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using UnityEngine.SceneManagement;
using Oculus.Interaction.Throw;
using UnityEngine.InputSystem;

// Option Manager ���� ���� ���� ���� �ʿ�

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
        // confirm ��ư�� ����� �Լ�

        leftpos = PlayerPrefs.GetFloat("trans_LeftAnchor");
        rightpos = PlayerPrefs.GetFloat("trans_RightAnchor");

        pianoWidth = Mathf.Abs(leftpos - rightpos);

        PlayerPrefs.SetFloat("trans_VPianoWidth", pianoWidth);
    }
}