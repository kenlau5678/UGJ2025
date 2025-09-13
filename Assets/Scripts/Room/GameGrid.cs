using UnityEngine;

public class GameGrid : MonoBehaviour
{
    public Vector2Int gridPos;
    private SpriteRenderer rend;
    private Color originalColor;
    public Color hoverColor = Color.green;
    public Color moveRangeColor = new Color(1f, 0.5f, 0f); // ��ɫ
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

    // �ⲿ���øı������ɫ
    public void SetColor(Color color)
    {
        rend.color = color;
    }

    // �ָ�ԭʼ��ɫ
    public void ResetColor()
    {
        rend.color = originalColor;
    }

    void OnMouseDown()
    {

        UnitController player = FindObjectOfType<UnitController>();

        if (isInRange) // �ƶ�
        {
            player.MoveToGrid(this);
        }
        else if (isAttackTarget) // ����
        {
            player.Attack(this);
            IsoGrid2D.instance.ClearHighlight();
        }
    }

}
