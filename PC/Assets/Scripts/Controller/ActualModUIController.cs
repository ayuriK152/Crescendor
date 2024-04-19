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
    private Button _resumeBtn;
    private Button _optionBtn;
    private Button _exitBtn;
    #endregion

    public void BindIngameUI()
    {
        base.BindIngameUI();
        accuracyTMP = GameObject.Find("MainCanvas/Accuracy/Value").GetComponent<TextMeshProUGUI>();
        introCountTMP = GameObject.Find("MainCanvas/IntroTimeCount").GetComponent<TextMeshProUGUI>();

        _correctGraph = GameObject.Find("MainCanvas/Accuracy/DetailGraph/Correct/Graph").GetComponent<Image>();
        _failGraph = GameObject.Find("MainCanvas/Accuracy/DetailGraph/Fail/Graph").GetComponent<Image>();
        _outlinerGraph = GameObject.Find("MainCanvas/Accuracy/DetailGraph/Outliner/Graph").GetComponent<Image>();
        _correctGraph.fillAmount = 0;
        _failGraph.fillAmount = 0;
        _outlinerGraph.fillAmount = 0;

        _controller = Managers.Ingame.currentController as ActualModController;

        _disconnectBtn.onClick.AddListener(DisconnectPianoBtn);
        _resumeBtn.onClick.AddListener(TogglePausePanel);
        _exitBtn.onClick.AddListener(OnClickExitBtn);

        Managers.Input.keyAction -= InputKeyEvent;
        Managers.Input.keyAction += InputKeyEvent;
    }

    public void UpdateAccuracy()
    {
        accuracyTMP.text = $"{Convert.ToInt32((_controller as ActualModController).currentAcc * 10000.0f) / 100.0f}%";
        _correctGraph.fillAmount = (float)(_controller as ActualModController).currentCorrect / ((_controller as ActualModController).currentFail + (_controller as ActualModController).currentCorrect);
        _failGraph.fillAmount = (float)(_controller as ActualModController).currentFail / ((_controller as ActualModController).currentFail + (_controller as ActualModController).currentCorrect);
    }

    void TogglePausePanel()
    {
        pausePanelObj.SetActive(!pausePanelObj.activeSelf);

        if (pausePanelObj.activeSelf)
        {
            _controller.enabled = false;
        }
        else
        {
            _controller.enabled = true;
        }
    }

    void OnClickExitBtn()
    {
        Managers.Input.keyAction = null;
        Managers.CleanManagerChilds();
        Managers.Scene.LoadScene(Define.Scene.ResultScene);
        Managers.Scene.LoadScene(Define.Scene.SongSelectScene);
    }

    void InputKeyEvent(KeyCode keyCode, Define.InputType inputType)
    {
        switch (inputType)
        {
            case Define.InputType.OnKeyDown:
                switch (keyCode)
                {
                    case KeyCode.Escape:
                        TogglePausePanel();
                        break;
                }
                break;
            case Define.InputType.OnKeyUp:
                break;
        }
    }
}
