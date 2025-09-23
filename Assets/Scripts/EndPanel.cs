using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndPanel : MonoBehaviour
{
    [Header("���Ƴ�")]
    public GameObject[] cardPool;

    [Header("չʾλ�ã���3���������ȥ��")]
    public Transform[] CardShowPosition;

    // ��ǰ���ɵĿ����б�
    private List<GameObject> currentCards = new List<GameObject>();

    private void Start()
    {
        ShowRandomCards();
    }

    public void ShowRandomCards()
    {
        // ���֮ǰ���ɵĿ���
        foreach (var card in currentCards)
        {
            Destroy(card);
        }
        currentCards.Clear();

        // ȷ�����Ƴغ�λ����Ч
        if (cardPool.Length == 0 || CardShowPosition.Length == 0)
        {
            Debug.LogWarning("���Ƴػ�չʾλ��δ���ã�");
            return;
        }

        // ������ɿ��Ƶ�ָ��λ��
        for (int i = 0; i < CardShowPosition.Length; i++)
        {
            int randomIndex = Random.Range(0, cardPool.Length);
            GameObject cardInstance = Instantiate(cardPool[randomIndex]);
            cardInstance.transform.SetParent(CardShowPosition[i]);
            cardInstance.transform.localPosition = Vector3.zero;
            cardInstance.transform.localScale = Vector3.one;
            currentCards.Add(cardInstance);
        }
    }
}
