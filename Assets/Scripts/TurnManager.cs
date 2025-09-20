using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum TurnPhase { PlayerTurn, EnemyTurn }


public class TurnManager : MonoBehaviour
{
    public static TurnManager instance;


    public TurnPhase phase = TurnPhase.PlayerTurn;
    public int turnIndex = 1;
    public EnemyUnit enemyUnit;

    [SerializeField] private HorizontalCardHolder playerCardHolder; // ������ƹ�����

    [Header("�ж���")]
    public int maxActionPoints = 2;   // ÿ�غϳ�ʼ�ж���
    public int actionPoints;          // ��ǰ�ж���
    public TextMeshProUGUI actionPointText;
    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }
    public void StartPlayerTurn()
    {
        // �غϿ�ʼʱ�����ж���
        actionPoints = maxActionPoints;
        actionPointText.text = "Action Point: " + actionPoints;
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
                enemy.ChasePlayer(enemy.player.currentGridPos);
                yield return new WaitForSeconds(1f); // �ȴ������ж����
            }
        }

        Debug.Log("���˻غϽ���");
    }
}