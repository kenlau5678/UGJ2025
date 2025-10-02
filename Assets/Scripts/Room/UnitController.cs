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
    public float meleeMultiplier = 1f;   

    public float rangedMultiplier = 1f;  

    public float dodgeChance = 0f;       

    public Vector2Int currentGridPos; // ��ҵ�ǰ����λ��

    public int maxActionPoints = 3;   // 每回合初始行动点
    public int actionPoints;          // 当前行动点

    public float shield = 0f;      // ����ֵ

    public bool isMoving = false;

    public SpriteRenderer sr;

    public bool isActive = false;

    public bool isNextAttackDizziness = false;
    public bool isNextAttackMultiple = false;
    public int SegmentCount = 0;

    public bool isNextAttackBloodSucking = false;

    public bool isNextAttackPull = false;
    public int PullDistance = 0;
    private void Start()
    {
        sr = transform.GetChild(0).GetComponent<SpriteRenderer>();
        currentGridPos = startPoint;
        if (IsoGrid2D.instance.GetTile(startPoint.x, startPoint.y) != null)
        {
            startGrid = IsoGrid2D.instance.GetTile(startPoint.x, startPoint.y);
            startGrid.GetComponent<GameGrid>().isOccupied = true;
            startGrid.GetComponent<GameGrid>().occupiedPlayer = this;
            transform.SetParent(startGrid.transform);
            transform.localPosition = Vector3.zero;
            IsoGrid2D.instance.currentPlayerGrid = startGrid.GetComponent<GameGrid>();
        }

        currentHealth = maxHealth;
        healthSystem.SetMaxHealth(maxHealth);
        healthSystem.SetMaxShield(10f);
        healthSystem.SetShield(shield);
        PlayerSwitchManager.instance.currentUnitController = this;
    }

    private void Update()
    {
        if (isActive == false) return;
        if (transform.childCount == 0) return; // 防止没子物件时报错

        
    }


    public void Move()
    {
        if (actionPoints <= 0) return;
        IsoGrid2D.instance.HighlightMoveRange(startPoint, moveRange);
    }

    public void MoveToGrid(GameGrid targetGrid)
    {
        if (actionPoints <= 0) return;
        UseActionPoint(1);
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
        isMoving = true;
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

            if (startGrid != null)
            {
                var oldGrid = startGrid.GetComponent<GameGrid>();
                oldGrid.isOccupied = false;
                oldGrid.occupiedPlayer = null; // 清空旧格子上的玩家
            }

            grid.isOccupied = true;
            grid.occupiedPlayer = this; // 让格子记录当前玩家
            startGrid = grid.gameObject;

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
        isMoving = false;
        Move();
    }

    public void TakeDamage(float amount)
    {
        if (Random.value < dodgeChance)
        {
            Debug.Log($"{name} 闪避了这次攻击！");
            return;
        }
        FindObjectOfType<CameraShake>().Shake();


        if (shield > 0)
        {
            if (shield >= amount)
            {
                shield -= amount;
                amount = 0f;
            }
            else
            {
                amount -= shield;
                shield = 0f;
            }

            // ���»�������ʾ��ǰֵ
            healthSystem.SetShield(shield);
        }

        // ʣ���˺���Ѫ
        if (amount > 0)
        {
            currentHealth -= amount;
            healthSystem.SetHealth(currentHealth);

            if (currentHealth <= 0)
            {
                currentHealth = 0;
                Debug.Log("Player is dead!");
                // TODO: ��Ϸʧ���߼�
            }
        }
    }

    public void AddShield(float amount)
    {
        shield += amount;
        Debug.Log($"��һ�û��� {amount}����ǰ����ֵ: {shield}");
        healthSystem.SetShield(shield);
    }

    public void Heal(float health)
    {
        currentHealth += health;
        if(currentHealth>=maxHealth)
        {
            currentHealth=maxHealth;
        }
        healthSystem.SetHealth(currentHealth);
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

    public void TeleportToGrid(GameGrid targetGrid)
    {
        // �ͷ�ԭ���ĸ���
        if (startGrid != null)
        {
            startGrid.GetComponent<GameGrid>().isOccupied = false;
        }

        // ռ���µĸ���
        targetGrid.isOccupied = true;
        startGrid = targetGrid.gameObject;

        // ��������
        string[] nameParts = targetGrid.name.Split('_');
        int x = int.Parse(nameParts[1]);
        int y = int.Parse(nameParts[2]);
        startPoint = new Vector2Int(x, y);
        currentGridPos = startPoint;

        IsoGrid2D.instance.currentPlayerGrid = targetGrid;

        // ���ø��ӹ�ϵ��˲�Ƶ���������
        transform.SetParent(targetGrid.transform);
        transform.localPosition = Vector3.zero;
    }

    public void UseActionPoint(int usePoint)
    {
        if (TurnManager.instance.currentController == this)
        {
            actionPoints-=usePoint;
            TurnManager.instance.actionPointText.text = "[" + gameObject.name + "]" + "Action Point: " + actionPoints;
        }
        TurnManager.instance.actionPointText.text = "Action Point: " + TurnManager.instance.currentController.actionPoints;
        Debug.Log($"剩余行动点：{TurnManager.instance.currentController.actionPoints}");
        if (actionPoints <= 0)
        {
            // 半透明
            Color c = sr.color;
            c.a = 0.75f;
            sr.color = c;
        }
    }

    public void RecoverActionPoint()
    {
        sr = transform.GetChild(0).GetComponent<SpriteRenderer>();
        actionPoints = maxActionPoints;
        TurnManager.instance.actionPointText.text = "Action Point: " + TurnManager.instance.currentController.actionPoints;
        Color c = sr.color;
        c.a = 1f;
        sr.color = c;
    }

    public void SetActionPoint(int actionPoint)
    {
        actionPoints = actionPoint;
        TurnManager.instance.actionPointText.text = "Action Point: " + TurnManager.instance.currentController.actionPoints;
        if (actionPoints <= 0)
        {
            // 半透明
            Color c = sr.color;
            c.a = 0.75f;
            sr.color = c;
        }
    }
    
    public void SetNextAttackBloodSuck()
    {
        isNextAttackBloodSucking = true;
        sr.color = Color.cyan;
    }
    public void RecoverState()
    {
        isNextAttackBloodSucking = false;
        sr.color = Color.white;
    }
}
