using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyUnit : MonoBehaviour
{
    public Vector2Int startPoint;
    public GameObject startGrid;
    public int moveRange = 3;
    public float moveSpeed = 2f;
    public float detectRange = 3f;  // 敌人发现玩家的范围（按格子数）

    public UnitController player;  // 玩家
    public HealthSystem healthSystem;
    public float maxHealth;
    public float currentHealth;

    public float attackDamage=2f;
    private void Start()
    {
        // 初始化敌人的位置
        if (IsoGrid2D.instance.GetTile(startPoint.x, startPoint.y) != null)
        {
            startGrid = IsoGrid2D.instance.GetTile(startPoint.x, startPoint.y);
            startGrid.GetComponent<GameGrid>().isOccupied = true;
            transform.SetParent(startGrid.transform);
            transform.localPosition = Vector3.zero;
        }

        // 找到玩家
        player = FindObjectOfType<UnitController>();

        currentHealth = maxHealth;
        healthSystem.SetMaxHealth(maxHealth);
    }

    private void Update()
    {
        // 只有在敌人回合才执行追击逻辑
        if (TurnManager.instance == null || TurnManager.instance.phase != TurnPhase.EnemyTurn)
            return;
        if (player == null) player = FindObjectOfType<UnitController>();
    }


    /// <summary>
    /// 敌人追踪玩家
    /// </summary>
    /// <summary>
    /// 敌人追踪玩家
    /// </summary>
    public void ChasePlayer(Vector2Int playerPos)
    {
        GameObject playerGrid = IsoGrid2D.instance.GetTile(playerPos.x, playerPos.y);
        if (playerGrid != null)
        {
            List<GameGrid> path = IsoGrid2D.instance.FindPath(startPoint, playerPos);

            if (path == null || path.Count == 0)
                return;

            // --- 如果路径只有1（说明敌人就在原地） ---
            if (path.Count == 1)
            {
                // 判断是否可以攻击玩家
                int dist = Mathf.Abs(playerPos.x - startPoint.x) + Mathf.Abs(playerPos.y - startPoint.y);
                if (dist == 1) // 玩家相邻
                {
                    AttackPlayer();
                }
                return; // 不移动，直接结束
            }

            // --- 正常移动逻辑 ---
            int steps = Mathf.Min(moveRange, path.Count - 1);
            List<GameGrid> limitedPath = path.GetRange(0, steps);

            StopAllCoroutines();
            StartCoroutine(FollowPath(limitedPath));
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
        // --- 先释放起始格子 ---
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

            // ---- 更新格子占用 ----
            if (startGrid != null)
                startGrid.GetComponent<GameGrid>().isOccupied = false; // 清空旧的

            grid.isOccupied = true;  // 占用新的
            startGrid = grid.gameObject;  // 更新当前格子

            // 更新坐标
            string[] nameParts = grid.name.Split('_'); // 名称格式 Tile_x_y
            int x = int.Parse(nameParts[1]);
            int y = int.Parse(nameParts[2]);
            startPoint = new Vector2Int(x, y);

            // 更新父子关系
            transform.SetParent(grid.transform);
            transform.localPosition = Vector3.zero;
        }

        // 判断与玩家是否相邻
        Vector2Int playerPos = player.currentGridPos;
        int dist = Mathf.Abs(playerPos.x - startPoint.x) + Mathf.Abs(playerPos.y - startPoint.y);
        if (dist == 1) // 上下左右相邻
        {
            AttackPlayer();
        }
    }

    private void AttackPlayer()
    {
        if (player == null || currentHealth <= 0) return;

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
        // 这里可以播放死亡动画 / 掉落物品 / 通知系统
        // 目前先直接销毁对象
        Destroy(gameObject);
        if (startGrid != null)
            startGrid.GetComponent<GameGrid>().isOccupied = false;
    }
}
