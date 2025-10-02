
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;

public class Card : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler, IPointerDownHandler
{
    [Header("Data")]
    public CardData data;  // Assigned when drawing the card.

    private Canvas canvas;
    private Image imageComponent;
    [SerializeField] private bool instantiateVisual = true;
    private VisualCardsHandler visualHandler;
    private Vector3 offset;

    [Header("Movement")]
    [SerializeField] private float moveSpeedLimit = 50;

    [Header("Selection")]
    public bool selected;
    public float selectionOffset = 50;
    private float pointerDownTime;
    private float pointerUpTime;

    [Header("Visual")]
    [SerializeField] private GameObject cardVisualPrefab;
    [HideInInspector] public CardVisual cardVisual;

    [Header("States")]
    public bool isHovering;
    public bool isDragging;
    [HideInInspector] public bool wasDragged;

    [Header("Events")]
    [HideInInspector] public UnityEvent<Card> PointerEnterEvent;
    [HideInInspector] public UnityEvent<Card> PointerExitEvent;
    [HideInInspector] public UnityEvent<Card, bool> PointerUpEvent;
    [HideInInspector] public UnityEvent<Card> PointerDownEvent;
    [HideInInspector] public UnityEvent<Card> BeginDragEvent;
    [HideInInspector] public UnityEvent<Card> EndDragEvent;
    [HideInInspector] public UnityEvent<Card, bool> SelectEvent;
    void Start()
    {
        canvas = GetComponentInParent<Canvas>();
        imageComponent = GetComponent<Image>();

        if (data != null && data.cardSprite != null)
        {
            imageComponent.sprite = data.cardSprite;  // Set sprite based on type.
        }

        if (!instantiateVisual) return;

        visualHandler = FindObjectOfType<VisualCardsHandler>();

        // Use custom visual if defined in data, else fallback to default.
        GameObject prefabToUse = (data != null && data.visualPrefab != null) ? data.visualPrefab : cardVisualPrefab;
        cardVisual = Instantiate(prefabToUse, visualHandler ? visualHandler.transform : canvas.transform).GetComponent<CardVisual>();
        cardVisual.Initialize(this);
    }

    void Update()
    {
        ClampPosition();

        if (isDragging)
        {
            Vector2 targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition) - offset;
            Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;
            Vector2 velocity = direction * Mathf.Min(moveSpeedLimit, Vector2.Distance(transform.position, targetPosition) / Time.deltaTime);
            transform.Translate(velocity * Time.deltaTime);
        }
    }

    void ClampPosition()
    {
        Vector2 screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));
        Vector3 clampedPosition = transform.position;
        clampedPosition.x = Mathf.Clamp(clampedPosition.x, -screenBounds.x, screenBounds.x);
        clampedPosition.y = Mathf.Clamp(clampedPosition.y, -screenBounds.y, screenBounds.y);
        transform.position = new Vector3(clampedPosition.x, clampedPosition.y, 0);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        BeginDragEvent.Invoke(this);
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        offset = mousePosition - (Vector2)transform.position;
        isDragging = true;
        canvas.GetComponent<GraphicRaycaster>().enabled = false;
        imageComponent.raycastTarget = false;

        wasDragged = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        EndDragEvent.Invoke(this);
        isDragging = false;
        canvas.GetComponent<GraphicRaycaster>().enabled = true;
        imageComponent.raycastTarget = true;

        StartCoroutine(FrameWait());

        IEnumerator FrameWait()
        {
            yield return new WaitForEndOfFrame();
            wasDragged = false;
        }
        //UseCard();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        PointerEnterEvent.Invoke(this);
        isHovering = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        PointerExitEvent.Invoke(this);
        isHovering = false;
    }


    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        PointerDownEvent.Invoke(this);
        pointerDownTime = Time.time;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        pointerUpTime = Time.time;

        PointerUpEvent.Invoke(this, pointerUpTime - pointerDownTime > .2f);

        if (pointerUpTime - pointerDownTime > .2f)
            return;

        if (wasDragged)
            return;

        selected = !selected;
        SelectEvent.Invoke(this, selected);

        if (selected)
            transform.localPosition += (cardVisual.transform.up * selectionOffset);
        else
            transform.localPosition = Vector3.zero;
    }

    public void Deselect()
    {
        if (selected)
        {
            selected = false;
            if (selected)
                transform.localPosition += (cardVisual.transform.up * 50);
            else
                transform.localPosition = Vector3.zero;
        }
    }


    public int SiblingAmount()
    {
        return transform.parent.CompareTag("Slot") ? transform.parent.parent.childCount - 1 : 0;
    }

    public int ParentIndex()
    {
        return transform.parent.CompareTag("Slot") ? transform.parent.GetSiblingIndex() : 0;
    }

    public float NormalizedPosition()
    {
        return transform.parent.CompareTag("Slot") ? ExtensionMethods.Remap((float)ParentIndex(), 0, (float)(transform.parent.parent.childCount - 1), 0, 1) : 0;
    }

    private void OnDestroy()
    {
        if(cardVisual != null)
        Destroy(cardVisual.gameObject);
    }

    //public abstract void ExecuteEffect();


    //public bool CanEffect()
    //{
    //    if (data == null) return false;
    //    if (PlayerSwitchManager.instance.isChoosing == true) return false;

    //    return true;
    //}

    // �����Ƿ�ɹ�ִ��
    public bool ExecuteEffect()
    {
        if (data == null) return false;
        if(PlayerSwitchManager.instance.isChoosing == true) return false;

        bool effectExecuted = false;
        UnitController playerUnit = IsoGrid2D.instance.controller.GetComponent<UnitController>();

        switch (data.effectType)
        {
            case CardData.CardEffectType.MoveUnit:
                playerUnit.Move();
                effectExecuted = true;
                break;

            case CardData.CardEffectType.Attack:
                {
                    // ʹ�ÿ��ƵĹ�����Χ��������ÿ��ƿ��Ʒ�Χ�����Լ� attackRange �ֶΣ�
                    bool hasTarget = IsoGrid2D.instance.HighlightAttackRange(
                        playerUnit.currentGridPos,
                        1 // ��ս�̶� 1 �񣬻��߿��Ըĳ� data.attackRange
                    );

                    if (hasTarget)
                    {
                        // TODO: �� HighlightAttackRange ���ҵ�Ŀ��󣬶Ե��˵��� TakeDamage(data.amount)
                        playerUnit.attackDamage = data.amount;
                        if (data.attackAttribute == CardData.AttackAttribute.MultipleDamage)
                        {
                            playerUnit.isNextAttackMultiple = true;
                            playerUnit.SegmentCount = data.SegmentCount;
                        }
                        effectExecuted = true;
                        Debug.Log($"��ս��������� {data.amount} ���˺���");
                    }
                }
                break;
            case CardData.CardEffectType.RemoteAttack:
                {

                    bool hasRangedTarget = IsoGrid2D.instance.HighlightAttackArea(playerUnit.currentGridPos, data.attackRange);
                    Debug.Log(hasRangedTarget);

                    if (hasRangedTarget)
                    {
                        // TODO: �� HighlightRangedAttackRange ���ҵ�Ŀ��󣬶Ե��˵��� TakeDamage(data.amount)
                        playerUnit.attackDamage = data.amount;
                        if(data.attackAttribute == CardData.AttackAttribute.Dizziness)
                        {
                            playerUnit.isNextAttackDizziness = true;
                        }
                        
                        effectExecuted = true;
                        Debug.Log($"Զ�̹�����{data.attackAttribute}������� {data.amount} ���˺���");
                    }
                }
                break;

            case CardData.CardEffectType.Heal:
                if(playerUnit.currentHealth < playerUnit.maxHealth)
                {
                    playerUnit.Heal(data.amount); // ʹ�ÿ��ƶ����������ֵ
                    effectExecuted = true;
                    Debug.Log($"���� {data.amount} ������ֵ��");
                }

                break;
            case CardData.CardEffectType.Bloodsucking:
                playerUnit.SetNextAttackBloodSuck();
                effectExecuted = true;
                Debug.Log("BloodSuck");
                break;
            case CardData.CardEffectType.Shield:
                
                playerUnit.AddShield(data.amount); // ʹ�ÿ��ƶ����������ֵ
                effectExecuted = true;
                Debug.Log($"��� {data.amount} �㻤�ܣ�");
                break;
            case CardData.CardEffectType.Switch:
                PlayerSwitchManager.instance.StartChooseSwitch();
                effectExecuted = true;
                Debug.Log($"�л�");
                break;
            
            default:
                Debug.LogWarning("No effect defined for this card type.");
                break;
            
        }

        data.ExecuteCustomEffect?.Invoke();

        return effectExecuted;
    }

}
