using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class Util
{
    public static T GetOrAddComponent<T>(GameObject go) where T : UnityEngine.Component
    {
        T component = go.GetComponent<T>();
        if (component == null)
            component = go.AddComponent<T>();
        return component;
    }



    //자식 오브젝트 중 GameObject를 검색하는 메소드
    public static GameObject FindChild(GameObject go, string name = null, bool recursive = false)
    {
        Transform transform = FindChild<Transform>(go, name, recursive);
        if (transform != null) return transform.gameObject;
        return null;
    }

    /// <summary>
    /// go의 자식 중 T와 일치하는 타입인 자식을 반환한다. 단, T는 UnityEngine Object만 가능. 이름이 없다, 이름은 비교하지 않고 일치하는 타입의 child를 반환한다.
    /// </summary>
    /// <param name="go">부모 오브젝트</param>
    /// <param name="name">찾고자 하는 자식 오브젝트 이름</param>
    /// <param name="recursive">재귀 검색 여부</param>
    /// <typeparam name="T">찾고자 하는 타입</typeparam>
    /// <returns></returns>
    public static T FindChild<T>(GameObject go, string name = null, bool recursive = false) where T : UnityEngine.Object
    {
        if (go == null) return null;
        if (recursive == false)
        {
            for (int i = 0; i < go.transform.childCount; i++)
            {
                Transform transform = go.transform.GetChild(i);
                //name 체크
                if (string.IsNullOrEmpty(name) || transform.name == name)
                {
                    //component 체크
                    T component = transform.GetComponent<T>();
                    if (component != null) return component;
                }
            }

        }
        else
        {
            foreach (T component in go.GetComponentsInChildren<T>())
            {
                if (string.IsNullOrEmpty(name) || component.name == name)
                    return component;
            }
        }
        return null;

    }
    
    
    //파일 삭제
    public static bool DeleteAllFilesInFolder(string folderPath = null)
    {
        
        //폴더 자체를 삭제하는 방법
        if (!string.IsNullOrEmpty(folderPath) && Directory.Exists(folderPath))
        {
            // 해당 폴더 삭제 (true를 사용하면 내부 파일/서브폴더까지 삭제됨)
            Directory.Delete(folderPath, true);
        
            Debug.Log($"폴더 및 모든 파일 삭제됨: {folderPath}");
            return true;
        }
        else
        {
            Debug.Log($"지정된 폴더가 존재하지 않습니다: {folderPath}");
            return false;
        }

        
        ////폴더는 살리고 내부 파일만 삭제하는 방법
        // if (!string.IsNullOrEmpty(folderPath) && Directory.Exists(folderPath))
        // {
        //     // 해당 폴더 내 모든 파일 가져오기
        //     string[] files = Directory.GetFiles(folderPath);
        //
        //     foreach (string file in files)
        //     {
        //         File.Delete(file); // 파일 삭제
        //         Debug.Log($"삭제됨: {file}");
        //     }
        //
        //     return true;
        // }
        // else
        // {
        //     Debug.Log($"지정된 폴더가 존재하지 않습니다: {folderPath}");
        //
        //     return false;
        // }
    }

}
