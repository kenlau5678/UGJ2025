using DG.Tweening;   // ���������� DOTween �����ռ�
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
public class GameGrid : MonoBehaviour
{
    public Vector2Int gridPos;
    private SpriteRenderer rend;
    private Color originalColor;
    public Color hoverColor = Color.green;
    public Color moveRangeColor = new Color(1f, 0.5f, 0f,0.5f); // ��ɫ
    public bool isInRange = false;

    public SpriteRenderer selectGrid;
    public bool isAttackTarget = false;
    public bool isOccupied = false;

    public UnitController occupiedPlayer;
    public EnemyUnit currentEnemy;
    public bool isInterable = false; // �Ƿ�ɽ���

    public Color interactColor = Color.blue; // �ɽ���������ɫ
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
        if (EventSystem.current.IsPointerOverGameObject()) return;
        UnitController player = FindObjectOfType<UnitController>();

        if (isInRange) // �ƶ�
        {
            UnitController playerToAction = IsoGrid2D.instance.controller.GetComponent<UnitController>();
            playerToAction.MoveToGrid(this);
        }
        else if (isAttackTarget) // ����
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
            yield return new WaitForSeconds(0.2f); // �ȴ� 0.4 ���ټ���
        }
    }

}
