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
    private Image _correctGraph;
    private Image _failGraph;
    private Image _outlinerGraph;
    #endregion

    public void BindIngameUI()
    {
        base.BindIngameUI();
        accuracyTMP = GameObject.Find("UI/Canvas/Accuracy/Value").GetComponent<TextMeshProUGUI>();
        introCountTMP = GameObject.Find("UI/Canvas/IntroTimeCount").GetComponent<TextMeshProUGUI>();

        _correctGraph = GameObject.Find("UI/Canvas/Accuracy/DetailGraph/Correct/Graph").GetComponent<Image>();
        _failGraph = GameObject.Find("UI/CanvasAccuracy/DetailGraph/Fail/Graph").GetComponent<Image>();
        //_outlinerGraph = GameObject.Find("MainCanvas/Accuracy/DetailGraph/Outliner/Graph").GetComponent<Image>();
        _correctGraph.fillAmount = 0;
        _failGraph.fillAmount = 0;
        //_outlinerGraph.fillAmount = 0;

        _controller = Managers.Ingame.currentController as ActualModController;
        _exitBtn.onClick.AddListener(OnClickExitBtn);
    }

    public void UpdateAccuracy()
    {
        accuracyTMP.text = $"{Convert.ToInt32((_controller as ActualModController).currentAcc * 10000.0f) / 100.0f}%";
        _correctGraph.fillAmount = (float)(_controller as ActualModController).currentCorrect / ((_controller as ActualModController).currentFail + (_controller as ActualModController).currentCorrect);
        _failGraph.fillAmount = (float)(_controller as ActualModController).currentFail / ((_controller as ActualModController).currentFail + (_controller as ActualModController).currentCorrect);
    }

    protected override void OnClickExitBtn()
    {
        Managers.Input.keyAction = null;
        Managers.CleanManagerChilds();
        Managers.Scene.LoadScene(Define.Scene.SongSelectScene);
    }
}
