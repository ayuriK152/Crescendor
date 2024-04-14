using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// 앱 실행 시작 시 Loading 화면

public class LoadingController : MonoBehaviour
{
    [SerializeField]
    Slider progressBar;

    [SerializeField]
    TextMeshProUGUI tmp;

    private Sprite img;

    private void Start()
    {
        StartCoroutine(Loading());

        img = GetComponent<Sprite>();
    }

    IEnumerator Loading()
    {
        yield return null;

        AsyncOperation op = SceneManager.LoadSceneAsync("MainMenuScene");
        op.allowSceneActivation = false;

        float timer = 0.0f;

        while (!op.isDone)
        {
            yield return null;

            timer += Time.unscaledDeltaTime;
            tmp.text = Mathf.Lerp(90, 100, timer).ToString() + "%";

            if (op.progress < 0.9f)
            {
                progressBar.value = op.progress;
            }

            else
            {
                timer += Time.unscaledDeltaTime;
                progressBar.value = Mathf.Lerp(0.9f, 1, timer);

                if (progressBar.value >= 1)
                {
                    op.allowSceneActivation = true;

                    yield break;
                }
            }
        }
    }
}
