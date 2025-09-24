using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine; 



public class PlayerSwitchManager : MonoBehaviour
{
    public static PlayerSwitchManager instance;
    public UnitController[] unitControllers;
    public Button[] switchButtons;
    public UnitController currentUnitController;
    public int currentIndex=0;
    public Vector2Int currentUnitGrid;
    public EnemyUnit[] enemyUnits;
    public bool isChoosing = false;
    public CinemachineVirtualCamera virtualCamera;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

    }

    private void Start()
    {
        enemyUnits = FindObjectsOfType<EnemyUnit>();
        foreach(Button switchbutton in switchButtons)
        {
            switchbutton.interactable = false;
        }
    }
    public void ChangePlayer(int index)
    {
        if (index < 0 || index >= unitControllers.Length) return;

        if (currentUnitController != null)
        {
            currentUnitGrid = currentUnitController.currentGridPos;
            currentUnitController.gameObject.SetActive(false);
        }

        currentUnitController = unitControllers[index];
        IsoGrid2D.instance.controller = currentUnitController.gameObject;
        currentIndex = index;
        currentUnitController.startPoint = currentUnitGrid;

        currentUnitController.TeleportToGrid(
            IsoGrid2D.instance.GetTile(currentUnitGrid.x, currentUnitGrid.y).GetComponent<GameGrid>()
        );

        currentUnitController.gameObject.SetActive(true);

        //ÇÐ»»Ïà»ú¸úËæ
        if (virtualCamera != null)
        {
            virtualCamera.Follow = currentUnitController.transform;
        }

        isChoosing = false;
        foreach (EnemyUnit enemy in enemyUnits)
        {
            //enemy.player = currentUnitController;
        }
        foreach (Button switchbutton in switchButtons)
        {
            switchbutton.interactable = false;
        }
        Debug.Log("Switched to: " + currentUnitController.name);
    }


    public void StartChooseSwitch()
    {
        isChoosing = true;
        foreach (Button switchbutton in switchButtons)
        {
            switchbutton.interactable = true;
        }
        switchButtons[currentIndex].interactable = false;
    }

}
