/* UI �Ŵ���
 * �ۼ� - �̿���
 * ���� ������ ���Ǵ� UI�� �ϰ� �����ϰ� ���ε��ϱ� ���� ����ϴ� ��ü */

using TMPro;
using UnityEngine;

public class UIManager
{
    public TextMeshProUGUI songTitleTMP;
    public TextMeshProUGUI songNoteMountTMP;
    public TextMeshProUGUI SongBpmTMP;
    public TextMeshProUGUI songBeatTMP;

    public void BindIngameUI()
    {
        songTitleTMP = GameObject.Find("MainCanvas/Slider/Title").GetComponent<TextMeshProUGUI>();
        songNoteMountTMP = GameObject.Find("MainCanvas/Informations/Notes/Value").GetComponent<TextMeshProUGUI>();
        SongBpmTMP = GameObject.Find("MainCanvas/Informations/BPM/Value").GetComponent<TextMeshProUGUI>();
        songBeatTMP = GameObject.Find("MainCanvas/Informations/Beat/Value").GetComponent<TextMeshProUGUI>();
    }
}
