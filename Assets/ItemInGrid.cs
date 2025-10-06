using System.Collections.Generic;
using UnityEngine;

public class ItemInGrid : MonoBehaviour
{
    [Tooltip("����ռ������������Խ�����")]
    public Vector2Int cornerA = Vector2Int.zero;
    public Vector2Int cornerB = Vector2Int.zero;

    [Tooltip("�Ƿ��ǵ�������������ѡ��ֻռ��cornerA")]
    public bool isSingleCell = false;

    private List<GameGrid> occupiedGrids = new List<GameGrid>();
    public bool isInterable = false; // �Ƿ�ɽ���
    public SpriteRenderer sr;

    void Start()
    {
        sr = transform.GetComponent<SpriteRenderer>();
        // ռ�ø���
        if (isSingleCell)
        {
            Occupy(cornerA, cornerA); // �������ֻռcornerA
        }
        else
        {
            Occupy(cornerA, cornerB);
        }

        if (sr != null && occupiedGrids.Count > 0)
        {
            float sum = 0f;
            foreach (var grid in occupiedGrids)
            {
                sum += -(grid.gridPos.x + grid.gridPos.y); // ����ԭ���Ĺ���ȡ��
            }

            int average = Mathf.RoundToInt(sum / occupiedGrids.Count);
            sr.sortingOrder = average + 2; // +2 ȷ����ʾ�ڸ����Ϸ�
            transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sortingOrder = average + 3;
        }

        // �ɽ���ֻ��ǣ����ı���ɫ
        if (isInterable)
        {
            foreach (var grid in occupiedGrids)
            {
                grid.isInterable = true;
            }
        }
    }

    public void Occupy(Vector2Int a, Vector2Int b)
    {
        ClearOccupied();

        int minX = Mathf.Min(a.x, b.x);
        int maxX = Mathf.Max(a.x, b.x);
        int minY = Mathf.Min(a.y, b.y);
        int maxY = Mathf.Max(a.y, b.y);

        for (int x = minX; x <= maxX; x++)
        {
            for (int y = minY; y <= maxY; y++)
            {
                GameObject tileObj = IsoGrid2D.instance.GetTile(x, y);
                if (tileObj != null)
                {
                    GameGrid gridComp = tileObj.GetComponent<GameGrid>();
                    gridComp.isOccupied = true;
                    occupiedGrids.Add(gridComp);
                }
                else
                {
                    Debug.LogWarning($"û�ҵ� ({x},{y}) ��Ӧ�ĸ��ӣ����� {gameObject.name}");
                }
            }
        }
    }

    public void ClearOccupied()
    {
        foreach (var grid in occupiedGrids)
        {
            grid.isOccupied = false;
            grid.isInterable = false;  // ����ɽ������
        }
        occupiedGrids.Clear();
    }

    private void OnDestroy()
    {
        ClearOccupied();
    }
}
