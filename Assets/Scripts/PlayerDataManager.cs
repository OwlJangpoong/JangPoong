using UnityEngine;
using TMPro;

public class PlayerDataManager : MonoBehaviour
{
    // ��ǳ ������ ����
    [SerializeField]
    public GameObject jangPoongPrefab;
    [SerializeField]
    public float jangPoongSpeed = 10.0f;
    [SerializeField]
    public float jangPoongDistance = 5.0f;
    [SerializeField]
    public float jangPoongLevel = 1.0f;

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
    public float hp = 100f;
    [SerializeField]
    public float maxHp = 100f;

    private void Awake()
    {
        InvokeRepeating("RegenerateMana", 1f, 1f);  // 1�ʸ��� RegenerateMana �޼��� ȣ��
    }

    private void Update()
    {
        UpdateManaText();
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

}
