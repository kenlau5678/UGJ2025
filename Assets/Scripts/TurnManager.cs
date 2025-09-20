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

    [SerializeField] private HorizontalCardHolder playerCardHolder; // 玩家手牌管理器

    [Header("行动点")]
    public int maxActionPoints = 2;   // 每回合初始行动点
    public int actionPoints;          // 当前行动点
    public TextMeshProUGUI actionPointText;
    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }
    public void StartPlayerTurn()
    {
        // 回合开始时重置行动点
        actionPoints = maxActionPoints;
        actionPointText.text = "Action Point: " + actionPoints;
        // --- 回合开始时抽一张卡 ---
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
        EnemyUnit[] enemies = FindObjectsOfType<EnemyUnit>();
        if (enemies.Length == 0)
        {
            Debug.Log("没有敌人了，返回地图场景");
            CollectionManager.instance.AddCoin(5);
            PanelManager.instance.ShowEndPanel();
        }
    }


    private IEnumerator EnemyTurn()
    {
        // --- 检查场上是否还有敌人 ---
        EnemyUnit[] enemies = FindObjectsOfType<EnemyUnit>();

        // 如果还有敌人，执行敌人AI逻辑
        foreach (var enemy in enemies)
        {
            if (enemy != null)
            {
                enemy.ChasePlayer(enemy.player.currentGridPos);
                yield return new WaitForSeconds(1f); // 等待敌人行动完成
            }
        }

        Debug.Log("敌人回合结束");
    }
}