using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndPanel : MonoBehaviour
{
    [Header("卡牌池")]
    public GameObject[] cardPool;

    [Header("展示位置（拖3个空物体进去）")]
    public Transform[] CardShowPosition;

    // 当前生成的卡牌列表
    private List<GameObject> currentCards = new List<GameObject>();

    private void Start()
    {
        ShowRandomCards();
    }

    public void ShowRandomCards()
    {
        // 清空之前生成的卡牌
        foreach (var card in currentCards)
        {
            Destroy(card);
        }
        currentCards.Clear();

        // 确保卡牌池和位置有效
        if (cardPool.Length == 0 || CardShowPosition.Length == 0)
        {
            Debug.LogWarning("卡牌池或展示位置未设置！");
            return;
        }

        // 随机生成卡牌到指定位置
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
