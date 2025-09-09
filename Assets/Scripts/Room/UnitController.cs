using System.Collections.Generic;
using UnityEngine;

public class UnitController : MonoBehaviour
{
    public Vector2Int startPoint;
    public GameObject startGrid;
    public int moveRange = 3;
    public float moveSpeed = 2f;
    private void Start()
    {
        if (IsoGrid2D.instance.GetTile(startPoint.x, startPoint.y) != null)
        {
            startGrid = IsoGrid2D.instance.GetTile(startPoint.x, startPoint.y);
            transform.SetParent(startGrid.transform);
            transform.localPosition = Vector3.zero;
            IsoGrid2D.instance.currentPlayerGrid = startGrid.GetComponent<GameGrid>();
        }
    }

    private void Update()
    {

    }

    public void Move()
    {
        IsoGrid2D.instance.HighlightMoveRange(startPoint, moveRange);
    }

    public void MoveToGrid(GameGrid targetGrid)
    {
        string[] nameParts = targetGrid.gameObject.name.Split('_');
        Vector2Int targetPos = new Vector2Int(int.Parse(nameParts[1]), int.Parse(nameParts[2]));

        List<GameGrid> path = IsoGrid2D.instance.FindPath(startPoint, targetPos);
        if (path != null)
        {
            StopAllCoroutines();
            StartCoroutine(FollowPath(path));
            IsoGrid2D.instance.ClearHighlight();
        }
    }

    private System.Collections.IEnumerator FollowPath(List<GameGrid> path)
    {
        foreach (var grid in path)
        {
            Vector3 targetPos = grid.transform.position + new Vector3(0, 0, 0);

            while ((transform.position - targetPos).sqrMagnitude > 0.01f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
                yield return null;
            }

            transform.position = targetPos;

            // 更新 startPoint
            string[] nameParts = grid.name.Split('_'); // 名称格式 Tile_x_y
            int x = int.Parse(nameParts[1]);
            int y = int.Parse(nameParts[2]);
            startPoint = new Vector2Int(x, y);

            IsoGrid2D.instance.currentPlayerGrid = grid.GetComponent<GameGrid>();
        }

        // 移动结束后刷新可移动范围
        //IsoGrid2D.instance.HighlightMoveRange(startPoint, moveRange);
    }



}
