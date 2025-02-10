using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using TMPro;

public static class FadeEffect
{

    public static IEnumerator Fade(Tilemap target, float start, float end, float fadeTime = 1, float delay = 0, UnityAction action = null)
    {

        yield return new WaitForSeconds(delay);

        //페이드 효과를 재생할 대상(target)이 없으면 코루틴 메소드 종료
        if (target == null) yield break;

        float percent = 0;

        while (percent < 1)
        {
            //페이드 효과 재생

            percent += Time.deltaTime / fadeTime;
            Color color = target.color; //현재 색상 정보 저장
            color.a = Mathf.Lerp(start, end, percent);//색상의 alpha 값 변경
            target.color = color;//변경된 색상 반영

            yield return null;
        }

        //페이드 효과 재생이 완료되면 action 메소드가 등록되어 있는지를 확인한다.
        //등록되어 있다면 해당 메소드를 실행한다.
        action?.Invoke();

    }
    public static IEnumerator Fade(SpriteRenderer target, float start, float end, float fadeTime = 1, float delay = 0, UnityAction action = null)
    {
        yield return new WaitForSeconds(delay);

        //페이드 효과를 재생할 대상(target)이 없으면 코루틴 메소드 종료
        if (target == null) yield break;

        float percent = 0;

        while (percent < 1)
        {
            //페이드 효과 재생

            percent += Time.deltaTime / fadeTime;
            Color color = target.color; //현재 색상 정보 저장
            color.a = Mathf.Lerp(start, end, percent);//색상의 alpha 값 변경
            target.color = color;//변경된 색상 반영

            yield return null;
        }

        //페이드 효과 재생이 완료되면 action 메소드가 등록되어 있는지를 확인한다.
        //등록되어 있다면 해당 메소드를 실행한다.
        action?.Invoke();

    }
    public static IEnumerator Fade(TextMeshProUGUI target, float start, float end, float fadeTime = 1, float delay = 0, UnityAction action = null)
    {
        yield return new WaitForSeconds(delay);
        //페이드 효과를 재생할 대상(target)이 없으면 코루틴 메소드 종료
        if (target == null) yield break;

        float percent = 0;

        while (percent < 1)
        {
            //페이드 효과 재생

            percent += Time.deltaTime / fadeTime;
            Color color = target.color; //현재 색상 정보 저장
            color.a = Mathf.Lerp(start, end, percent);//색상의 alpha 값 변경
            target.color = color;//변경된 색상 반영

            yield return null;
        }

        //페이드 효과 재생이 완료되면 action 메소드가 등록되어 있는지를 확인한다.
        //등록되어 있다면 해당 메소드를 실행한다.
        action?.Invoke();

    }
    public static IEnumerator Fade(Image target, float start, float end, float fadeTime = 1, float delay = 0f, UnityAction action = null)
    {
        Debug.Log("fade 코루틴 시작");
        
        
        // 지정된 딜레이 후 실행
        if (delay > 0)
            yield return new WaitForSeconds(delay);

        yield return new WaitForEndOfFrame(); // 다음 프레임에서 즉시 페이드 시작

        
        //페이드 효과를 재생할 대상(target)이 없으면 코루틴 메소드 종료
        if (target == null) yield break;

        float percent = 0;
        float elapsedTime = 0f;

        while (percent < 1)
        {
            //페이드 효과 재생
            
            // percent += Time.deltaTime / fadeTime;
            elapsedTime += Time.unscaledDeltaTime; // 프레임 속도 영향 제거
            percent = Mathf.Clamp01(elapsedTime / fadeTime);
            
            
            Color color = target.color; //현재 색상 정보 저장
            color.a = Mathf.Lerp(start, end, percent);//색상의 alpha 값 변경
            target.color = color;//변경된 색상 반영

            yield return null;
        }

        //페이드 효과 재생이 완료되면 action 메소드가 등록되어 있는지를 확인한다.
        //등록되어 있다면 해당 메소드를 실행한다.
        action?.Invoke();

    }
}
