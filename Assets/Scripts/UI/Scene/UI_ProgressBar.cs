using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;

public class UI_ProgressBar : MonoBehaviour
{
    private GameObject player; //플레이어
    private Transform playerTransform;
    private GameObject start;
    private Transform startTransform;
    private GameObject MapEnd;
    private Transform MapEndTransform;
    private Slider progressBarSlider; 
    private float stageLength;

    void Start(){
        //맵의 시작과 끝 부분의 트랜스폼 컴포넌트 가져오기
        start = GameObject.Find("Start");
        MapEnd = GameObject.Find("MapEnd");
        startTransform = start.GetComponent<Transform>();
        MapEndTransform = MapEnd.GetComponent<Transform>();

        //슬라이더 컴포넌트 가져오기
        progressBarSlider = GetComponent<Slider>();
        
        //플레이어 트랜스폼 컴포넌트 가져오기
        player = GameObject.FindWithTag("Player");
        playerTransform = player.GetComponent<Transform>();

        // 스테이지의 가로 길이 계산 (x 축 기준)
        stageLength = MapEndTransform.position.x - startTransform.position.x;
        Debug.Log("스테이지 길이:" + stageLength);
    }

    void Update()
    {
        // 플레이어의 진행도 계산
        float playerPosition = playerTransform.position.x;
        float progress = (playerPosition - startTransform.position.x) / stageLength;

        // 진행바 업데이트
        UpdateProgressBar(progress);
    }

    void UpdateProgressBar(float progress)
    {
        // 진행률을 슬라이더에 반영
        progressBarSlider.value = Mathf.Clamp01(progress);
    }
}
