using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UI_HPBar : UI_Base
{
    [SerializeField] protected GameObject HP_Bar;
    private float offset_y = 0.4f;


    [SerializeField] protected Monster monster;

    protected float max; //slider max
    protected float min; //slider min


    [SerializeField] private float activeTime = 2f;



    public override void Init()
    {
        //bind로 자동화로 수정예정
        HP_Bar = Util.FindChild(gameObject, "HP_Bar");
        //체력 스탯 가져오기
        monster = transform.parent.GetComponent<Monster>();
        max = monster.stat.monsterData.MaxHp;
        min = 0f;
        HP_Bar.GetComponent<Slider>().maxValue = max;
        HP_Bar.GetComponent<Slider>().minValue = min;



    }


    //HP bar 위치 조정
    protected virtual void Update()
    {
        //부모의 위치
        Transform parent = transform.parent;
        //hp bar 위치 = 부모 오브젝트의 머리 위치. MovementRigidbody2D의 headPosition을 이용한다.
        Vector2 headPosition = Util.GetOrAddComponent<Mon_MovementRigidbody2D>(parent.gameObject).HeadPosition;
        transform.position = new Vector3(headPosition.x, headPosition.y + offset_y, parent.position.z);

        float value = monster.stat.CurrentHp;
        SetHpRatio(value);
    }


    public virtual void SetHpRatio(float value)
    {
        HP_Bar.GetComponent<Slider>().value = value;
    }

    public virtual void ShowHP()
    {
        gameObject.SetActive(true);
        StopAllCoroutines();
        StartCoroutine(HideHP());

    }


    public IEnumerator HideHP()
    {
        float time = 0f;
        while (time < activeTime)
        {
            time += Time.deltaTime;
            yield return null;
        }

        gameObject.SetActive(false);



    }

}
