using System.Collections;
using UnityEngine;


public enum TurnPhase { PlayerTurn, EnemyTurn }


public class TurnManager : MonoBehaviour
{
    public static TurnManager instance;


    public TurnPhase phase = TurnPhase.PlayerTurn;
    public int turnIndex = 1;
    public EnemyUnit enemyUnit;

    [SerializeField] private HorizontalCardHolder playerCardHolder; // 玩家手牌管理器

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }
    public void StartPlayerTurn()
    {
        // --- 回合开始时抽一张卡 ---
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
                    Debug.Log("玩家回合开始");
                    StartPlayerTurn();
                    // 等待玩家结束回合按钮
                    yield return new WaitUntil(() => phase != TurnPhase.PlayerTurn);
                    break;


                case TurnPhase.EnemyTurn:
                    Debug.Log("敌人回合开始");
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
        // TODO: 在这里写敌方AI逻辑
        enemyUnit.ChasePlayer(enemyUnit.player.startPoint);
        yield return new WaitForSeconds(1f);
        Debug.Log("敌人回合结束");
    }
}