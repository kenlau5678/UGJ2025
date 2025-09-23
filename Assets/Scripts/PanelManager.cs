using UnityEngine;

public class PanelManager : MonoBehaviour
{
    public static PanelManager instance;
    [Header("UI Panels")]
    public GameObject pausePanel;   // 拖到 Inspector 中
    public GameObject endPanel;
    private bool isPaused = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

    }
    void Start()
    {
        if (pausePanel != null)
            pausePanel.SetActive(false);
    }

    void Update()
    {
        // 按下 ESC 开关暂停
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                Resume();
            else
                Pause();
        }
    }

    public void ShowEndPanel()
    {
        endPanel.gameObject.SetActive(true);
    }

    public void CloseEndPanel()
    {
        endPanel.gameObject.SetActive(false);
    }
    public void Pause()
    {
        if (pausePanel != null)
            pausePanel.SetActive(true);

        Time.timeScale = 0f;  // 停止时间流动
        isPaused = true;
        Debug.Log("游戏已暂停");
    }

    public void Resume()
    {
        if (pausePanel != null)
            pausePanel.SetActive(false);

        Time.timeScale = 1f;  // 恢复时间
        isPaused = false;
        Debug.Log("游戏已恢复");
    }

    public void QuitGame()
    {
        Debug.Log("退出游戏");
        Application.Quit();

        // 如果在编辑器里调试，退出 Play 模式
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
