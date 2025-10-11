using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �� IsoGrid2D �ľֲ�����ϵ������һ�������ĸ��Ӹ�������
/// ����Խű������ transform λ��Ϊ׼���߼����걣�� startPos��
/// ���޸� IsoGrid2D �� width / height��ֻ�Ǹ��Ӹ��ӡ�
/// ͬʱ֧������ߵ� triggerPositions ������Ӵ��� parent ���
/// </summary>
public class GridAddition : MonoBehaviour
{
    [Header("�����������")]
    public Vector2Int startPos = new Vector2Int(12, 0);  // �߼��������
    public int areaWidth = 3;                            // ������
    public int areaHeight = 3;                           // ����߶�

    [Header("�������")]
    public List<Vector2Int> triggerPositions = new List<Vector2Int>(); // ��Ҵ��������б�
    public GameObject targetParent;                                       // �����󼤻�ĸ�����
    public bool generateOnStart = false;                                   // �Ƿ��� Start �����ɶ������

    private IsoGrid2D gridSystem;
    public bool triggered = false; // ��ֹ�ظ�����

    void Start()
    {
        gridSystem = IsoGrid2D.instance;
        if (gridSystem == null)
        {
            Debug.LogError("δ�ҵ� IsoGrid2D ʵ������ȷ�������д��� IsoGrid2D��");
            return;
        }

        if (targetParent != null)
            targetParent.SetActive(false); // ��ʼ�ر�

        if (generateOnStart)
            GenerateExtraArea();
    }

    void Update()
    {
        if (triggered) return;

        // �����ҵ�ǰλ��
        if (gridSystem.currentPlayerGrid != null)
        {
            Vector2Int playerPos = gridSystem.currentPlayerGrid.gridPos;
            foreach (var pos in triggerPositions)
            {
                if (pos == playerPos)
                {
                    triggered = true;
                    ActivateTargetAndGenerate();
                    break;
                }
            }
        }
    }

    [ContextMenu("Generate Extra Area")]
    public void GenerateExtraArea()
    {
        if (gridSystem == null)
            gridSystem = IsoGrid2D.instance;

        List<GameObject> newTiles = new List<GameObject>();

        // �� IsoGrid2D �ľֲ��ռ�Ϊ��׼
        Vector3 startLocalPos = gridSystem.transform.InverseTransformPoint(transform.position);

        float cellSize = gridSystem.cellSize;  // �� IsoGrid2D һ��

        for (int y = 0; y < areaHeight; y++)
        {
            for (int x = 0; x < areaWidth; x++)
            {
                Vector2Int logicPos = new Vector2Int(startPos.x + x, startPos.y + y);

                // ʹ����ͬ�� cellSize ����ƫ��
                Vector3 offset = gridSystem.GridToWorld(x, y, cellSize);
                Vector3 localOffset = offset - gridSystem.GridToWorld(0, 0, cellSize);

                GameObject tile = Instantiate(gridSystem.tilePrefab, gridSystem.transform);
                tile.transform.localPosition = startLocalPos + localOffset;
                tile.name = $"ExtraTile_{logicPos.x}_{logicPos.y}";

                GameGrid gridComp = tile.GetComponent<GameGrid>();
                gridComp.gridPos = logicPos;
                gridComp.isOccupied = false;

                GridNode node = new GridNode(gridComp, logicPos);
                IsoGrid2D.instance.extraNodes[logicPos] = node;

                gridSystem.grid.Add(tile);
                newTiles.Add(tile);
            }
        }
        IsoGrid2D.instance.controller.GetComponent<UnitController>().Move();

        Debug.Log($"�����ɸ������� {areaWidth}x{areaHeight}���߼���㣺{startPos}��cellSize={cellSize}���� {newTiles.Count} ��");
    }

    /// <summary>
    /// ������岢���ɶ������
    /// </summary>
    private void ActivateTargetAndGenerate()
    {
        if (targetParent != null)
            targetParent.SetActive(true);


        GenerateExtraArea();


        Debug.Log("������ҵ�����ӣ������弤����ɶ������");
    }
}
