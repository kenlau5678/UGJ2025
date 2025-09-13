using UnityEngine;

public class GameGrid : MonoBehaviour
{
    public Vector2Int gridPos;
    private SpriteRenderer rend;
    private Color originalColor;
    public Color hoverColor = Color.green;
    public Color moveRangeColor = new Color(1f, 0.5f, 0f); // 橙色
    public bool isInRange=false;

    public SpriteRenderer selectGrid;
    public bool isAttackTarget = false;
    public bool isOccupied = false;

    void Start()
    {
        rend = GetComponent<SpriteRenderer>();
        originalColor = rend.color;
        selectGrid.enabled = false;
    }

    void OnMouseEnter()
    {
        //rend.color = hoverColor;
        selectGrid.enabled = true;
        IsoGrid2D.instance.currentSelectedGrid = this;
        //HandManager.instance.SetCurrentHoverGrid(this);
    }

    void OnMouseExit()
    {
        //rend.color = originalColor;
        selectGrid.enabled = false;
        IsoGrid2D.instance.currentSelectedGrid = null;
        //HandManager.instance.ClearCurrentHoverGrid(this);
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

        UnitController player = FindObjectOfType<UnitController>();

        if (isInRange) // 移动
        {
            player.MoveToGrid(this);
        }
        else if (isAttackTarget) // 攻击
        {
            player.Attack(this);
            IsoGrid2D.instance.ClearHighlight();
        }
    }

}
