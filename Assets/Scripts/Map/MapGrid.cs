using UnityEngine;
using UnityEngine.SceneManagement;

public class MapGrid : MonoBehaviour
{
    public Vector2Int gridPos;
    private SpriteRenderer rend;
    private bool visited = false;  // 是否已经被访问过
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
    // 玩家走过的格子 → 永远灰色
    public void SetVisited()
    {
        if (!visited)
        {
            visited = true;
            if (rend != null)
                rend.color = Color.gray;
        }
    }

    // 高亮格子 → 只作用于未访问格子
    public void SetHighlight(Color highlightColor)
    {
        if (!visited && rend != null)
            rend.color = highlightColor;

        if (icon != null)
            icon.enabled = true;
    }

    // 判断格子是否灰色（已访问）
    public bool IsGray()
    {
        return visited;
    }

    public void ResetIfFarFromPlayer()
{
    // 如果格子未被访问过或未显示过 icon，保持白色
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
