/* 씬 매니저
 * 작성 - 이원섭
 * 씬의 정보를 가져오고, 씬 간의 전환과 같은 역할을 수행하는 객체
 * 끝에 Ex가 붙은건 유니티 엔진 내에서 이미 사용중인 SceneManager라는 스크립트가 존재하기 때문.
 * 절대로 Ex를 지워서는 안됨.*/


using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerEx
{
    public Define.Scene currentScene;

    public float progress;

    public void Init()
    { 
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public void LoadScene(Define.Scene sceneName)
    {
        // 비동기식으로 변경
        AsyncOperation op = SceneManager.LoadSceneAsync(GetSceneName(sceneName));
        op.allowSceneActivation = false;

        // System.Enum.TryParse(GetSceneName(sceneName), true, out currentScene);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        System.Enum.TryParse(SceneManager.GetActiveScene().name, true, out currentScene);
        Managers.InitPostSceneLoad();
    }

    public string GetSceneName(Define.Scene sceneName)
    {
        return System.Enum.GetName(typeof(Define.Scene), sceneName);
    }
}
