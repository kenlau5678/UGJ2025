using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyUnit : MonoBehaviour
{
    public Vector2Int startPoint;
    public GameObject startGrid;
    public int moveRange = 3;
    public float moveSpeed = 2f;
    public float detectRange = 3f;  // ���˷�����ҵķ�Χ������������

    public UnitController player;  // ���
    public HealthSystem healthSystem;
    public float maxHealth;
    public float currentHealth;

    public float attackDamage=2f;
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

        currentHealth = maxHealth;
        healthSystem.SetMaxHealth(maxHealth);
    }

    private void Update()
    {
        // ֻ���ڵ��˻غϲ�ִ��׷���߼�
        if (TurnManager.instance == null || TurnManager.instance.phase != TurnPhase.EnemyTurn)
            return;
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
                Debug.Log(path.Count);
                Debug.Log(moveRange);
                int steps = Mathf.Min(moveRange, path.Count-1); // -1 �����ߵ�������ڸ��ӣ����Ը�����������
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
        // --- ���ͷ���ʼ���� ---
        if (startGrid != null)
            startGrid.GetComponent<GameGrid>().isOccupied = false;

        foreach (var grid in path)
        {
            Vector3 targetPos = grid.transform.position;

            while ((transform.position - targetPos).sqrMagnitude > 0.01f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
                yield return null;
            }

            transform.position = targetPos;

            // ---- ���¸���ռ�� ----
            if (startGrid != null)
                startGrid.GetComponent<GameGrid>().isOccupied = false; // ��վɵ�

            grid.isOccupied = true;  // ռ���µ�
            startGrid = grid.gameObject;  // ���µ�ǰ����

            // ��������
            string[] nameParts = grid.name.Split('_'); // ���Ƹ�ʽ Tile_x_y
            int x = int.Parse(nameParts[1]);
            int y = int.Parse(nameParts[2]);
            startPoint = new Vector2Int(x, y);

            // ���¸��ӹ�ϵ
            transform.SetParent(grid.transform);
            transform.localPosition = Vector3.zero;
        }

        // �ж�������Ƿ�����
        Vector2Int playerPos = player.currentGridPos;
        int dist = Mathf.Abs(playerPos.x - startPoint.x) + Mathf.Abs(playerPos.y - startPoint.y);
        if (dist == 1) // ������������
        {
            AttackPlayer();
        }
    }

    private void AttackPlayer()
    {
        if (player == null) return;

        // ��Ѫ
        player.TakeDamage(attackDamage);
        Debug.Log("Enemy attacks player!");
    }
    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        healthSystem.SetHealth(currentHealth);
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Debug.Log($"{name} is dead!");

            Die();

        }
    }
    private void Die()
    {
        // ������Բ����������� / ������Ʒ / ֪ͨϵͳ
        // Ŀǰ��ֱ�����ٶ���
        Destroy(gameObject);
        if (startGrid != null)
            startGrid.GetComponent<GameGrid>().isOccupied = false;
    }
}
