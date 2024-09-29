using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_NoteTransform : MonoBehaviour
{
    private float leftpos;
    private float rightpos;

    private float pianoWidth;

    private float r_ypos;
    private float l_ypos;
    private float r_zpos;
    private float l_zpos;

    [SerializeField]
    private TextMeshProUGUI confirmMsg;
    [SerializeField]
    private TextMeshProUGUI leftAnchorMsg;
    [SerializeField]
    private TextMeshProUGUI rightAnchorMsg;

    private void Awake()
    {
        leftpos = GameObject.Find("LeftNoteAnchor").transform.position.x;
        rightpos = GameObject.Find("RightNoteAnchor").transform.position.x;

        r_ypos = GameObject.Find("RightNoteAnchor").transform.position.y;
        l_ypos = GameObject.Find("LeftNoteAnchor").transform.position.y;
        r_zpos = GameObject.Find("RightNoteAnchor").transform.position.z;
        l_zpos = GameObject.Find("LeftNoteAnchor").transform.position.z;
        leftAnchorMsg.text = $"{leftpos}, {l_ypos}, {l_zpos}";
        rightAnchorMsg.text = $"{rightpos}, {r_ypos}, {r_zpos}";
    }

    public void ConfirmBtn()
    {
        leftpos = PlayerPrefs.GetFloat("trans_LeftAnchor");
        rightpos = PlayerPrefs.GetFloat("trans_RightAnchor");
        l_ypos = PlayerPrefs.GetFloat("trans_LeftY");
        r_ypos = PlayerPrefs.GetFloat("trans_RightY");
        l_zpos = PlayerPrefs.GetFloat("trans_LeftZ");
        r_zpos = PlayerPrefs.GetFloat("trans_RightZ");

        pianoWidth = Mathf.Abs(leftpos - rightpos);

        PlayerPrefs.SetFloat("trans_VPianoWidth", pianoWidth);
        PlayerPrefs.SetFloat("trans_xpos", leftpos);
        PlayerPrefs.SetFloat("trans_ypos", l_ypos);
        PlayerPrefs.SetFloat("trans_zpos", l_zpos);

        leftAnchorMsg.text = $"{leftpos}, {l_ypos}, {l_zpos}";
        rightAnchorMsg.text = $"{rightpos}, {r_ypos}, {r_zpos}";

        confirmMsg.text = "Confirm";

        // Confirm 버튼 누른 후에 다른 버튼 누르면 앵커 위치 겹치는 현상 해결하기
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

        GameObject.Find("LeftNoteAnchor").transform.position = new Vector3(leftpos, l_ypos, l_zpos);
        GameObject.Find("RightNoteAnchor").transform.position = new Vector3(rightpos, r_ypos, r_zpos);
    }
}
