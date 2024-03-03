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

    private void Awake()
    {
        leftpos = GameObject.Find("LeftNoteAnchor").transform.position.x;
        rightpos = GameObject.Find("RightNoteAnchor").transform.position.x;
    }

    public void GetWidth()
    {
        // �Ƹ� Option���� ����� �ǾƳ� �ʺ� �ν� Scene�� manager�� �� ��
        
        leftpos = PlayerPrefs.GetFloat("trans_LeftAnchor");
        rightpos = PlayerPrefs.GetFloat("trans_RightAnchor");
        Debug.Log("Left: " + leftpos + " Right: " + rightpos);

        pianoWidth = Mathf.Abs(leftpos - rightpos);
        
        Debug.Log("�ν��� �ǾƳ� �ʺ�: " + pianoWidth);

        PlayerPrefs.SetFloat("trans_VPianoWidth", pianoWidth);
    }
}