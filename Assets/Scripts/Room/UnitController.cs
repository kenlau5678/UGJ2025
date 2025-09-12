using System.Collections.Generic;
using UnityEngine;

public class UnitController : MonoBehaviour
{
    public Vector2Int startPoint;
    public GameObject startGrid;
    public int moveRange = 3;
    public float moveSpeed = 2f;

    public HealthSystem healthSystem;
    public float maxHealth;
    public float currentHealth;

    [Header("Combat")]
    public int attackRange = 1;    // ������Χ���������� 1 ��
    public float attackDamage = 5f;

    public Vector2Int currentGridPos; // ��ҵ�ǰ����λ��
    private void Start()
    {
        if (IsoGrid2D.instance.GetTile(startPoint.x, startPoint.y) != null)
        {
            startGrid = IsoGrid2D.instance.GetTile(startPoint.x, startPoint.y);
            startGrid.GetComponent<GameGrid>().isOccupied = true;
            transform.SetParent(startGrid.transform);
            transform.localPosition = Vector3.zero;
            IsoGrid2D.instance.currentPlayerGrid = startGrid.GetComponent<GameGrid>();
        }

        currentHealth = maxHealth;
        healthSystem.SetMaxHealth(maxHealth);
    }

    private void Update()
    {

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

    private System.Collections.IEnumerator FollowPath(List<GameGrid> path)
    {
        // ���ͷ���ʼ����
        if (startGrid != null)
            startGrid.GetComponent<GameGrid>().isOccupied = false;

        foreach (var grid in path)
        {
            Vector3 targetPos = grid.transform.position;

            // �ƶ���Ŀ�����
            while ((transform.position - targetPos).sqrMagnitude > 0.01f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
                yield return null;
            }

            transform.position = targetPos;

            // ---- ���¸���ռ�� ----
            if (startGrid != null)
                startGrid.GetComponent<GameGrid>().isOccupied = false;  // ��վɵ�

            grid.isOccupied = true;  // ռ���µ�
            startGrid = grid.gameObject;  // ���µ�ǰ����

            // ��������
            string[] nameParts = grid.name.Split('_');
            int x = int.Parse(nameParts[1]);
            int y = int.Parse(nameParts[2]);
            startPoint = new Vector2Int(x, y);
            currentGridPos = startPoint;

            IsoGrid2D.instance.currentPlayerGrid = grid.GetComponent<GameGrid>();

            // ���¸��ӹ�ϵ
            transform.SetParent(grid.transform);
            transform.localPosition = Vector3.zero; // ��֤����
        }
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        healthSystem.SetHealth(currentHealth);
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Debug.Log("Player is dead!");
            // TODO: ������Դ�����Ϸʧ���߼�
        }
    }
    public void Attack(GameGrid targetGrid)
    {
        EnemyUnit enemy = targetGrid.GetComponentInChildren<EnemyUnit>();
        if (enemy != null)
        {
            Debug.Log($"��ҹ��� {enemy.name}����� {attackDamage} �˺���");
            enemy.TakeDamage(attackDamage);
           
        }
    }
}
