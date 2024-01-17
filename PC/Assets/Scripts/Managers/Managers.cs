using UnityEngine;

public class Managers : MonoBehaviour
{
    static Managers _managerInstance = null;
    static Managers ManagerInstance {  get { return _managerInstance; } }

    private static InputManager _input = new InputManager();

    public static InputManager Input { get { return _input; } }

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

        Input.Init();
    }
}
