using UnityEngine;
using TMPro;

public class PlayerDataManager : MonoBehaviour
{
    // ��ǳ ������ ����
    [SerializeField]
    public GameObject[] jangPoongPrefabs;
    [SerializeField]
    public GameObject jangPoongPrefab;
    [SerializeField]
    public float jangPoongSpeed = 10.0f;
    [SerializeField]
    public float jangPoongDistance = 5.0f;
    [SerializeField]
    public float jangPoongLevel = 1.0f;
    [SerializeField]
    public float jangPoongDamage = 0.5f;
    [SerializeField]
    public float levelUpToken = 0;
    private float[] LevelArr = { 0.5f, 0.7f, 1.1f, 1.6f, 2.2f, 2.9f, 3.5f, 4.2f, 5.0f };

    // ���� ������ ����
    [SerializeField]
    private TextMeshProUGUI manaText;
    [SerializeField]
    public float mana = 100f;
    [SerializeField]
    public float maxMana = 100f;
    [SerializeField]
    public float manaRegenerationRate = 3f;
    [SerializeField]
    public float manaConsumption = 5f;

    // ü�� ������ ����
    [SerializeField]
    private TextMeshProUGUI hpText;
    [SerializeField]
    public float hp = 10.0f;
    [SerializeField]
    public float maxHp = 10.0f;

    private void Awake()
    {
        InvokeRepeating("RegenerateMana", 1f, 1f);  // 1�ʸ��� RegenerateMana �޼��� ȣ��
    }

    private void Update()
    {
        UpdateManaText();
        UpdateHpText();

        // ������ �׽�Ʈ��
        if (Input.GetKeyDown(KeyCode.L))
        {
            LevelUp();
        }
    }

    // ���� ���
    private void RegenerateMana()
    {
        mana = Mathf.Min(mana + manaRegenerationRate, maxMana);
    }

    // ���� �ؽ�Ʈ ������Ʈ
    private void UpdateManaText()
    {
        manaText.text = $"Mana {mana}/{maxMana}";
    }

    // ������
    public void LevelUp()
    {
        levelUpToken += 1;
        jangPoongLevel = Mathf.Clamp(1 + levelUpToken, 1, jangPoongPrefabs.Length);
        jangPoongDamage = LevelArr[(int)jangPoongLevel - 1];
        UpdateJangPoongPrefab();
    }

    // ��ǳ ������ ������Ʈ
    private void UpdateJangPoongPrefab()
    {
        jangPoongPrefab = jangPoongPrefabs[(int)jangPoongLevel - 1];
    }

    // ü�� �ؽ�Ʈ ������Ʈ
    private void UpdateHpText()
    {
        hpText.text = $"Hp {hp}/{maxHp}";
    }
}
