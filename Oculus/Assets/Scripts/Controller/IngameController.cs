﻿using System.Collections;
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
    public float notePosOffset = 0;
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
    protected bool[] _isPlayingEffect = new bool[88];

    protected List<Material> _vPianoKeyMat = new List<Material>();
    protected List<GameObject> _vPianoKeyObj = new List<GameObject>();
    protected List<SpriteRenderer> _vPianoKeyEffect = new List<SpriteRenderer>();
    protected Color[] _vPianoKeyEffectColors = new Color[3];
    protected List<int> _correctNoteKeys = new List<int>();

    protected IngameUIController _uiController;
    #endregion

    // Effect 관련 변수
    private GameObject vPiano;
    private GameObject correctEffect;
    private GameObject correctOrgEffect;
    private GameObject accuracyEffect;
    private GameObject congratulationEffect;

    protected void Init()
    {
        songTitle = PlayerPrefs.GetString("trans_SongTitle");

        passedNote = 0;
        totalNote = 0;
        notePosOffset = PlayerPrefs.GetFloat("trans_zpos");

        currentDeltaTime = -1;
        currentDeltaTimeF = 0;

        // 이펙트 관련 초기화
        vPiano = GameObject.Find("VirtualPiano");
        correctEffect = Resources.Load<GameObject>("Effects/correct") as GameObject;
        correctOrgEffect = Resources.Load<GameObject>("Effects/correct_org") as GameObject;
        accuracyEffect = Resources.Load<GameObject>("Effects/accuracy") as GameObject;
        congratulationEffect = Resources.Load<GameObject>("Effects/congratulation") as GameObject;

        for (int i = 0; i < 88; i++)
        {
            _initInputTiming[i] = -1;
        }

        Managers.Midi.noteScale = noteScale * 0.1f;
        Managers.Midi.LoadAndInstantiateMidi(songTitle);
        vPiano.transform.position = Managers.Midi.noteInstantiatePoint.position;
        vPiano.transform.localScale *= Managers.Midi.widthValue;
        correctEffect.transform.localScale *= Managers.Midi.widthValue;
        correctOrgEffect.transform.localScale *= Managers.Midi.widthValue;
        accuracyEffect.transform.localScale *= Managers.Midi.widthValue;
        congratulationEffect.transform.localScale *= Managers.Midi.widthValue;

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
            {
                TurnOnHighlight(i);
            }

            else
                TurnOffHighlight(i);
        }
        yield return null;
    }

    void TurnOnHighlight(int keyNum)
    {
        if (_correctNoteKeys.Contains(keyNum) || _isPlayingEffect[keyNum])
        {
            _isPlayingEffect[keyNum] = true;
            CorrectEffect(keyNum);
            if (!Managers.Midi.BlackKeyJudge(keyNum + 1))
            {
                _vPianoKeyMat[keyNum].color = new Color(0, 1f, 0);
                _vPianoKeyEffect[keyNum].color = _vPianoKeyEffectColors[2];
            }
            else
            {
                _vPianoKeyMat[keyNum].color = new Color(0, 0.5f, 0);
                _vPianoKeyEffect[keyNum].color = _vPianoKeyEffectColors[3];
            }

        }
        else
        {
            _vPianoKeyMat[keyNum].color = new Color(1f, 0, 0);
            _vPianoKeyEffect[keyNum].color = _vPianoKeyEffectColors[1];

        }
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

    protected void CorrectEffect(int keyNum)
    {
        int octave = 0;
        int chord = 0;

        if (keyNum >= 0 && keyNum <= 2)
        {
            octave = 0;
            chord = keyNum;
        }

        else if (keyNum >= 3 && keyNum <= 86)
        {
            octave = (keyNum - 3) / 12 + 1;
            chord = (keyNum - 3) % 12;
        }

        else if (keyNum == 87)
        {
            octave = 8;
            chord = 0;
        }

        Transform effectPos = vPiano.transform.GetChild(octave).GetChild(chord);
        GameObject effect_clone = Instantiate(accuracyEffect, effectPos);
        effect_clone.transform.Rotate(new Vector3(90, 0, 0));
        effect_clone.transform.position = new Vector3(effect_clone.transform.position.x, effect_clone.transform.position.y, 0.85f);
        GameObject effectOrg = Instantiate(correctOrgEffect, effectPos);
        effectOrg.transform.Rotate(new Vector3(90, 0, 0));
        effectOrg.transform.position = new Vector3(effectOrg.transform.position.x, effectOrg.transform.position.y, 0.85f);
    }

    protected ParticleSystem AccurayEffect()
    {
        Transform camera = GameObject.FindWithTag("MainCamera").transform;
        GameObject effect_clone = Instantiate(accuracyEffect, camera);

        // 수정 예정
        float range_x = Random.Range(-5, 5);
        float range_z = Random.Range(0, 4);

        effect_clone.transform.position = new Vector3(range_x, effect_clone.transform.position.y, range_z);

        ParticleSystem particleSystem = effect_clone.GetComponent<ParticleSystem>();

        return particleSystem;
    }

    protected void CongratulationEffect()
    {
        Transform camera = GameObject.FindWithTag("MainCamera").transform;
        GameObject effect_clone = Instantiate(congratulationEffect, camera);

        effect_clone.transform.position = new Vector3(0f, 2.5f, 2f);
    }
}
