using System.Collections.Generic;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    public static DeckManager instance;

    [Header("��ʼ���� (�� Inspector ����)")]
    public List<CardData> initialDeck; // ��ʼ����

    public List<CardData> deck;       // ��ǰ����
    private List<CardData> discardPile; // ���ƶ�

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        // ��ʼ����������ƶ�
        deck = new List<CardData>(initialDeck);
        discardPile = new List<CardData>();
    }

    /// <summary>
    /// ��һ���ƣ����������˿���ϴ��
    /// </summary>
    public CardData DrawCard()
    {
        if (deck.Count == 0)
        {
            if (discardPile.Count > 0)
            {
                // �����ƶ�ϴ��ȥ
                deck.AddRange(discardPile);
                discardPile.Clear();
                Shuffle(deck);
            }
            else
            {
                Debug.Log("��������ƶѶ����ˣ����ܳ���");
                return null;
            }
        }

        CardData drawnCard = deck[0];
        deck.RemoveAt(0);
        return drawnCard;
    }

    /// <summary>
    /// ���Ƽ������ƶ�
    /// </summary>
    public void Discard(CardData card)
    {
        discardPile.Add(card);
    }

    /// <summary>
    /// ����¿�������
    /// </summary>
    public void AddCard(CardData card)
    {
        deck.Add(card);
    }

    /// <summary>
    /// �ӿ����Ƴ���
    /// </summary>
    public void RemoveCard(CardData card)
    {
        if (deck.Contains(card))
            deck.Remove(card);
    }

    /// <summary>
    /// ϴ��
    /// </summary>
    public void Shuffle(List<CardData> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int rand = Random.Range(i, list.Count);
            (list[i], list[rand]) = (list[rand], list[i]);
        }
    }
}
