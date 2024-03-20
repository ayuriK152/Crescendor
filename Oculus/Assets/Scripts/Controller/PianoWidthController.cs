using Meta.WitAi.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

// Option Manager 생성 이후 구조 변경 필요

public class PianoWidthController : MonoBehaviour
{
    private float leftpos;
    private float rightpos;

    private float pianoWidth;

    private float r_ypos;
    private float l_ypos;
    private float r_zpos;
    private float l_zpos;

    [SerializeField]
    GameObject UI_Confirm;
    
    private void Awake()
    {
        leftpos = GameObject.Find("LeftNoteAnchor").transform.position.x;
        rightpos = GameObject.Find("RightNoteAnchor").transform.position.x;

        r_ypos = GameObject.Find("RightNoteAnchor").transform.position.y;
        l_ypos = GameObject.Find("LeftNoteAnchor").transform.position.y;
        r_zpos = GameObject.Find("RightNoteAnchor").transform.position.z;
        l_zpos = GameObject.Find("LeftNoteAnchor").transform.position.z;
    }
    
    public void DestroyUI()
    {
        if(UI_Confirm.activeSelf == true)
        {
            UI_Confirm.SetActive(false);
        }
    }

    public void ConfirmBtn()
    {
        leftpos = PlayerPrefs.GetFloat("trans_LeftAnchor");
        rightpos = PlayerPrefs.GetFloat("trans_RightAnchor");

        pianoWidth = Mathf.Abs(leftpos - rightpos);

        PlayerPrefs.SetFloat("trans_VPianoWidth", pianoWidth);
        PlayerPrefs.SetFloat("trans_ypos", r_ypos);         
        PlayerPrefs.SetFloat("trans_zpos", r_zpos);         

        UI_Confirm.SetActive(true);
        Invoke("DestroyUI", 3f);
    }

    public void UpBtn()
    {
        r_ypos += 0.005f;
        l_ypos = r_ypos;

        GameObject.Find("LeftNoteAnchor").transform.position = new Vector3(leftpos, l_ypos, l_zpos);
        GameObject.Find("RightNoteAnchor").transform.position = new Vector3(rightpos, r_ypos, r_zpos);
    }

    public void DownBtn()
    {
        r_ypos -= 0.005f;
        l_ypos = r_ypos;

        GameObject.Find("LeftNoteAnchor").transform.position = new Vector3(leftpos, l_ypos, l_zpos);
        GameObject.Find("RightNoteAnchor").transform.position = new Vector3(rightpos, r_ypos, r_zpos);
    }

    public void LeftBtn()
    {
        leftpos -= 0.005f;
        rightpos -= 0.005f;

        GameObject.Find("LeftNoteAnchor").transform.position = new Vector3(leftpos, l_ypos, l_zpos);
        GameObject.Find("RightNoteAnchor").transform.position = new Vector3(rightpos, r_ypos, r_zpos);
    }

    public void RightBtn()
    {
        leftpos += 0.005f;
        rightpos += 0.005f;

        GameObject.Find("LeftNoteAnchor").transform.position = new Vector3(leftpos, l_ypos, l_zpos);
        GameObject.Find("RightNoteAnchor").transform.position = new Vector3(rightpos, r_ypos, r_zpos);
    }

    public void ForwardBtn()
    {
        r_zpos += 0.005f;
        l_zpos = r_zpos;

        GameObject.Find("LeftNoteAnchor").transform.position = new Vector3(leftpos, l_ypos, l_zpos);
        GameObject.Find("RightNoteAnchor").transform.position = new Vector3(rightpos, r_ypos, r_zpos);
    }

    public void BackBtn()
    {
        r_zpos -= 0.005f;
        l_zpos = r_zpos;

        GameObject.Find("LeftNoteAnchor").transform.position = new Vector3(leftpos, l_ypos, l_zpos);
        GameObject.Find("RightNoteAnchor").transform.position = new Vector3(rightpos, r_ypos, r_zpos);
    }

    public void Wide()
    {
        leftpos -= 0.005f;
        rightpos += 0.005f;

        GameObject.Find("LeftNoteAnchor").transform.position = new Vector3(leftpos, l_ypos, l_zpos);
        GameObject.Find("RightNoteAnchor").transform.position = new Vector3(rightpos, r_ypos, r_zpos);
    }

    public void Narrow()
    {
        leftpos += 0.005f;
        rightpos -= 0.005f;

        GameObject.Find("LeftNoteAnchor").transform.position = new Vector3(leftpos, l_ypos,l_zpos);
        GameObject.Find("RightNoteAnchor").transform.position = new Vector3(rightpos, r_ypos, r_zpos);
    }
}