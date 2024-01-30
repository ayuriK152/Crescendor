/* 매니저 오브젝트
 * 작성 - 이원섭
 * 모든 매니저 객체를 관리하는 객체. 실질적으로 게임 월드에 객체로써 생성되는 오브젝트. */

using UnityEngine;

public class Managers : MonoBehaviour
{
    static GameObject _managerObj = null;
    public static GameObject ManagerObj { get { return _managerObj; } }

    static Managers _managerInstance = null;
    public static Managers ManagerInstance {  get { return _managerInstance; } }

    private static InputManager _input = new InputManager();
    private static MidiManager _midi = new MidiManager();
    private static UIManager _ui = new UIManager();
    private static IngameManager _ingame = new IngameManager();
    private static SceneManagerEx _scene = new SceneManagerEx();

    public static InputManager Input { get { return _input; } }
    public static MidiManager Midi { get { return _midi; } }
    public static UIManager UI { get { return _ui; } }
    public static IngameManager Ingame { get { return _ingame; } }
    public static SceneManagerEx Scene {  get { return _scene; } }

    void Awake()
    {
        if (_managerInstance != null)
        {
            Destroy(gameObject);
            return;
        }
        Init();
    }

    void Update()
    {
        Input.Update();
    }

    static void Init()
    {
        if (_managerInstance == null)
        {
            GameObject go = GameObject.Find("@Manager");
            if (go == null)
            {
                go = new GameObject { name = "@Manager" };
                go.AddComponent<Managers>();
            }

            DontDestroyOnLoad(go);
            _managerInstance = go.GetComponent<Managers>();
        }

        /* 아래 Init 메소드 호출 순서는 의도되었으며, 임의로 순서를 바꿔서는 절대로 안됨
         * 바꿔야하는 이유가 있다면 상의 요망*/
        Input.Init();
        Midi.Init();
        Ingame.Init();
        Scene.Init();
    }
}
