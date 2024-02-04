using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;	// UnityWebRequest사용을 위해서 적어준다.

public class NetworkTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(UnityWebRequestPatchTest());
    }
    void Update()
    {
        // StartCoroutine(UnityWebRequestGETTest());
    }

    IEnumerator UnityWebRequestGETTest()
    {
        // GET 방식
        string url = "http://15.164.2.49:3000/record";

        // UnityWebRequest에 내장되있는 GET 메소드를 사용한다.
        UnityWebRequest www = UnityWebRequest.Get(url);

        yield return www.SendWebRequest();  // 응답이 올때까지 대기한다.

        if (www.error == null)  // 에러가 나지 않으면 동작.
        {
            Debug.Log(www.downloadHandler.text);
        }
        else
        {
            Debug.Log("error");
        }
    }
    // JSON 파일 양식 선언
    public class Body{
        public int score;
        public int date;
        public string midi;
    }
    IEnumerator UnityWebRequestPatchTest()
    {
        // URL 세팅
        // PUT 방식
        string url = "http://15.164.2.49:3000/record/setscore/test/1";

        // JSON 인자 입력 부분(함수 매개변수로도 가능)
        // API의 Body 부분에 해당된다.
        Body body = new Body{
            score = 100,
            date = 20240206,
            midi = "찐막",
        };

        string json = JsonUtility.ToJson(body);
        
        // 아래부터는 복붙으로 사용해도 됨
        UnityWebRequest www = UnityWebRequest.Put(url, json);

        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json);
        www.uploadHandler = new UploadHandlerRaw(jsonToSend);
        www.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/json");

        yield return www.SendWebRequest();  // 응답이 올때까지 대기한다.

        if (www.error == null)  // 에러가 나지 않으면 동작.
        {
            Debug.Log(www.downloadHandler.text);
        }
        else
        {
            Debug.Log("error");
        }
    }
}