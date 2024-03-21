using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngameController : MonoBehaviour
{
    protected List<Material> vPianoKeyMat = new List<Material>();
    protected List<GameObject> vPianoKeyObj = new List<GameObject>();

    protected void Init()
    {
        Transform[] tempVPianoMat = GameObject.Find("VirtualPiano").GetComponentsInChildren<Transform>();
        foreach (Transform t in tempVPianoMat)
        {
            MeshRenderer tempVPianoKeyMat;
            if (t.TryGetComponent<MeshRenderer>(out tempVPianoKeyMat))
            {
                vPianoKeyObj.Add(t.gameObject);
                vPianoKeyMat.Add(tempVPianoKeyMat.material);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    protected IEnumerator ToggleKeyHighlight()
    {
        for (int i = 0; i < 88; i++)
        {
            if (Managers.Input.keyChecks[i])
                TurnOnHighlight(i);
            else
                TurnOffHighlight(i);
        }
        yield return null;
    }

    void TurnOnHighlight(int keyNum)
    {
        if (!Managers.Midi.BlackKeyJudge(keyNum + 1))
        {
            vPianoKeyMat[keyNum].color = new Color(1, 0, 0);
        }
        else
        {
            vPianoKeyMat[keyNum].color = new Color(0.5f, 0, 0);
        }
    }

    void TurnOffHighlight(int keyNum)
    {
        if (!Managers.Midi.BlackKeyJudge(keyNum + 1))
        {
            vPianoKeyMat[keyNum].color = new Color(1, 1, 1);
        }
        else
        {
            vPianoKeyMat[keyNum].color = new Color(0, 0, 0);
        }
    }
}
