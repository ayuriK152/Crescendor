using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResultUIController : MonoBehaviour
{
    public TextMeshProUGUI songTitleTMP;
    public TextMeshProUGUI songComposerTMP;
    public TextMeshProUGUI playerRankTMP;

    public TextMeshProUGUI correctMountTMP;
    public TextMeshProUGUI failMountTMP;
    public TextMeshProUGUI outlinerMountTMP;

    public TextMeshProUGUI songLengthTMP;
    public TextMeshProUGUI accuracyTMP;
    public TextMeshProUGUI wellHitRatioTMP;
    public TextMeshProUGUI tryCountTMP;

    public Image correctGraphImage;
    public Image failGraphImage;
    public Image outlinerGraphImage;

    // public Button songSelectBtn;

    ResultController _controller;

    public void BindIngameUI()
    {
        songTitleTMP = GameObject.Find("MainCanvas/ResultInfo/SongTitle").GetComponent<TextMeshProUGUI>();
        songComposerTMP = GameObject.Find("MainCanvas/ResultInfo/Composer").GetComponent<TextMeshProUGUI>();
        playerRankTMP = GameObject.Find("MainCanvas/PlayerRank/Value").GetComponent<TextMeshProUGUI>();

        correctMountTMP = GameObject.Find("MainCanvas/ResultInfo/ResultGraph/Detail/CorrectValue").GetComponent<TextMeshProUGUI>();
        failMountTMP = GameObject.Find("MainCanvas/ResultInfo/ResultGraph/Detail/FailedValue").GetComponent<TextMeshProUGUI>();
        outlinerMountTMP = GameObject.Find("MainCanvas/ResultInfo/ResultGraph/Detail/OutlinerValue").GetComponent<TextMeshProUGUI>();

        songLengthTMP = GameObject.Find("MainCanvas/ResultInfo/ResultValues/ValueSection/SongLength/Value").GetComponent<TextMeshProUGUI>();
        accuracyTMP = GameObject.Find("MainCanvas/ResultInfo/ResultValues/ValueSection/Accuracy/Value").GetComponent<TextMeshProUGUI>();
        wellHitRatioTMP = GameObject.Find("MainCanvas/ResultInfo/ResultValues/ValueSection/WellHitNote/Value").GetComponent<TextMeshProUGUI>();
        tryCountTMP = GameObject.Find("MainCanvas/ResultInfo/ResultValues/ValueSection/TryCount/Value").GetComponent<TextMeshProUGUI>();

        correctGraphImage = GameObject.Find("MainCanvas/ResultInfo/ResultGraph/CorrectGraph").GetComponent<Image>();
        failGraphImage = GameObject.Find("MainCanvas/ResultInfo/ResultGraph/FailGraph").GetComponent<Image>();
        outlinerGraphImage = GameObject.Find("MainCanvas/ResultInfo/ResultGraph/OutlinerGraph").GetComponent<Image>();

        // songSelectBtn = GameObject.Find("MainCanvas/SideBar/SongSelectBtn").GetComponent<Button>();
        // songSelectBtn.onClick.AddListener(OnClickSongSelectBtn);
    }

    /*
    void OnClickSongSelectBtn()
    {
        Managers.Scene.LoadScene(Define.Scene.SongSelectScene);
    }
    */
}
