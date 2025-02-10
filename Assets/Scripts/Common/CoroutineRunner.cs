using UnityEngine;

public class CoroutineRunner : MonoBehaviour
{
    private static CoroutineRunner _instance;

    public static CoroutineRunner Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject obj = new GameObject("CoroutineRunner");
                _instance = obj.AddComponent<CoroutineRunner>();
                DontDestroyOnLoad(obj); // 씬이 변경되어도 유지되도록 설정
            }
            return _instance;
        }
    }
    
    
    public void StopAllRunningCoroutines()
    {
        StopAllCoroutines(); // ✅ 모든 코루틴 중지
    }
}