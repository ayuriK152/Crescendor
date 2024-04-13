using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// 앱 실행 시작 시 Loading 화면

public class LoadingController : MonoBehaviour
{
    [SerializeField]
    Slider progressBar;

    private void Start()
    {
        StartCoroutine(Loading());
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
