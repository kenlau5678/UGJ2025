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

    void Start()
    {
        // ռ�ø���
        if (isSingleCell)
        {
            Occupy(cornerA, cornerA); // �������ֻռcornerA
        }
        else
        {
            Occupy(cornerA, cornerB);
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
