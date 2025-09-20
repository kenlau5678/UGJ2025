using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class CardForShow : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("卡牌数据")]
    public CardData cardData; // 这个卡牌对应的CardData

    private Vector3 originalScale;
    public Vector3 hoverScale = new Vector3(1.2f, 1.2f, 1f);
    private bool isClicked = false;

    [Header("动画设置")]
    public float hoverDuration = 0.2f;
    public float clickDuration = 0.15f;

    private void Start()
    {
        originalScale = transform.localScale;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isClicked)
        {
            transform.DOScale(hoverScale, hoverDuration).SetEase(Ease.OutBack);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isClicked)
        {
            transform.DOScale(originalScale, hoverDuration).SetEase(Ease.OutBack);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // 点击动画：先放大 1.1 倍，然后回到 hoverScale
        transform.DOScale(hoverScale * 1.1f, clickDuration)
                 .SetEase(Ease.OutBack)
                 .OnComplete(() => transform.DOScale(hoverScale, hoverDuration).SetEase(Ease.OutBack));

        // 添加到初始牌组
        if (cardData != null && DeckManager.instance != null)
        {
            DeckManager.instance.initialDeck.Add(cardData);
            Debug.Log($"卡牌 {cardData.cardName} 已加入初始牌组");
            SceneManager.LoadScene("Map");
        }
    }


}
