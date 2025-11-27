using System.Reflection;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGodMode : MonoBehaviour
{
    [Header("테스트용 HP/MP 설정")]
    public float testMaxHp = 10000f;
    public float testHp     = 10000f;
    public int   testMaxMp  = 10000;
    public int   testMp     = 10000;

    [Header("옵션")]
    public bool lockHP = true;
    public bool lockMP = true;

    [Header("장풍 테스트")]
    public bool overrideJangpoongDamage = true;
    public float overrideDamageValue = 30f;

    // 내부 상태
    private float lastHp = -1f;
    private int   lastMp = -1;

    // 리플렉션 캐시
    private FieldInfo fiJpDamage;
    private HashSet<JangpoongController> patched = new HashSet<JangpoongController>();

    void Start()
    {
        if (Managers.Player != null)
        {
            // HP/MP 세팅
            if (Managers.Player.MaxHp < testMaxHp) Managers.Player.SetMaxHp(testMaxHp);
            Managers.Player.SetHp(testHp);

            if (Managers.Player.MaxMana < testMaxMp) Managers.Player.SetMaxMana(testMaxMp);
            Managers.Player.SetMana(testMp);

            lastHp = Managers.Player.Hp;
            lastMp = Managers.Player.Mana;

            Debug.Log($"[GodMode] HP {Managers.Player.Hp}/{Managers.Player.MaxHp}, MP {Managers.Player.Mana}/{Managers.Player.MaxMana}");
        }
        else
        {
            Debug.LogWarning("[GodMode] Managers.Player 가 null 입니다.");
        }

        // 장풍 private 필드 접근용 리플렉션 준비
        fiJpDamage = typeof(JangpoongController).GetField("jangPoongDamage",
            BindingFlags.Instance | BindingFlags.NonPublic);
    }

    void Update()
    {
        if (Managers.Player == null) return;

        // HP/MP 변화 로깅
        if (!Mathf.Approximately(Managers.Player.Hp, lastHp))
        {
            Debug.Log($"[GodMode] HP 변경: {lastHp} -> {Managers.Player.Hp}");
            lastHp = Managers.Player.Hp;
        }
        if (Managers.Player.Mana != lastMp)
        {
            Debug.Log($"[GodMode] MP 변경: {lastMp} -> {Managers.Player.Mana}");
            lastMp = Managers.Player.Mana;
        }

        // HP/MP 고정 옵션
        if (lockHP && Managers.Player.Hp < testHp) Managers.Player.SetHp(testHp);
        if (lockMP && Managers.Player.Mana < testMp) Managers.Player.SetMana(testMp);

        // 🔥 장풍 데미지 전역 오버라이드 (원본 스크립트 수정 없이)
        if (overrideJangpoongDamage && fiJpDamage != null)
        {
            // 씬에 살아있는 장풍들을 찾아서 한 번만 패치
            var jps = FindObjectsOfType<JangpoongController>();
            foreach (var jp in jps)
            {
                if (jp == null || patched.Contains(jp)) continue;

                fiJpDamage.SetValue(jp, overrideDamageValue);
                patched.Add(jp);
                Debug.Log($"[GodMode] 장풍 데미지 오버라이드 -> {overrideDamageValue} ({jp.name})");
            }

            // 사라진 장풍은 집합에서 정리
            patched.RemoveWhere(x => x == null);
        }
    }
}