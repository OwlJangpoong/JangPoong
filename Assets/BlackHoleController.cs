using System.Collections;
using UnityEngine;

public interface IHitFromPlayer
{
    void HitByPlayer(int damage);
}

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Collider2D))]
public class BlackHoleController : MonoBehaviour, IHitFromPlayer
{
    [Header("Tuning")]
    [SerializeField] private float lifeTime = 10f;          // 전체 수명(Spawn~End 포함)
    [SerializeField] private float pullStrength = 14f;      // 끌림 세기 (Force)
    [SerializeField] private float slowFactor = 0.5f;       // 감속 (0.5면 절반 속도 느낌)
    [SerializeField] private float mpDrainPerSec = 10f;     // 초당 MP 흡수
    [SerializeField] private float contactDamagePerSec = 5f;// 접촉 중 초당 HP 피해
    [SerializeField] private float contactTickInterval = 0.25f;

    [Header("Hit Points (파괴 가능)")]
    [SerializeField] private int maxHp = 30;   // 플레이어 투사체로 파괴 가능
    private int currentHp;

    [Header("Refs")]
    [SerializeField] private Animator anim;
    [SerializeField] private SpriteRenderer sr;
    [SerializeField] private Collider2D triggerCol;

    private Transform target;                 // 플레이어 Transform
    private Rigidbody2D targetRb;             // 플레이어 Rigidbody
    private PlayerStatsController targetStats;// 플레이어 스탯

    private bool activeCore = false;          // Spawn 이후 본작동 시작
    private float mpBuffer = 0f;              // MP 틱 버퍼
    private float lifeTimer;

    // 접촉 중 틱 데미지 관리
    private float contactTickTimer = 0f; 
    private bool playerInside = false;

    // 외부에서 소환 시 데이터 주입 (옵션)
    public void Init(Transform player)
    {
        target = player;
        if (target)
        {
            targetRb = target.GetComponent<Rigidbody2D>();
            targetStats = target.GetComponent<PlayerStatsController>();
        }
    }

    private void Reset()
    {
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        triggerCol = GetComponent<Collider2D>();
        if (triggerCol) triggerCol.isTrigger = true;
    }

    private void Awake()
    {
        if (!anim) anim = GetComponent<Animator>();
        if (!sr) sr = GetComponent<SpriteRenderer>();
        if (!triggerCol) triggerCol = GetComponent<Collider2D>();

        currentHp = maxHp;
        lifeTimer = lifeTime;
    }

    private void OnEnable()
    {
        // 시작하면 Spawn 애니가 재생될 것이고, 끝 프레임에서 EnableBlackHoleCore() 호출됨
        StartCoroutine(CoLifeTimer());
    }

    private IEnumerator CoLifeTimer()
    {
        // 전체 수명 타이머
        while (lifeTimer > 0f)
        {
            lifeTimer -= Time.deltaTime;
            yield return null;
        }

        // 수명 종료 → End 애니
        TriggerEnd();
    }

    // 애니메이션 이벤트에서 호출
    public void EnableBlackHoleCore()
    {
        activeCore = true;
    }

    // 애니메이션 이벤트에서 호출 (End 마지막 프레임)
    public void DestroySelf()
    {
        Destroy(gameObject);
    }

    private void Update()
    {
        if (!activeCore || !target) return;

        // 끌어당김 + 감속
        if (targetRb)
        {
            Vector2 dir = (transform.position - target.position);
            targetRb.AddForce(dir.normalized * (pullStrength * Time.deltaTime), ForceMode2D.Force);
            // 느려지도록 약한 감쇠
            targetRb.velocity *= (1f - (1f - slowFactor) * Time.deltaTime);
        }

        // MP 흡수(초당)
        if (targetStats)
        {
            mpBuffer += mpDrainPerSec * Time.deltaTime;
            if (mpBuffer >= 1f)
            {
                int mp = Mathf.FloorToInt(mpBuffer);
                targetStats.LoseMP(mp);
                mpBuffer -= mp;
            }
        }

        // 접촉 중 HP 틱 데미지
        if (playerInside && targetStats)
        {
            contactTickTimer -= Time.deltaTime;
            if (contactTickTimer <= 0f)
            {
                float dmg = contactDamagePerSec * contactTickInterval;
                targetStats.OnAttacked(dmg);
                contactTickTimer = contactTickInterval;
            }
        }
    }

    // 플레이어가 블랙홀 트리거 범위에 들어오면 "접촉 피해" 활성
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!activeCore) return;
        if (other.CompareTag("Player"))
        {
            playerInside = true;
            contactTickTimer = 0f; // 바로 한 틱 들어가게 0으로 초기화
        }
        // 플레이어 투사체에게 피격 (파괴 가능)
        if (other.CompareTag("PlayerProjectile"))
        {
            // 투사체가 데미지를 들고있다면 꺼내서 HitByPlayer에 전달
            int dmg = 1;
            var proj = other.GetComponent<IProjectileDamage>();
            if (proj != null) dmg = proj.Damage;

            HitByPlayer(dmg);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!activeCore) return;
        if (other.CompareTag("Player"))
        {
            playerInside = false;
        }
    }

    public void HitByPlayer(int damage)
    {
        if (damage <= 0) return;
        currentHp -= damage;
        // 간단한 피격 피드백 (알파 펄스 등)
        if (sr)
        {
            var c = sr.color;
            c.a = 0.7f;
            sr.color = c;
            // 천천히 복원
            LeanAlphaBack();
        }

        if (currentHp <= 0)
        {
            TriggerEnd();
        }
    }

    private async void LeanAlphaBack()
    {
        // LeanTween/DoTween 없으면 간단 복원
        // 프레임 두어개에 걸쳐 회복
        for (int i = 0; i < 5; i++)
        {
            if (!sr) break;
            var c = sr.color;
            c.a = Mathf.Min(1f, c.a + 0.06f);
            sr.color = c;
            await System.Threading.Tasks.Task.Yield();
        }
    }

    public void TriggerEnd()
    {
        if (anim && gameObject.activeInHierarchy)
        {
            activeCore = false;
            anim.ResetTrigger("End");
            anim.SetTrigger("End");   // End 애니 → 마지막 프레임에서 DestroySelf()
        }
        else
        {
            DestroySelf();
        }
    }
}

// (선택) 플레이어 투사체가 데미지를 노출하도록
public interface IProjectileDamage
{
    int Damage { get; }
}