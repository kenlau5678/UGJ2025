using System.Collections;
using UnityEngine;


public enum TurnPhase { PlayerTurn, EnemyTurn }


public class TurnManager : MonoBehaviour
{
    public static TurnManager instance;


    public TurnPhase phase = TurnPhase.PlayerTurn;
    public int turnIndex = 1;
    public EnemyUnit enemyUnit;

    [SerializeField] private HorizontalCardHolder playerCardHolder; // ������ƹ�����

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }
    public void StartPlayerTurn()
    {
        // --- �غϿ�ʼʱ��һ�ſ� ---
        if (playerCardHolder != null)
            playerCardHolder.StartCoroutine(playerCardHolder.DrawNewCard());

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
    }


    private IEnumerator EnemyTurn()
    {
        // TODO: ������д�з�AI�߼�
        enemyUnit.ChasePlayer(enemyUnit.player.startPoint);
        yield return new WaitForSeconds(1f);
        Debug.Log("���˻غϽ���");
    }
}