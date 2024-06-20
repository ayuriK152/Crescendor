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
    #endregion

    public void BindIngameUI()
    {
        base.BindIngameUI();
        accuracyTMP = GameObject.Find("MainCanvas/Accuracy/Value").GetComponent<TextMeshProUGUI>();
        introCountTMP = GameObject.Find("MainCanvas/IntroTimeCount").GetComponent<TextMeshProUGUI>();

        _correctGraph = GameObject.Find("MainCanvas/Accuracy/DetailGraph/Correct/Graph").GetComponent<Image>();
        _failGraph = GameObject.Find("MainCanvas/Accuracy/DetailGraph/Fail/Graph").GetComponent<Image>();
        _correctGraph.fillAmount = 0;
        _failGraph.fillAmount = 0;
        _correctTMP = GameObject.Find("MainCanvas/Accuracy/DetailGraph/Correct/Value").GetComponent<TextMeshProUGUI>();
        _failTMP = GameObject.Find("MainCanvas/Accuracy/DetailGraph/Fail/Value").GetComponent<TextMeshProUGUI>();

        _controller = Managers.Ingame.currentController as ActualModController;

        _exitBtn.onClick.AddListener(OnClickExitBtn);
    }

    public void UpdateAccuracy()
    {
        accuracyTMP.text = $"{Convert.ToInt32((_controller as ActualModController).currentAcc * 10000.0f) / 100.0f}%";
        _correctGraph.fillAmount = (float)(_controller as ActualModController).currentCorrect / ((_controller as ActualModController).currentFail + (_controller as ActualModController).currentCorrect);
        _correctTMP.text = $"{Math.Truncate(_correctGraph.fillAmount * 100.0f) * 100.0f}%";
        _failGraph.fillAmount = (float)(_controller as ActualModController).currentFail / ((_controller as ActualModController).currentFail + (_controller as ActualModController).currentCorrect);
        _failTMP.text = $"{Math.Truncate(_failGraph.fillAmount * 100.0f) * 100.0f}%";
    }

    protected override void OnClickExitBtn()
    {
        Managers.Input.keyAction = null;
        Managers.CleanManagerChilds();
        Managers.Scene.LoadScene(Define.Scene.SongSelectScene);
    }
}
