using UnityEngine;

[CreateAssetMenu(fileName = "CardData", menuName = "Card/CardData", order = 1)]
public class CardData : ScriptableObject
{
    public string cardName;
    public string description;
    public Sprite cardSprite;        // ����ͼƬ
    public GameObject visualPrefab;  // ��ѡ�Զ���Ԥ����
    public int manaCost;             // ����ħ��ֵ
    
    public CardEffectType effectType;

    // ��Ӧ��ֵ������ effectType ���ò�ͬ
    [Tooltip("������������ֵ������ EffectType ����")]
    public float amount = 0f;

    // Զ�̹���ר�ò���
    [Header("Ranged Attack Settings")]
    public int attackRange = 0;   // Զ�̹�����Χ����������

    public AttackAttribute attackAttribute = AttackAttribute.None;
    public int SegmentCount = 0;

    // Enum for simple effects
    public enum CardEffectType
    {
        MoveUnit,
        Attack,         // ��ս������ʹ�� amount
        Heal,           // ���ƣ�ʹ�� amount
        RemoteAttack,    // Զ�̹�����ʹ�� amount + attackRange
        Shield,
        Switch,
    }

    // �������ԣ�����չ��
    public enum AttackAttribute
    {
        None,
        Fire,
        Ice,
        Poison,
        Lightning,
        Dizziness,
        MultipleDamage
    }

    // Optional: Delegate for custom effects
    public System.Action ExecuteCustomEffect;
}
