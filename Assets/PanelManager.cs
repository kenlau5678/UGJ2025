using UnityEngine;

public class PanelManager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject pausePanel;   // �ϵ� Inspector ��

    private bool isPaused = false;

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
