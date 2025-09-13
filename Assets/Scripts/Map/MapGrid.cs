using UnityEngine;
using UnityEngine.SceneManagement;

public class MapGrid : MonoBehaviour
{
    public Vector2Int gridPos;
    private SpriteRenderer rend;
    private bool visited = false;  // �Ƿ��Ѿ������ʹ�
    public int type;
    public SpriteRenderer icon;
    public Color gridColor = Color.white;

    void Awake()
    {
        rend = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        if (rend != null)
        {
            if (type == 1)
            {
                rend.color = Color.white;
            }
        }

        if (icon != null)
            if (type == 1) icon.enabled = false;
    }
    // ����߹��ĸ��� �� ��Զ��ɫ
    public void SetVisited()
    {
        if (!visited)
        {
            visited = true;
            if (rend != null)
                rend.color = Color.gray;
        }
    }

    // �������� �� ֻ������δ���ʸ���
    public void SetHighlight(Color highlightColor)
    {
        if (!visited && rend != null)
            rend.color = highlightColor;

        if (icon != null)
            icon.enabled = true;
    }

    // �жϸ����Ƿ��ɫ���ѷ��ʣ�
    public bool IsGray()
    {
        return visited;
    }

    public void ResetIfFarFromPlayer()
{
    // �������δ�����ʹ���δ��ʾ�� icon�����ְ�ɫ
    if (!visited && (icon == null || !icon.enabled))
    {
        if (rend != null)
            rend.color = Color.white;
    }
}
    void OnMouseDown()
    {

    }

}
