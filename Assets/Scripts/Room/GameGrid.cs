using DG.Tweening;   // 别忘了引入 DOTween 命名空间
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
public class GameGrid : MonoBehaviour
{
    public Vector2Int gridPos;
    private SpriteRenderer rend;
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
    void Start()
    {
        rend = GetComponent<SpriteRenderer>();
        originalColor = rend.color;
        selectGrid.enabled = false;
    }

    void OnMouseEnter()
    {
        selectGrid.enabled = true;
        IsoGrid2D.instance.currentSelectedGrid = this;

        // 如果有玩家站在格子上，做一个放大缩小动画
        if (occupiedPlayer != null)
        {
            Transform playerTransform = occupiedPlayer.transform;

            // 先杀掉可能还在跑的 scale 动画，避免叠加
            playerTransform.DOKill();

            // 保存原始缩放
            Vector3 originalScale = playerTransform.localScale;

            // 放大到1.2倍，再回到原始大小
            playerTransform.DOScale(originalScale * 1.1f, 0.1f)
                .SetLoops(2, LoopType.Yoyo) // 往返两次
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
        UnitController player = FindObjectOfType<UnitController>();

        if (isInRange) // 移动
        {
            UnitController playerToAction = IsoGrid2D.instance.controller.GetComponent<UnitController>();
            playerToAction.MoveToGrid(this);
        }
        else if (isAttackTarget) // 攻击
        {
            
            if(IsoGrid2D.instance.controller.GetComponent<UnitController>().isNextAttackDizziness == true)
            {
                this.currentEnemy.Dizziness();
                IsoGrid2D.instance.controller.GetComponent<UnitController>().Attack(this);
                IsoGrid2D.instance.controller.GetComponent<UnitController>().isNextAttackDizziness = false;
            }
            if (IsoGrid2D.instance.controller.GetComponent<UnitController>().isNextAttackMultiple == true)
            {
                StartCoroutine(AttackMultiple());
                IsoGrid2D.instance.controller.GetComponent<UnitController>().isNextAttackMultiple = false;
            }
            else
            {
                IsoGrid2D.instance.controller.GetComponent<UnitController>().Attack(this);
            }
            IsoGrid2D.instance.ClearHighlight();
        }
        else
        {
            if(occupiedPlayer != null)
            {
                
                TurnManager.instance.ChangePlayer(occupiedPlayer);
                
            }
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
