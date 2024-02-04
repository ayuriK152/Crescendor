/* UI 매니저
 * 작성 - 이원섭
 * 여러 씬에서 사용되는 UI를 일괄 관리하고 바인딩하기 위해 사용하는 객체 */

using System;
using Unity.VisualScripting;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static Define;

public class UIManager
{
    public object currentUIController;

    int _order = 10;
    Stack<UI_Popup> _popupStack = new Stack<UI_Popup>();
    UI_Scene _sceneUI = null;

    public void Init()
    {
        if (currentUIController != null)
        {
            Type type = currentUIController.GetType();
            switch (type.FullName)
            {
                case "PracticeModUIController":
                    GameObject.Destroy(Managers.ManagerInstance.GetComponent<PracticeModUIController>());
                    break;
                case "ActualModUIController":
                    GameObject.Destroy(Managers.ManagerInstance.GetComponent<ActualModUIController>());
                    break;
                case "ResultUIController":
                    GameObject.Destroy(Managers.ManagerInstance.GetComponent<ResultUIController>());
                    break;
            }
        }

        switch (Managers.Scene.currentScene)
        {
            case Scene.PracticeModScene:
                currentUIController = Managers.ManagerInstance.GetComponent<PracticeModUIController>();
                if (currentUIController == null)
                    currentUIController = Managers.ManagerInstance.AddComponent<PracticeModUIController>();
                break;

            case Scene.ActualModScene:
                currentUIController = Managers.ManagerInstance.GetComponent<ActualModUIController>();
                if (currentUIController == null)
                    currentUIController = Managers.ManagerInstance.AddComponent<ActualModUIController>();
                break;

            case Scene.ResultScene:
                currentUIController = Managers.ManagerInstance.GetComponent<ResultUIController>();
                if (currentUIController == null)
                    currentUIController = Managers.ManagerInstance.AddComponent<ResultUIController>();
                break;
        }
    }

    public GameObject Root
    {
        get
        {
            GameObject root = GameObject.Find("@UI");
            if (root == null)
                root = new GameObject { name = "@UI" };
            return root;
        }
    }

    public void SetCanvas(GameObject go, bool sort = true)
    {
        Canvas canvas = Util.GetOrAddComponent<Canvas>(go);
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.overrideSorting = true;

        if (sort)
        {
            canvas.sortingOrder = _order;
            _order++;
        }
        else
        {
            canvas.sortingOrder = 0;
        }
    }

    public T MakeSubItem<T>(Transform parent = null, string name = null) where T : UI_Base
    {
        if (string.IsNullOrEmpty(name))
            name = typeof(T).Name;

        GameObject go = Managers.Resource.Instantiate($"UI/SubItem/{name}");
        if (parent != null)
            go.transform.SetParent(parent);

        return Util.GetOrAddComponent<T>(go);
    }

    public T ShowSceneUI<T>(string name = null) where T : UI_Scene
    { 
        if (string.IsNullOrEmpty(name))
            name = typeof(T).Name;

        GameObject go = Managers.Resource.Instantiate($"UI/Scene/{name}");
        T sceneUI = Util.GetOrAddComponent<T>(go);
        _sceneUI = sceneUI;

        go.transform.SetParent(Root.transform);

        return sceneUI;
    }

    public T ShowPopupUI<T>(string name = null) where T : UI_Popup
    {
        if (string.IsNullOrEmpty(name))
            name = typeof(T).Name;

        GameObject go = Managers.Resource.Instantiate($"UI/Popup/{name}");
        T popup = Util.GetOrAddComponent<T>(go);
        _popupStack.Push(popup);

        go.transform.SetParent(Root.transform);

        return popup;
    }

    public void ClosePopupUI(UI_Popup popup)
    {
        if (_popupStack.Count == 0)
            return;

        if (_popupStack.Peek() != popup)
        {
            Debug.Log("Close Popup Failed!");
            return;
        }

        ClosePopupUI();
    }

    public void ClosePopupUI()
    {
        if (_popupStack.Count == 0)
            return;

        UI_Popup popup = _popupStack.Pop();
        Managers.Resource.Destroy(popup.gameObject);
        popup = null;
        _order--;
    }

    public void CloseAllPopupUI()
    {
        while (_popupStack.Count > 0)
            ClosePopupUI();
    }
}
