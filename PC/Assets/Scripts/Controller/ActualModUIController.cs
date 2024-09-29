using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ActualModUIController : IngameUIController
{
    #region Public Members
    public TextMeshProUGUI accuracyTMP;
    public TextMeshProUGUI introCountTMP;
    #endregion

    #region Private Members
    private TextMeshProUGUI _correctTMP;
    private TextMeshProUGUI _failTMP;
    private Image _correctGraph;
    private Image _failGraph;
    private Transform _metronomePoint;
    #endregion

    public void BindIngameUI()
    {
        base.BindIngameUI();
        accuracyTMP = GameObject.Find("MainCanvas/Accuracy/Value").GetComponent<TextMeshProUGUI>();
        introCountTMP = GameObject.Find("MainCanvas/IntroTimeCount").GetComponent<TextMeshProUGUI>();

        _correctGraph = GameObject.Find("MainCanvas/Accuracy/DetailGraph/Correct/Graph").GetComponent<Image>();
        _failGraph = GameObject.Find("MainCanvas/Accuracy/DetailGraph/Fail/Graph").GetComponent<Image>();
        _correctGraph.fillAmount = 0.0f;
        _failGraph.fillAmount = 0.0f;
        _correctTMP = GameObject.Find("MainCanvas/Accuracy/DetailGraph/Correct/Value").GetComponent<TextMeshProUGUI>();
        _failTMP = GameObject.Find("MainCanvas/Accuracy/DetailGraph/Fail/Value").GetComponent<TextMeshProUGUI>();
        _metronomePoint = GameObject.Find("MainCanvas/Metronome/Point").transform;

        _controller = Managers.Ingame.currentController as ActualModController;

        _resumeBtn.onClick.AddListener(TogglePausePanel);
        _exitBtn.onClick.AddListener(OnClickExitBtn);

        Managers.Input.keyAction -= InputKeyEvent;
        Managers.Input.keyAction += InputKeyEvent;
    }

    public void UpdateAccuracy()
    {
        if ((_controller as ActualModController).currentCorrect == 0 && (_controller as ActualModController).currentFail == 0)
        {
            accuracyTMP.text = $"100%";
            _correctGraph.fillAmount = 0;
            _failGraph.fillAmount = 0;
        }
        else
        {
            accuracyTMP.text = $"{Convert.ToInt32((_controller as ActualModController).currentAcc * 10000.0f) / 100.0f}%";
            _correctGraph.fillAmount = (float)(_controller as ActualModController).currentCorrect / ((_controller as ActualModController).currentFail + (_controller as ActualModController).currentCorrect);
            _failGraph.fillAmount = (float)(_controller as ActualModController).currentFail / ((_controller as ActualModController).currentFail + (_controller as ActualModController).currentCorrect);
        }
        _correctTMP.text = $"{Math.Truncate(_correctGraph.fillAmount * 100.0f)}%";
        _failTMP.text = $"{Math.Truncate(_failGraph.fillAmount * 100.0f)}%";
    }

    public void UpdateMetronomeUI(float value)
    {
        _metronomePoint.GetComponent<RectTransform>().anchoredPosition = new Vector2(value * 250, _metronomePoint.GetComponent<RectTransform>().anchoredPosition.y);
    }

    public void ResetMetronomePointPos()
    {
        _metronomePoint.transform.position = new Vector2(0, _metronomePoint.transform.position.y);
    }

    protected override void OnClickExitBtn()
    {
        Managers.Input.keyAction = null;
        Managers.CleanManagerChilds();
        Managers.Scene.LoadScene(Define.Scene.SongSelectScene);
    }
}
