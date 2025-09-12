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
    public int attackRange = 1;    // 攻击范围（上下左右 1 格）
    public float attackDamage = 5f;

    public Vector2Int currentGridPos; // 玩家当前格子位置
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
        // 先释放起始格子
        if (startGrid != null)
            startGrid.GetComponent<GameGrid>().isOccupied = false;

        foreach (var grid in path)
        {
            Vector3 targetPos = grid.transform.position;

            // 移动到目标格子
            while ((transform.position - targetPos).sqrMagnitude > 0.01f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
                yield return null;
            }

            transform.position = targetPos;

            // ---- 更新格子占用 ----
            if (startGrid != null)
                startGrid.GetComponent<GameGrid>().isOccupied = false;  // 清空旧的

            grid.isOccupied = true;  // 占用新的
            startGrid = grid.gameObject;  // 更新当前格子

            // 更新坐标
            string[] nameParts = grid.name.Split('_');
            int x = int.Parse(nameParts[1]);
            int y = int.Parse(nameParts[2]);
            startPoint = new Vector2Int(x, y);
            currentGridPos = startPoint;

            IsoGrid2D.instance.currentPlayerGrid = grid.GetComponent<GameGrid>();

            // 更新父子关系
            transform.SetParent(grid.transform);
            transform.localPosition = Vector3.zero; // 保证居中
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
            // TODO: 这里可以触发游戏失败逻辑
        }
    }
    public void Attack(GameGrid targetGrid)
    {
        EnemyUnit enemy = targetGrid.GetComponentInChildren<EnemyUnit>();
        if (enemy != null)
        {
            Debug.Log($"玩家攻击 {enemy.name}，造成 {attackDamage} 伤害！");
            enemy.TakeDamage(attackDamage);
           
        }
    }
}
