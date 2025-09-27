using System.Collections.Generic;
using UnityEngine;

public class ItemInGrid : MonoBehaviour
{
    [Tooltip("矩形占用区域的两个对角坐标")]
    public Vector2Int cornerA = Vector2Int.zero;
    public Vector2Int cornerB = Vector2Int.zero;

    [Tooltip("是否是单格物件，如果勾选，只占用cornerA")]
    public bool isSingleCell = false;

    private List<GameGrid> occupiedGrids = new List<GameGrid>();
    public bool isInterable = false; // 是否可交互

    void Start()
    {
        // 占用格子
        if (isSingleCell)
        {
            Occupy(cornerA, cornerA); // 单格物件只占cornerA
        }
        else
        {
            Occupy(cornerA, cornerB);
        }

        // 可交互只标记，不改变颜色
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
                    Debug.LogWarning($"没找到 ({x},{y}) 对应的格子，跳过 {gameObject.name}");
                }
            }
        }
    }

    public void ClearOccupied()
    {
        foreach (var grid in occupiedGrids)
        {
            grid.isOccupied = false;
            grid.isInterable = false;  // 清除可交互标记
        }
        occupiedGrids.Clear();
    }

    private void OnDestroy()
    {
        ClearOccupied();
    }
}
