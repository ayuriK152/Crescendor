/* �Ŵ��� ������Ʈ
 * �ۼ� - �̿���
 * ��� �Ŵ��� ��ü�� �����ϴ� ��ü. ���������� ���� ���忡 ��ü�ν� �����Ǵ� ������Ʈ. */

using UnityEngine;

public class Managers : MonoBehaviour
{
    static GameObject _managerObj = null;
    public static GameObject ManagerObj { get { return _managerObj; } }

    static Managers _managerInstance = null;
    public static Managers ManagerInstance {  get {  return _managerInstance; } }

    private static InputManager _input = new InputManager();
    private static MidiManager _midi = new MidiManager();
    private static UIManager _ui = new UIManager();
    private static IngameManager _ingame = new IngameManager();
    private static SceneManagerEx _scene = new SceneManagerEx();
    private static DataManager _data = new DataManager();
    private static SongManager _song = new SongManager();

    public static InputManager Input { get { return _input; } }
    public static MidiManager Midi { get { return _midi; } }
    public static UIManager UI { get { return _ui; } }
    public static IngameManager Ingame { get { return _ingame; } }
    public static SceneManagerEx Scene {  get { return _scene; } }
    public static DataManager Data { get { return _data; } }
    public static SongManager Song { get { return _song; } }

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

        /* �Ʒ� Init �޼ҵ� ȣ�� ������ �ǵ��Ǿ�����, ���Ƿ� ������ �ٲ㼭�� ����� �ȵ�
         * �ٲ���ϴ� ������ �ִٸ� ���� ���*/
        Scene.Init();
        Input.Init();
        Midi.Init();
        Data.Init();
    }

    static public void InitManagerPosition()
    {
        _managerInstance.transform.position = new Vector3(0, 0, Datas.DEFAULT_NOTE_POSITION_OFFSET);
    }

    static public void InitPostSceneLoad()
    {
        UI.Init();
        Ingame.Init();
    }

    static public void CleanManagerChilds()
    {
        foreach (Transform child in _managerInstance.transform)
        {
            Destroy(child.gameObject);
        }
    }
}
