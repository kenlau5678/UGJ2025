using DG.Tweening;   // ���������� DOTween �����ռ�
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
    public Color moveRangeColor = new Color(1f, 0.5f, 0f,0.5f); // ��ɫ
    public bool isInRange = false;

    public SpriteRenderer selectGrid;
    public bool isAttackTarget = false;
    public bool isOccupied = false;

    public UnitController occupiedPlayer;
    public EnemyUnit currentEnemy;
    public bool isInterable = false; // �Ƿ�ɽ���

    public Color interactColor = Color.blue; // �ɽ���������ɫ
    public Vector3 playerOriginalScale;

    public int sortingOrder;
    private void Awake()
    {
        rend = GetComponent<SpriteRenderer>();

    }
    void Start()
    {
        //�� (0,0) Ϊ����㣬ԽԶ�ĸ���Խ������
        sortingOrder = gridPos.x + gridPos.y;

        // ���ѡ�п�Ҫ�ڸ����ϲ�
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

            // ֻ�ڵ�һ�μ�¼ԭʼ����
            if (playerOriginalScale == Vector3.zero)
            {
                playerOriginalScale = playerTransform.localScale;
            }

            // ��ֹͣ�ɶ���
            playerTransform.DOKill();

            // ÿ��ִ�ж���ǰ������Ϊԭʼ��С����ֹԽ��Խ��
            playerTransform.localScale = playerOriginalScale;

            // ִ�зŴ��ٻ�ԭ����
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

        if (occupiedPlayer != null) // �л���ɫ
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
            yield return new WaitForSeconds(0.2f); // �ȴ� 0.4 ���ټ���
        }
    }

}
