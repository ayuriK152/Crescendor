using Melanchall.DryWetMidi.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngameController : MonoBehaviour
{
    protected List<Material> vPianoKeyMat = new List<Material>();
    protected List<GameObject> vPianoKeyObj = new List<GameObject>();
    protected List<SpriteRenderer> vPianoKeyEffect = new List<SpriteRenderer>();
    protected Color[] vPianoKeyEffectColors = new Color[3];
    protected List<int> correctNoteKeys = new List<int>();

    protected void Init()
    {
        vPianoKeyEffectColors[0] = new Color(0.0f, 0.0f, 0.0f, 0.0f);
        vPianoKeyEffectColors[1] = new Color(1.0f, 0.0f, 0.0f, 1.0f);
        vPianoKeyEffectColors[2] = new Color(0.0f, 1.0f, 0.0f, 1.0f);

        Transform[] tempVPianoMat = GameObject.Find("VirtualPiano").GetComponentsInChildren<Transform>();
        foreach (Transform t in tempVPianoMat)
        {
            MeshRenderer tempVPianoKeyMat;
            if (t.TryGetComponent<MeshRenderer>(out tempVPianoKeyMat))
            {
                vPianoKeyObj.Add(t.gameObject);
                vPianoKeyEffect.Add(t.transform.Find("LanePressEffect").GetComponent<SpriteRenderer>());
                vPianoKeyEffect[vPianoKeyEffect.Count - 1].color = vPianoKeyEffectColors[0];
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
        if (!correctNoteKeys.Contains(keyNum) && vPianoKeyEffect[keyNum].color != vPianoKeyEffectColors[2])
            vPianoKeyEffect[keyNum].color = vPianoKeyEffectColors[1];
        else
            vPianoKeyEffect[keyNum].color = vPianoKeyEffectColors[2];
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
        vPianoKeyEffect[keyNum].color = vPianoKeyEffectColors[0];
    }
}
