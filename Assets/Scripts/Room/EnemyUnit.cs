using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class EnemyUnit : MonoBehaviour
{
    public Vector2Int startPoint;
    public GameObject startGrid;
    public int moveRange = 3;
    public float moveSpeed = 2f;

    public UnitController targetPlayer;  // 追击的目标玩家
    public HealthSystem healthSystem;
    public float maxHealth;
    public float currentHealth;

    public float attackDamage = 2f;
    public bool isDizziness;

    public SpriteRenderer sr;
    private void Start()
    {
        sr = transform.GetChild(0).GetComponent<SpriteRenderer>();
        // 初始化敌人的位置
        if (IsoGrid2D.instance.GetTile(startPoint.x, startPoint.y) != null)
        {
            startGrid = IsoGrid2D.instance.GetTile(startPoint.x, startPoint.y);
            startGrid.GetComponent<GameGrid>().isOccupied = true;
            startGrid.GetComponent<GameGrid>().currentEnemy = this;
            transform.SetParent(startGrid.transform);
            transform.localPosition = Vector3.zero;
        }

        currentHealth = maxHealth;
        healthSystem.SetMaxHealth(maxHealth);
    }

    private void Update()
    {
        // 只有在敌人回合才执行追击逻辑
        if (TurnManager.instance == null || TurnManager.instance.phase != TurnPhase.EnemyTurn)
            return;
    }

    /// <summary>
    /// 选择最近的玩家作为目标
    /// </summary>
    private void ChooseNearestPlayer()
    {
        UnitController[] players = FindObjectsOfType<UnitController>();
        if (players == null || players.Length == 0) return;

        UnitController nearest = null;
        int shortestPath = int.MaxValue;

        foreach (var p in players)
        {
            Debug.Log(p);
            List<GameGrid> path = IsoGrid2D.instance.FindPath(startPoint, p.currentGridPos);
            if (path != null && path.Count < shortestPath)
            {
                shortestPath = path.Count;
                nearest = p;
                Debug.Log(nearest);
            }
        }

        targetPlayer = nearest;
    }

    /// <summary>
    /// 敌人追踪玩家
    /// </summary>
    public void ChasePlayer()
    {
        ChooseNearestPlayer();
        if (targetPlayer == null) return;

        Vector2Int playerPos = targetPlayer.currentGridPos;
        GameObject playerGrid = IsoGrid2D.instance.GetTile(playerPos.x, playerPos.y);
        if (playerGrid != null)
        {
            List<GameGrid> path = IsoGrid2D.instance.FindPath(startPoint, playerPos);

            if (path == null || path.Count == 0)
                return;

            // --- 如果路径只有1（说明敌人就在原地） ---
            if (path.Count == 1)
            {
                int dist = Mathf.Abs(playerPos.x - startPoint.x) + Mathf.Abs(playerPos.y - startPoint.y);
                if (dist == 1) // 玩家相邻
                {
                    AttackPlayer();
                }
                return;
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
        foreach (var grid in path)
        {
            // ---- 把目标格子先标记为占用，防止冲突 ----
            grid.isOccupied = true;
            grid.currentEnemy = this;

            Vector3 targetPos = grid.transform.position;

            while ((transform.position - targetPos).sqrMagnitude > 0.01f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
                yield return null;
            }

            transform.position = targetPos;

            // ---- 释放旧的格子 ----
            if (startGrid != null)
            {
                GameGrid oldGrid = startGrid.GetComponent<GameGrid>();
                oldGrid.isOccupied = false;
                oldGrid.currentEnemy = null;
            }

            // ---- 占用新的格子 ----
            startGrid = grid.gameObject;

            string[] nameParts = grid.name.Split('_');
            int x = int.Parse(nameParts[1]);
            int y = int.Parse(nameParts[2]);
            startPoint = new Vector2Int(x, y);

            transform.SetParent(grid.transform);
            transform.localPosition = Vector3.zero;
        }

        // 攻击判定
        if (targetPlayer != null)
        {
            Vector2Int playerPos = targetPlayer.currentGridPos;
            int dist = Mathf.Abs(playerPos.x - startPoint.x) + Mathf.Abs(playerPos.y - startPoint.y);
            if (dist == 1)
            {
                AttackPlayer();
            }
        }
    }



    private void AttackPlayer()
    {
        if (targetPlayer == null || currentHealth <= 0) return;

        targetPlayer.TakeDamage(attackDamage);
        Debug.Log("Enemy attacks player!");
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        FindObjectOfType<CameraShake>().Shake();
        healthSystem.SetHealth(currentHealth);

        if(IsoGrid2D.instance.controller.GetComponent<UnitController>().isNextAttackBloodSucking == true)
        {
            IsoGrid2D.instance.controller.GetComponent<UnitController>().Heal(amount);
            IsoGrid2D.instance.controller.GetComponent<UnitController>().RecoverState();
        }

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Debug.Log($"{name} is dead!");

            Die();
        }
    }

    private void Die()
    {
        if (startGrid != null)
        {
            GameGrid grid = startGrid.GetComponent<GameGrid>();
            grid.isOccupied = false;
            grid.currentEnemy = null;
        }
        Destroy(gameObject);
    }

    public void Dizziness()
    {
        isDizziness = true;
        Color c = Color.blue;
        sr.color = c;
    }

    public void Recover()
    {
        isDizziness = false;
        Color c = Color.white;
        sr.color = c;
    }
}
