using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UIManager
{
    //sort order 관리
    int order = 10; //최근에 사용한 Order를 저장하는 변수
    
    //외부에서 popup 등의 ui가 켜질때 UIManager에게 SetCanvas를 요청하여 order를 정해달라는 함수
    public void SetCanvas(GameObject go, bool sort = true)
    {
        Canvas canvas = Util.GetOrAddComponent<Canvas>(go);
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.overrideSorting = true;

        if (sort)
        {
            canvas.sortingOrder = order;
            order++;
        }
        else
        {
            //popup이랑 연관 없는 그냥 ui
            canvas.sortingOrder = 0;
        }
    }
    
    #region UI_Popup
    // Stack<UI_Popup> _popupStack = new Stack<UI_Popup>();     //pop 목록 :  가장 마지막에 띄운 popup이 삭제되어야 하므로 stack으로 관리
    //
    // //1. 팝업 띄우기
    // public T ShowPopupUI<T>(string name = null) where T : UI_Popup //name : prefab 이름, T : scirpt class명
    // {
    //     if (string.IsNullOrEmpty(name)) //prefab 이름을 명시하지 않는 경우 T type(script class 명)과 동일한 이름의 prefab을 가져온다.
    //     {
    //         name = typeof(T).Name;
    //     }
    //
    //     GameObject go = Managers.Resource.Instantiate($"UI/Popup/{name}");
    //     T popup = go.GetOrAddComponent<T>();
    //
    //     _popupStack.Push(popup); //stack에 push 
    //
    //
    //
    //     GameObject root = GameObject.Find("@UI_Root");
    //     if (root == null)
    //     {
    //         root = new GameObject { name = "@UI_Root" };
    //     }
    //     go.transform.SetParent(root.transform);
    //     
    //     return popup;
    // }
    //
    // public void ClosePopupUI()
    // {
    //     if (_popupStack.Count == 0) return;
    //     UI_Popup popup = _popupStack.Pop();
    //     Managers.Resource.Destroy(popup.gameObject);
    //
    //     popup = null;
    //     order--;
    // }
    //
    // public void ClosePopupUI(UI_Popup popup)
    // {
    //     if (_popupStack.Count == 0) return;
    //     if (_popupStack.Peek() != popup)
    //     {
    //         Debug.Log("Close Popup Failed!");
    //         return;
    //     }
    //
    //     ClosePopupUI();
    // }
    //
    //
    // public void CloseAllPopupUI()
    // {
    //     while (_popupStack.Count > 0)
    //     {
    //         ClosePopupUI();
    //     }
    // }
    #endregion



    #region UI_WorldSpace

    public T MakeWorldSpaceUI<T>(Transform parent = null, string name = null) where T : UI_Base
    {
        if (string.IsNullOrEmpty(name))
            name = typeof(T).Name;

        GameObject go = Managers.Resource.Instantiate($"UI/WorldSpace/{name}");
        if (parent != null)
            go.transform.SetParent(parent);

        Canvas canvas = go.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.worldCamera = Camera.main;

        return Util.GetOrAddComponent<T>(go);
    }


    #endregion




}

