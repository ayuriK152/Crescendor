using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class IngameController : MonoBehaviour
{
    public TextMeshProUGUI deviceText;
    public TextMeshProUGUI noteText;

    public int tempo = 120;
    public float scrollSpeed = 1.0f;
    public float notePosOffset = -2.625f;
    public float noteScale = 3.0f;
    public float widthValue = 1.5f;
    public string songTitle;

    public int passedNote;
    public int totalNote;

    public int currentDeltaTime;
    public float currentDeltaTimeF;

    protected int[] _initInputTiming = new int[88];

    protected List<Material> vPianoKeyMat = new List<Material>();
    protected List<GameObject> vPianoKeyObj = new List<GameObject>();
    protected List<SpriteRenderer> vPianoKeyEffect = new List<SpriteRenderer>();
    protected Color[] vPianoKeyEffectColors = new Color[3];
    protected List<int> correctNoteKeys = new List<int>();

    protected void Init()
    {
        songTitle = PlayerPrefs.GetString("trans_SongTitle");

        passedNote = 0;
        totalNote = 0;

        currentDeltaTime = -1;
        currentDeltaTimeF = 0;

        for (int i = 0; i < 88; i++)
        {
            _initInputTiming[i] = -1;
        }

        Managers.Midi.noteScale = noteScale;
        Managers.Midi.widthValue = widthValue;
        Managers.Midi.LoadAndInstantiateMidi(songTitle);

        totalNote = Managers.Midi.notes.Count;

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

    public void DisconnectPiano()
    {
        Managers.Input.inputDevice.StopEventsListening();
    }
}
