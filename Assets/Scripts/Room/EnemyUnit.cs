using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyUnit : MonoBehaviour
{
    public Vector2Int startPoint;
    public GameObject startGrid;
    public int moveRange = 3;
    public float moveSpeed = 2f;
    public float detectRange = 5f;  // ���˷�����ҵķ�Χ������������

    public UnitController player;  // ���

    private void Start()
    {
        // ��ʼ�����˵�λ��
        if (IsoGrid2D.instance.GetTile(startPoint.x, startPoint.y) != null)
        {
            startGrid = IsoGrid2D.instance.GetTile(startPoint.x, startPoint.y);
            transform.SetParent(startGrid.transform);
            transform.localPosition = Vector3.zero;
        }

        // �ҵ����
        player = FindObjectOfType<UnitController>();
    }

    private void Update()
    {
        // ֻ���ڵ��˻غϲ�ִ��׷���߼�
        if (TurnManager.instance == null || TurnManager.instance.phase != TurnPhase.EnemyTurn)
            return;

        if (player != null)
        {
            // ��ȡ������ڸ���
            Vector2Int playerPos = player.startPoint;

            // ���˺���ҵ������پ���
            int dist = Mathf.Abs(playerPos.x - startPoint.x) + Mathf.Abs(playerPos.y - startPoint.y);

            if (dist <= detectRange)
            {
                // ׷�����
                ChasePlayer(playerPos);
            }
        }
    }


    /// <summary>
    /// ����׷�����
    /// </summary>
    /// <summary>
    /// ����׷�����
    /// </summary>
    public void ChasePlayer(Vector2Int playerPos)
    {
        GameObject playerGrid = IsoGrid2D.instance.GetTile(playerPos.x, playerPos.y);
        if (playerGrid != null)
        {
            List<GameGrid> path = IsoGrid2D.instance.FindPath(startPoint, playerPos);
            if (path != null && path.Count > 0)
            {
                // ���·�����ȳ������ƶ���Χ��ֻ�� moveRange ��
                int steps = Mathf.Min(moveRange, path.Count - 1); // -1 �����ߵ�������ڸ��ӣ����Ը�����������
                List<GameGrid> limitedPath = path.GetRange(0, steps);

                StopAllCoroutines();
                StartCoroutine(FollowPath(limitedPath));
            }
        }
    }


    public void Move()
    {
        IsoGrid2D.instance.HighlightMoveRange(startPoint, moveRange);
    }

    public void MoveToGrid(GameGrid targetGrid)
    {
        string[] nameParts = targetGrid.gameObject.name.Split('_');
        Vector2Int targetPos = new Vector2Int(int.Parse(nameParts[1]), int.Parse(nameParts[2]));

        List<GameGrid> path = IsoGrid2D.instance.FindPath(startPoint, targetPos);
        if (path != null)
        {
            StopAllCoroutines();
            StartCoroutine(FollowPath(path));
            IsoGrid2D.instance.ClearHighlight();
        }
    }

    private IEnumerator FollowPath(List<GameGrid> path)
    {
        foreach (var grid in path)
        {
            Vector3 targetPos = grid.transform.position;

            while ((transform.position - targetPos).sqrMagnitude > 0.01f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
                yield return null;
            }

            transform.position = targetPos;

            // ���� startPoint
            string[] nameParts = grid.name.Split('_'); // ���Ƹ�ʽ Tile_x_y
            int x = int.Parse(nameParts[1]);
            int y = int.Parse(nameParts[2]);
            startPoint = new Vector2Int(x, y);

            //IsoGrid2D.instance.currentPlayerGrid = grid.GetComponent<GameGrid>();
        }
    }
}
