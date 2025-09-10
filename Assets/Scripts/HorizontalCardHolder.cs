using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using DG.Tweening;
using System.Linq;
using UnityEngine.UI;

public class HorizontalCardHolder : MonoBehaviour
{
    [SerializeField] private Card selectedCard;
    [SerializeReference] private Card hoveredCard;

    [SerializeField] private GameObject slotPrefab;
    private RectTransform rect;

    [Header("Spawn Settings")]
    [SerializeField] private int cardsToSpawn = 7;
    public List<Card> cards;

    [Header("Card Draw Settings")]
    [SerializeField] private float drawAnimationDuration = 0.5f;

    bool isCrossing = false;
    [SerializeField] private bool tweenCardReturn = true;

    void Start()
    {
        for (int i = 0; i < cardsToSpawn; i++)
        {
            Instantiate(slotPrefab, transform);
        }

        rect = GetComponent<RectTransform>();
        cards = GetComponentsInChildren<Card>().ToList();

        int cardCount = 0;

        foreach (Card card in cards)
        {
            card.PointerEnterEvent.AddListener(CardPointerEnter);
            card.PointerExitEvent.AddListener(CardPointerExit);
            card.BeginDragEvent.AddListener(BeginDrag);
            card.EndDragEvent.AddListener(EndDrag);
            card.name = cardCount.ToString();
            cardCount++;
        }

        StartCoroutine(Frame());

        IEnumerator Frame()
        {
            yield return new WaitForSecondsRealtime(.1f);
            for (int i = 0; i < cards.Count; i++)
            {
                if (cards[i].cardVisual != null)
                    cards[i].cardVisual.UpdateIndex(transform.childCount);
            }
        }
    }

    private void BeginDrag(Card card)
    {
        selectedCard = card;
    }

    void EndDrag(Card card)
    {
        if (selectedCard == null)
            return;

        selectedCard.transform.DOLocalMove(selectedCard.selected ? new Vector3(0, selectedCard.selectionOffset, 0) : Vector3.zero, tweenCardReturn ? .15f : 0).SetEase(Ease.OutBack);

        rect.sizeDelta += Vector2.right;
        rect.sizeDelta -= Vector2.right;
        if(IsoGrid2D.instance.currentSelectedGrid == IsoGrid2D.instance.currentPlayerGrid)
        {
            UseCard(selectedCard);
        }

        selectedCard = null;
        
    }

    void CardPointerEnter(Card card)
    {
        hoveredCard = card;
    }

    void CardPointerExit(Card card)
    {
        hoveredCard = null;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Delete))
        {
            if (hoveredCard != null)
            {
                Destroy(hoveredCard.transform.parent.gameObject);
                cards.Remove(hoveredCard);
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            foreach (Card card in cards)
            {
                card.Deselect();
            }
        }

        // New card draw input
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(DrawNewCard());
        }

        if (selectedCard == null)
            return;

        if (isCrossing)
            return;

        for (int i = 0; i < cards.Count; i++)
        {
            if (selectedCard.transform.position.x > cards[i].transform.position.x)
            {
                if (selectedCard.ParentIndex() < cards[i].ParentIndex())
                {
                    Swap(i);
                    break;
                }
            }

            if (selectedCard.transform.position.x < cards[i].transform.position.x)
            {
                if (selectedCard.ParentIndex() > cards[i].ParentIndex())
                {
                    Swap(i);
                    break;
                }
            }
        }
    }

    void Swap(int index)
    {
        isCrossing = true;

        Transform focusedParent = selectedCard.transform.parent;
        Transform crossedParent = cards[index].transform.parent;

        cards[index].transform.SetParent(focusedParent);
        cards[index].transform.localPosition = cards[index].selected ? new Vector3(0, cards[index].selectionOffset, 0) : Vector3.zero;
        selectedCard.transform.SetParent(crossedParent);

        isCrossing = false;

        if (cards[index].cardVisual == null)
            return;

        bool swapIsRight = cards[index].ParentIndex() > selectedCard.ParentIndex();
        cards[index].cardVisual.Swap(swapIsRight ? -1 : 1);

        //Updated Visual Indexes
        foreach (Card card in cards)
        {
            card.cardVisual.UpdateIndex(transform.childCount);
        }
    }

    public IEnumerator DrawNewCard()
    {
        GameObject newSlot = Instantiate(slotPrefab, transform);
        Card newCard = newSlot.GetComponentInChildren<Card>();

        if (newCard != null)
        {
            // Set up card events
            newCard.PointerEnterEvent.AddListener(CardPointerEnter);
            newCard.PointerExitEvent.AddListener(CardPointerExit);
            newCard.BeginDragEvent.AddListener(BeginDrag);
            newCard.EndDragEvent.AddListener(EndDrag);
            newCard.name = cards.Count.ToString();



            // Animation
            Vector3 startPos = new Vector3(1000f, 0, 0);
            newSlot.transform.localPosition = startPos;
            newSlot.transform.DOLocalMove(Vector3.zero, drawAnimationDuration)
                .SetEase(Ease.OutBack);

            // Update visual indexes
            yield return new WaitForSeconds(drawAnimationDuration);

            // 强制刷新布局
            LayoutRebuilder.ForceRebuildLayoutImmediate(rect);
            foreach (Card card in cards)
            {
                if (card.cardVisual != null)
                    card.cardVisual.UpdateIndex(transform.childCount);
            }

            // Add to cards list
            cards.Add(newCard);
        }
    }
    void UseCard(Card card)
    {
        if (card == null) return;

        // 1. 执行卡牌效果（调用游戏逻辑）
        card.ExecuteEffect(); // 可以在Card里写一个方法描述具体效果

        // 2. 播放使用动画
        //card.cardVisual.transform.DOPunchPosition(Vector3.up * 50, 0.3f, 10);

        // 3. 从手牌中移除
        cards.Remove(card);
        Destroy(card.transform.parent.gameObject); // 或 Destroy(card.gameObject) 视情况

        // 4. 刷新其他卡牌的Visual索引
        foreach (var c in cards)
            c.cardVisual.UpdateIndex(transform.childCount);

        // 5. （可选）刷新布局
        LayoutRebuilder.ForceRebuildLayoutImmediate(rect);
    }

}