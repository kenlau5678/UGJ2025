using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum TurnPhase { PlayerTurn, EnemyTurn }


public class TurnManager : MonoBehaviour
{
    public static TurnManager instance;

    public UnitController currentController;
    public TurnPhase phase = TurnPhase.PlayerTurn;
    public int turnIndex = 1;
    public EnemyUnit enemyUnit;

    [SerializeField] private HorizontalCardHolder playerCardHolder; // ������ƹ�����


    public int actionPoints;          // ��ǰ�ж���
    public TextMeshProUGUI actionPointText;

    public UnitController[] unitControllers;
    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }


    public void ChangePlayer(UnitController player)
    {
        if (player.isActive == false) return;
        IsoGrid2D.instance.ClearHighlight();
        currentController = player;
        IsoGrid2D.instance.controller = currentController.gameObject;
        IsoGrid2D.instance.currentPlayerGrid = currentController.startGrid.GetComponent<GameGrid>();
        actionPointText.text = "[" + currentController.name + "]" + "Action Point: " + currentController.actionPoints;
        CameraMove.instance.ChangeFollow(player.gameObject);
        currentController.Move();
        PlayerSwitchManager.instance.currentUnitController = currentController;
        var psm = PlayerSwitchManager.instance;
        if (psm != null)
        {
            psm.currentUnitController = player;

            // �ҵ� player ��Ӧ�� slot index
            int slotIndex = psm.allSlots.FindIndex(s => s.unit == player);
            if (slotIndex >= 0)
            {
                psm.currentIndex = slotIndex;
            }
            else
            {
                Debug.LogWarning("���δ�� PlayerSwitchManager �� slots ���ҵ���");
            }
        }
    }
    public void StartPlayerTurn()
    {
        // �غϿ�ʼʱ�����ж���
        foreach (var unitController in unitControllers) 
        {
            unitController.RecoverActionPoint();
        }
        actionPointText.text = "[" + unitControllers[0].name +"]"+"Action Point: " + unitControllers[0].actionPoints;
        ChangePlayer(unitControllers[0]);
        // --- �غϿ�ʼʱ��һ�ſ� ---
        if (playerCardHolder != null)
            playerCardHolder.StartCoroutine(playerCardHolder.DrawNewCard());
        UnitController[] players = FindObjectsOfType<UnitController>();
        foreach(UnitController player in players)
        {
            player.shield = 0;
            player.healthSystem.SetShield(0);
        }
        Debug.Log("Player Turn Started!");
    }

    private void Start()
    {
        unitControllers = FindObjectsOfType<UnitController>();
        StartCoroutine(RunTurnLoop());
    }


    private IEnumerator RunTurnLoop()
    {
        while (true)
        {
            switch (phase)
            {
                case TurnPhase.PlayerTurn:
                    Debug.Log("��һغϿ�ʼ");
                    StartPlayerTurn();
                    // �ȴ���ҽ����غϰ�ť
                    yield return new WaitUntil(() => phase != TurnPhase.PlayerTurn);
                    break;


                case TurnPhase.EnemyTurn:
                    Debug.Log("���˻غϿ�ʼ");
                    yield return StartCoroutine(EnemyTurn());
                    phase = TurnPhase.PlayerTurn;
                    turnIndex++;
                    break;
            }


            yield return null;
        }
    }


    public void EndPlayerTurn()
    {
        phase = TurnPhase.EnemyTurn;
        EnemyUnit[] enemies = FindObjectsOfType<EnemyUnit>();
        if (enemies.Length == 0)
        {
            Debug.Log("û�е����ˣ����ص�ͼ����");
            CollectionManager.instance.AddCoin(5);
            PanelManager.instance.ShowEndPanel();
        }
    }


    private IEnumerator EnemyTurn()
    {
        // --- ��鳡���Ƿ��е��� ---
        EnemyUnit[] enemies = FindObjectsOfType<EnemyUnit>();

        // ������е��ˣ�ִ�е���AI�߼�
        foreach (var enemy in enemies)
        {
            if (enemy != null)
            {
                if(enemy.isDizziness == true)
                {
                    enemy.Recover();
                    continue;
                }
                enemy.ChasePlayer();
                yield return new WaitForSeconds(1.5f); // �ȴ������ж����
            }
        }

        Debug.Log("���˻غϽ���");
    }
}