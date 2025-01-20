using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ResourceManager
{
    //필요한 인터페이스
    //1. load : 리소스 가져오기
    //2. instantiate : 프리팹에서 인스턴스 생성
    //3. destroy


    //Load
    public T Load<T>(string path) where T : Object
    {
        path = Path.Combine(Path.GetDirectoryName(path) ?? string.Empty, Path.GetFileNameWithoutExtension(path)); 
        T resource = Resources.Load<T>(path);
        if (resource == null)
        {
            Debug.Log("Failed to load resource");
            return null;
        }
        return Resources.Load<T>(path);
    }


    /// <summary>
    /// 프리팹을 load하여 인스턴스를 생성한다.
    /// Resource/Prefabs 를 기준으로 한 폴더 경로를 사용한다.("Resources/Prefabs/"는 생략한다)
    /// </summary>
    /// <param name="path">프리팹이 위치한 폴더 경로(Resource/Prefabs 를 기준)</param>
    /// <param name="parent"></param>
    /// <returns></returns>
    public GameObject Instantiate(string path, Transform parent = null, Vector3 position=default)
    {
        //prefab 가져오기
        GameObject prefab = Load<GameObject>($"Prefabs/{path}");
        if (prefab == null)
        {
            //오류
            Debug.Log($"Failed to load prefab : {path}");
            return null;
        }
        return Object.Instantiate(prefab, position, Quaternion.identity, parent); //Object 붙인 이유 : 현재 함수 이름과 Object.Instantiate 이름이 동일하기 때문에 그냥 사용시 현재 함수가 재귀 호출 될 수 있음.

    }

    public void Destroy(GameObject gameObject)
    {
        if (gameObject == null) return;
        Object.Destroy(gameObject);
    }




}
