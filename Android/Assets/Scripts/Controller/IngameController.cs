using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class IngameController : MonoBehaviour
{
    #region Public Members
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
    #endregion

    #region Protected Members
    protected int _tempoMapIdx = 0;
    protected int _beatMapIdx = 0;
    protected int[] _initInputTiming = new int[88];

    protected List<Material> _vPianoKeyMat = new List<Material>();
    protected List<GameObject> _vPianoKeyObj = new List<GameObject>();
    protected List<SpriteRenderer> _vPianoKeyEffect = new List<SpriteRenderer>();
    protected Color[] _vPianoKeyEffectColors = new Color[3];
    protected List<int> _correctNoteKeys = new List<int>();

    protected IngameUIController _uiController;
    #endregion

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

        _vPianoKeyEffectColors[0] = new Color(0.0f, 0.0f, 0.0f, 0.0f);
        _vPianoKeyEffectColors[1] = new Color(1.0f, 0.0f, 0.0f, 1.0f);
        _vPianoKeyEffectColors[2] = new Color(0.0f, 1.0f, 0.0f, 1.0f);

        Transform[] tempVPianoMat = GameObject.Find("VirtualPiano").GetComponentsInChildren<Transform>();
        foreach (Transform t in tempVPianoMat)
        {
            MeshRenderer tempVPianoKeyMat;
            if (t.TryGetComponent<MeshRenderer>(out tempVPianoKeyMat))
            {
                _vPianoKeyObj.Add(t.gameObject);
                _vPianoKeyEffect.Add(t.transform.Find("LanePressEffect").GetComponent<SpriteRenderer>());
                _vPianoKeyEffect[_vPianoKeyEffect.Count - 1].color = _vPianoKeyEffectColors[0];
                _vPianoKeyMat.Add(tempVPianoKeyMat.material);
            }
        }
    }

    public void UpdateTempo()
    {
        if (_tempoMapIdx == Managers.Midi.song.tempoMap.Count)
            return;
        if (Managers.Midi.song.tempoMap[_tempoMapIdx].deltaTime - currentDeltaTime > 0)
            return;
        tempo = Managers.Midi.song.tempoMap[_tempoMapIdx].tempo;
        _uiController.UpdateTempoText();
        _tempoMapIdx += 1;
    }

    public void UpdateBeat()
    {
        if (_beatMapIdx == Managers.Midi.song.beatMap.Count)
            return;
        if (Managers.Midi.song.beatMap[_beatMapIdx].deltaTime - currentDeltaTime > 0)
            return;
        Managers.Midi.beat = new KeyValuePair<int, int>(Managers.Midi.song.beatMap[_beatMapIdx].numerator, Managers.Midi.song.beatMap[_beatMapIdx].denominator);
        _uiController.UpdateBeatText();
        _beatMapIdx += 1;
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
            _vPianoKeyMat[keyNum].color = new Color(1, 0, 0);
        }
        else
        {
            _vPianoKeyMat[keyNum].color = new Color(0.5f, 0, 0);
        }
        if (!_correctNoteKeys.Contains(keyNum) && _vPianoKeyEffect[keyNum].color != _vPianoKeyEffectColors[2])
            _vPianoKeyEffect[keyNum].color = _vPianoKeyEffectColors[1];
        else
            _vPianoKeyEffect[keyNum].color = _vPianoKeyEffectColors[2];
    }

    void TurnOffHighlight(int keyNum)
    {
        if (!Managers.Midi.BlackKeyJudge(keyNum + 1))
        {
            _vPianoKeyMat[keyNum].color = new Color(1, 1, 1);
        }
        else
        {
            _vPianoKeyMat[keyNum].color = new Color(0, 0, 0);
        }
        _vPianoKeyEffect[keyNum].color = _vPianoKeyEffectColors[0];
    }
}
