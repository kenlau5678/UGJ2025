using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class CardForShow : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("��������")]
    public CardData cardData; // ������ƶ�Ӧ��CardData

    private Vector3 originalScale;
    public Vector3 hoverScale = new Vector3(1.2f, 1.2f, 1f);
    private bool isClicked = false;

    [Header("��������")]
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
        // ����������ȷŴ� 1.1 ����Ȼ��ص� hoverScale
        transform.DOScale(hoverScale * 1.1f, clickDuration)
                 .SetEase(Ease.OutBack)
                 .OnComplete(() => transform.DOScale(hoverScale, hoverDuration).SetEase(Ease.OutBack));

        // ��ӵ���ʼ����
        if (cardData != null && DeckManager.instance != null)
        {
            DeckManager.instance.initialDeck.Add(cardData);
            Debug.Log($"���� {cardData.cardName} �Ѽ����ʼ����");
            SceneManager.LoadScene("Map");
        }
    }


}
