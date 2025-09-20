using UnityEngine;

public class PanelManager : MonoBehaviour
{
    public static PanelManager instance;
    [Header("UI Panels")]
    public GameObject pausePanel;   // �ϵ� Inspector ��
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
        // ���� ESC ������ͣ
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

        Time.timeScale = 0f;  // ֹͣʱ������
        isPaused = true;
        Debug.Log("��Ϸ����ͣ");
    }

    public void Resume()
    {
        if (pausePanel != null)
            pausePanel.SetActive(false);

        Time.timeScale = 1f;  // �ָ�ʱ��
        isPaused = false;
        Debug.Log("��Ϸ�ѻָ�");
    }

    public void QuitGame()
    {
        Debug.Log("�˳���Ϸ");
        Application.Quit();

        // ����ڱ༭������ԣ��˳� Play ģʽ
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
