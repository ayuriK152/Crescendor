using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingController : MonoBehaviour
{
    [SerializeField]
    Slider progressBar;

    [SerializeField]
    TextMeshProUGUI tmp;

    [SerializeField]
    CanvasGroup canvas;

    static string nextScene = "MainMenuScene";

    public static void SceneLoading(string sceneName)
    {
        nextScene = sceneName;
        SceneManager.LoadScene("LoadingScene");
    }

    private void Start()
    {
        StartCoroutine(Loading());
    }

    IEnumerator Loading()
    {
        yield return null;

        AsyncOperation op = SceneManager.LoadSceneAsync(nextScene);
        op.allowSceneActivation = false;

        float timer = 0.0f;

        while (!op.isDone)
        {
            yield return null;

            timer += Time.unscaledDeltaTime;
            tmp.text = Mathf.Lerp(op.progress * 100, 100, timer).ToString() + "%";

            if (op.progress < 0.9f)
            {
                progressBar.value = op.progress;
            }

            else
            {
                timer += Time.unscaledDeltaTime;
                progressBar.value = Mathf.Lerp(0.9f, 1, timer);
                canvas.alpha -= 0.01f;

                if (progressBar.value >= 1)
                {
                    op.allowSceneActivation = true;

                    yield break;
                }
            }
        }
    }
}
