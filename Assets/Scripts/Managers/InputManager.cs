using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager
{
    //리스터 패턴
    //Input Manager가 대표로 입력을 체크한 후, 실제로 입력이 있으면 이를 '이벤트'로 전파해주는 형식으로 구현
    public Action KeyAction = null; //일종의 delegate. 키 입력 액션
    public Action<Define.MouseEvent> MouseAction = null; // 마우스 입력 액션

    private bool _pressed = false;
    private float _pressedTime = 0;

    public void OnUpdate()
    {
        //UI 클릭시 실행 x
        if (EventSystem.current.IsPointerOverGameObject()) return;
        if (Input.anyKey && KeyAction != null)
        {
            KeyAction.Invoke(); //이벤트를 구독 신청한 대상자들에게 KeyAction이 있었음을 알림
        }

        if (MouseAction != null)
        {
            if (Input.GetMouseButton(0)) //mouse pointer held down
            {
                if (!_pressed) //마우스 포인터 누르기 시작
                {
                    MouseAction.Invoke(Define.MouseEvent.PointerDown);
                    _pressedTime = Time.time;
                }
                //누르는 중
                MouseAction.Invoke(Define.MouseEvent.Press);
                _pressed = true;
            }
            else //mouse pointer help up
            {
                if (_pressed)
                {
                    //클릭
                    if(Time.time < _pressedTime+0.2f)
                        MouseAction.Invoke(Define.MouseEvent.Click);
                    
                    //포인터 업
                    MouseAction.Invoke(Define.MouseEvent.PointerUp);
                }

                _pressed = false;
                _pressedTime = 0;
            }
        }
      
    }
    public void Clear()
    {
        KeyAction = null;
        MouseAction = null;
    }
}
