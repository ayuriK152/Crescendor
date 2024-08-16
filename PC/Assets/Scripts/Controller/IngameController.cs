using ABCUnity.Example;
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
    public float tempoSpeed;
    public float scrollSpeed = 1.0f;
    public float notePosOffset = -2.625f * 4;
    public float widthValue = 6.0f;
    public string songTitle;

    public int passedNote;
    public int totalNote;

    public int currentDeltaTime;
    public float currentDeltaTimeF;
    #endregion

    #region Protected Members
    protected int _tempoMapIdx = 0;
    protected int _beatMapIdx = 0;
    protected int _currentBarIdx = 0;
    protected int[] _initInputTiming = new int[88];
    protected bool[] _isPlayingEffect = new bool[88];

    protected List<Material> _vPianoKeyMat = new List<Material>();
    protected List<GameObject> _vPianoKeyObj = new List<GameObject>();
    protected List<SpriteRenderer> _vPianoKeyEffect = new List<SpriteRenderer>();
    protected Color[] _vPianoKeyEffectColors = new Color[4];
    protected List<int> _correctNoteKeys = new List<int>();

    protected IngameUIController _uiController;
    protected BasicLayout sheetController;
    protected GameObject sheetObject;
    #endregion

    // Effect ���� ����
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

        currentDeltaTime = -1;
        currentDeltaTimeF = 0;

        // ����Ʈ ���� �ʱ�ȭ
        vPiano = GameObject.Find("VirtualPiano");
        correctEffect = Resources.Load<GameObject>("Effects/correct") as GameObject;
        correctOrgEffect = Resources.Load<GameObject>("Effects/correct_org") as GameObject;
        accuracyEffect = Resources.Load<GameObject>("Effects/accuracy") as GameObject;
        congratulationEffect = Resources.Load<GameObject>("Effects/congratulation") as GameObject;

        for (int i = 0; i < 88; i++)
        {
            _initInputTiming[i] = -1;
        }

        Managers.Midi.widthValue = widthValue;
        Managers.Midi.notePositionOffset = notePosOffset;
        Managers.Midi.LoadAndInstantiateMidi(songTitle);

        totalNote = Managers.Midi.notes.Count;

        _vPianoKeyEffectColors[0] = new Color(0.0f, 0.0f, 0.0f, 0.0f);
        _vPianoKeyEffectColors[1] = new Color(1.0f, 0.0f, 0.0f, 1.0f);
        _vPianoKeyEffectColors[2] = new Color(0.0f, 1.0f, 0.0f, 1.0f);
        _vPianoKeyEffectColors[3] = new Color(0.0f, 0.5f, 0.0f, 1.0f);

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

        sheetController = GameObject.Find("SheetController").GetComponent<BasicLayout>();
        sheetObject = GameObject.Find("ABCLayout");
        if (PlayerPrefs.GetInt("user_SheetShow") == 0)
            sheetObject.SetActive(false);
        else
            sheetObject.SetActive(true);

        if (!PlayerPrefs.HasKey("user_TempoSpeed"))
        {
            PlayerPrefs.SetInt("user_TempoSpeed", 10);
        }
        tempoSpeed = PlayerPrefs.GetInt("user_TempoSpeed") / 10.0f;

        Managers.Ingame.OptionChangedAction = null;
        Managers.Ingame.OptionChangedAction += ToggleSheetShow;
    }

    void ToggleSheetShow()
    {
        if (Managers.Scene.currentScene == Define.Scene.ActualModScene ||
            Managers.Scene.currentScene == Define.Scene.PracticeModScene ||
            Managers.Scene.currentScene == Define.Scene.ReplayModScene)
        {
            if (PlayerPrefs.GetInt("user_SheetShow") == 0)
                sheetObject.SetActive(false);
            else
                sheetObject.SetActive(true);
        }
    }

    public void UpdateTempo()
    {
        if (_tempoMapIdx == Managers.Midi.song.tempoMap.Count)
            return;
        if (Managers.Midi.song.tempoMap[_tempoMapIdx].deltaTime - currentDeltaTime >= 0)
            return;
        tempo = Managers.Midi.song.tempoMap[_tempoMapIdx].tempo;
        _uiController.UpdateTempoText();
        _tempoMapIdx += 1;
    }

    public void UpdateBeat()
    {
        if (_beatMapIdx == Managers.Midi.song.beatMap.Count)
            return;
        if (Managers.Midi.song.beatMap[_beatMapIdx].deltaTime - currentDeltaTime >= 0)
            return;
        Managers.Midi.beat = new KeyValuePair<int, int>(Managers.Midi.song.beatMap[_beatMapIdx].numerator, Managers.Midi.song.beatMap[_beatMapIdx].denominator);
        _uiController.UpdateBeatText();
        _beatMapIdx += 1;
    }

    public void UpdateBar()
    {
        if (_currentBarIdx >= Managers.Midi.barTiming.Count - 1)
            return;
        if (Managers.Midi.barTiming[_currentBarIdx] >= currentDeltaTime)
            return;
        int temp = _currentBarIdx / 4;
        _currentBarIdx += 1;
        if (temp != _currentBarIdx / 4)
        {
            StartCoroutine(sheetController.ShowSheetAtIndex($"SheetDatas/{songTitle}", _currentBarIdx / 4));
        }
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

    public void DisconnectPiano()
    {
        Managers.Input.DisconnectPiano();
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
        GameObject effect_clone = Instantiate(correctEffect, effectPos);
        effect_clone.transform.Rotate(new Vector3(90, 0, 0));
        effect_clone.transform.position = new Vector3(effect_clone.transform.position.x, effect_clone.transform.position.y + 1, -10.5f);
        GameObject effectOrg = Instantiate(correctOrgEffect, effectPos);
        effectOrg.transform.Rotate(new Vector3(90, 0, 0));
        effectOrg.transform.position = new Vector3(effect_clone.transform.position.x, effect_clone.transform.position.y + 1, -10.5f);

    }

    protected ParticleSystem AccurayEffect()
    {
        Transform camera = GameObject.FindWithTag("MainCamera").transform;
        GameObject effect_clone = Instantiate(accuracyEffect, camera);

        // ���� ����
        float range_x = UnityEngine.Random.Range(-5, 5);
        float range_z = UnityEngine.Random.Range(0, 4);

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
