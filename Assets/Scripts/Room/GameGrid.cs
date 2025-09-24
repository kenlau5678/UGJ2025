using UnityEngine;
using DG.Tweening;   // ���������� DOTween �����ռ�

public class GameGrid : MonoBehaviour
{
    public Vector2Int gridPos;
    private SpriteRenderer rend;
    private Color originalColor;
    public Color hoverColor = Color.green;
    public Color moveRangeColor = new Color(1f, 0.5f, 0f); // ��ɫ
    public bool isInRange = false;

    public SpriteRenderer selectGrid;
    public bool isAttackTarget = false;
    public bool isOccupied = false;

    public UnitController occupiedPlayer;
    public EnemyUnit currentEnemy;

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

        // ��������վ�ڸ����ϣ���һ���Ŵ���С����
        if (occupiedPlayer != null)
        {
            Transform playerTransform = occupiedPlayer.transform;

            // ��ɱ�����ܻ����ܵ� scale �������������
            playerTransform.DOKill();

            // ����ԭʼ����
            Vector3 originalScale = playerTransform.localScale;

            // �Ŵ�1.2�����ٻص�ԭʼ��С
            playerTransform.DOScale(originalScale * 1.1f, 0.1f)
                .SetLoops(2, LoopType.Yoyo) // ��������
                .SetEase(Ease.OutQuad);
        }
    }

    void OnMouseExit()
    {
        selectGrid.enabled = false;
        IsoGrid2D.instance.currentSelectedGrid = null;
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
            IsoGrid2D.instance.controller.GetComponent<UnitController>().MoveToGrid(this);
        }
        else if (isAttackTarget) // ����
        {
            occupiedPlayer.Attack(this);
            IsoGrid2D.instance.ClearHighlight();
        }
        else
        {
            if(occupiedPlayer != null)
            {
                IsoGrid2D.instance.controller = occupiedPlayer.gameObject;
                IsoGrid2D.instance.currentPlayerGrid = this;
                occupiedPlayer.Move();
            }
        }
    }
}
