using DG.Tweening;   // 别忘了引入 DOTween 命名空间
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class GameGrid : MonoBehaviour
{
    public Vector2Int gridPos;
    public SpriteRenderer rend;
    private Color originalColor;
    public Color hoverColor = Color.green;
    public Color moveRangeColor = new Color(1f, 0.5f, 0f,0.5f); // 橙色
    public bool isInRange = false;

    public SpriteRenderer selectGrid;
    public bool isAttackTarget = false;
    public bool isOccupied = false;

    public UnitController occupiedPlayer;
    public EnemyUnit currentEnemy;
    public bool isInterable = false; // 是否可交互

    public Color interactColor = Color.blue; // 可交互格子颜色
    public Vector3 playerOriginalScale;

    public int sortingOrder;
    private void Awake()
    {
        rend = GetComponent<SpriteRenderer>();

    }
    void Start()
    {
        //以 (0,0) 为最近点，越远的格子越“靠后”
        sortingOrder = gridPos.x + gridPos.y;

        // 如果选中框要在格子上层
        if (selectGrid != null)
            selectGrid.sortingOrder = -sortingOrder + 1;
        originalColor = rend.color;
        selectGrid.enabled = false;
    }


    void OnMouseEnter()
    {
        selectGrid.enabled = true;
        IsoGrid2D.instance.currentSelectedGrid = this;

        if (occupiedPlayer != null)
        {
            Transform playerTransform = occupiedPlayer.transform;

            // 只在第一次记录原始缩放
            if (playerOriginalScale == Vector3.zero)
            {
                playerOriginalScale = playerTransform.localScale;
            }

            // 先停止旧动画
            playerTransform.DOKill();

            // 每次执行动画前，重置为原始大小（防止越放越大）
            playerTransform.localScale = playerOriginalScale;

            // 执行放大再还原动画
            playerTransform.DOScale(playerOriginalScale * 1.1f, 0.1f)
                .SetLoops(2, LoopType.Yoyo)
                .SetEase(Ease.OutQuad);
        }
    }

    void OnMouseExit()
    {
        selectGrid.enabled = false;
        IsoGrid2D.instance.currentSelectedGrid = null;
    }

    // 外部调用改变格子颜色
    public void SetColor(Color color)
    {
        rend.color = color;
    }

    // 恢复原始颜色
    public void ResetColor()
    {
        rend.color = originalColor;
    }

    void OnMouseDown()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;

        if (occupiedPlayer != null) // 切换角色
        { 
            TurnManager.instance.ChangePlayer(occupiedPlayer); 
        }
        NormalGridClick();
    }

    void NormalGridClick()
    {
        UnitController playerController = IsoGrid2D.instance.controller.GetComponent<UnitController>();

        if (isInRange)
        {
            playerController.MoveToGrid(this);
            IsoGrid2D.instance.ResetWaiting();
            return;
        }

        if (isAttackTarget)
        {
            if (playerController.isNextAttackDizziness)
            {
                currentEnemy.Dizziness();
                playerController.Attack(this);
                playerController.isNextAttackDizziness = false;
            }
            else if (playerController.isNextAttackMultiple)
            {
                StartCoroutine(AttackMultiple());
                playerController.isNextAttackMultiple = false;
            }
            else if (playerController.isNextAttackPull)
            {
                playerController.Attack(this);
                currentEnemy.BePulled(playerController.currentGridPos, playerController.PullDistance);
                playerController.PullDistance = 0;
                playerController.isNextAttackPull = false;
            }
            else
            {
                playerController.Attack(this);
            }

            FindAnyObjectByType<HorizontalCardHolder>().DrawCardAndUpdate();
            IsoGrid2D.instance.ResetWaiting();
        }
    }

    



    private IEnumerator AttackMultiple()
    {
        var unitController = IsoGrid2D.instance.controller.GetComponent<UnitController>();

        for (int i = 0; i < unitController.SegmentCount; i++)
        {
            unitController.Attack(this);
            yield return new WaitForSeconds(0.2f); // 等待 0.4 秒再继续
        }
    }

}
