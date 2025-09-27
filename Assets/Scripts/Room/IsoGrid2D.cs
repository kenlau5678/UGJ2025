using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TestTools;

public class IsoGrid2D : MonoBehaviour
{
    public static IsoGrid2D instance;
    public int width = 10;          // 地图宽
    public int height = 10;         // 地图高
    public float cellSize = 1f;     // 格子大小
    public GameObject tilePrefab;   // 格子预制体（一个 Sprite，菱形格子）
    public GameGrid currentSelectedGrid = null;
    public GameObject controller;
    // 用一维列表存格子，Inspector 可见
    public List<GameObject> grid = new List<GameObject>();
    public GameGrid currentPlayerGrid = null;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            GenerateGrid();  // 提前生成
            
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        
    }

    public GridNode[,] nodes;

    void GenerateGrid()
    {
        grid.Clear();
        nodes = new GridNode[width, height];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Vector3 worldPos = GridToWorld(x, y, cellSize);
                GameObject tile = Instantiate(tilePrefab, transform);
                tile.transform.localPosition = worldPos;
                tile.name = $"Tile_{x}_{y}";
                tile.GetComponent<GameGrid>().gridPos = new Vector2Int(x, y);
                grid.Add(tile);

                nodes[x, y] = new GridNode(tile.GetComponent<GameGrid>(), new Vector2Int(x, y));
            }
        }
    }


    // 逻辑坐标 (x,y) -> 世界坐标
    public Vector3 GridToWorld(int x, int y, float cellSize)
    {
        float worldX = (x - y) * cellSize * 1f;
        float worldY = (x + y) * cellSize * 0.5f;
        return new Vector3(worldX, worldY, 0);
    }

    // 根据 (x,y) 拿到格子 GameObject
    public GameObject GetTile(int x, int y)
    {
        if (x < 0 || x >= width || y < 0 || y >= height) return null;
        int index = y * width + x;
        return grid[index];
    }



    public void HighlightMoveRange(Vector2Int playerPos, int moveRange)
    {
        ClearHighlight();

        Queue<(Vector2Int pos, int step)> queue = new Queue<(Vector2Int, int)>();
        HashSet<Vector2Int> visited = new HashSet<Vector2Int>();

        queue.Enqueue((playerPos, 0));
        visited.Add(playerPos);

        while (queue.Count > 0)
        {
            var (pos, step) = queue.Dequeue();
            GameGrid gridComp = GetTile(pos.x, pos.y).GetComponent<GameGrid>();

            // 标记为可移动
            if (step > 0) // step=0 是玩家自己所在格子，可以选择不高亮
            {
                gridComp.SetColor(gridComp.moveRangeColor);
                gridComp.isInRange = true;
            }

            if (step >= moveRange) continue;

            // 四个方向
            Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
            foreach (var dir in directions)
            {
                Vector2Int newPos = pos + dir;
                if (newPos.x < 0 || newPos.x >= width || newPos.y < 0 || newPos.y >= height) continue;
                if (visited.Contains(newPos)) continue;

                GameGrid neighbor = GetTile(newPos.x, newPos.y).GetComponent<GameGrid>();
                if (neighbor.isOccupied) continue; // 被占用格子不能移动

                queue.Enqueue((newPos, step + 1));
                visited.Add(newPos);
            }
        }
    }




    public void ClearHighlight()
    {
        foreach (var tile in grid)
        {
            GameGrid gridComp = tile.GetComponent<GameGrid>();
            gridComp.ResetColor();       // 恢复颜色
            gridComp.isInRange = false;
            gridComp.isAttackTarget = false;
        }
    }

    public List<GameGrid> FindPath(Vector2Int start, Vector2Int target)
    {
        GridNode startNode = nodes[start.x, start.y];
        GridNode targetNode = nodes[target.x, target.y];

        List<GridNode> openList = new List<GridNode>();
        HashSet<GridNode> closedList = new HashSet<GridNode>();

        openList.Add(startNode);

        while (openList.Count > 0)
        {
            GridNode currentNode = openList[0];
            for (int i = 1; i < openList.Count; i++)
            {
                if (openList[i].fCost < currentNode.fCost ||
                    (openList[i].fCost == currentNode.fCost && openList[i].hCost < currentNode.hCost))
                {
                    currentNode = openList[i];
                }
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            if (currentNode == targetNode)
            {
                return RetracePath(startNode, targetNode);
            }

            foreach (GridNode neighbor in GetNeighbors(currentNode))
            {
                // 如果该格子被占用，不能走
                if (neighbor.grid.isOccupied && neighbor != targetNode)
                    continue;

                if (!neighbor.walkable || closedList.Contains(neighbor))
                    continue;

                int newGCost = currentNode.gCost + GetDistance(currentNode, neighbor);
                if (newGCost < neighbor.gCost || !openList.Contains(neighbor))
                {
                    neighbor.gCost = newGCost;
                    neighbor.hCost = GetDistance(neighbor, targetNode);
                    neighbor.parent = currentNode;

                    if (!openList.Contains(neighbor))
                        openList.Add(neighbor);
                }
            }

        }

        return null; // 没有路径
    }

    List<GameGrid> RetracePath(GridNode startNode, GridNode endNode)
    {
        List<GameGrid> path = new List<GameGrid>();
        GridNode currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode.grid);
            currentNode = currentNode.parent;
        }

        path.Reverse();
        return path;
    }

    List<GridNode> GetNeighbors(GridNode node)
    {
        List<GridNode> neighbors = new List<GridNode>();
        Vector2Int[] directions = {
        new Vector2Int(0,1), new Vector2Int(1,0),
        new Vector2Int(0,-1), new Vector2Int(-1,0)
    };

        foreach (var dir in directions)
        {
            int nx = node.position.x + dir.x;
            int ny = node.position.y + dir.y;
            if (nx >= 0 && nx < width && ny >= 0 && ny < height)
                neighbors.Add(nodes[nx, ny]);
        }
        return neighbors;
    }

    int GetDistance(GridNode a, GridNode b)
    {
        return Mathf.Abs(a.position.x - b.position.x) + Mathf.Abs(a.position.y - b.position.y);
    }

    public bool HighlightAttackRange(Vector2Int playerPos, int attackRange)
    {
        ClearHighlight();

        bool foundTarget = false;

        Vector2Int[] directions = {
        Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right
    };

        foreach (var dir in directions)
        {
            Vector2Int targetPos = playerPos + dir * attackRange;
            GameObject tile = GetTile(targetPos.x, targetPos.y);
            if (tile != null)
            {
                EnemyUnit enemy = tile.GetComponentInChildren<EnemyUnit>();
                if (enemy != null) // 如果该格子有敌人
                {
                    GameGrid gridComp = tile.GetComponent<GameGrid>();
                    gridComp.SetColor(Color.red); // 高亮为红色
                    gridComp.isAttackTarget = true;
                    foundTarget = true;
                }
            }
        }

        return foundTarget;
    }
    public void HighlightSingleTile(Vector2Int pos)
    {
        ClearHighlight(); // 先清空已有高亮（可选，看你需不需要多格同时亮）

        GameObject tile = GetTile(pos.x, pos.y);
        if (tile != null)
        {
            GameGrid gridComp = tile.GetComponent<GameGrid>();
            gridComp.SetColor(new Color(1f, 0.5f, 0.5f, 1f));
            gridComp.isInRange = true;  // 你可以用这个布尔来标记它被高亮了
        }
    }

    public bool HighlightRangedAttack(Vector2Int playerPos, int attackRange)
    {
        ClearHighlight();

        bool foundTarget = false;

        for (int dx = -attackRange; dx <= attackRange; dx++)
        {
            for (int dy = -attackRange; dy <= attackRange; dy++)
            {
                int distance = Mathf.Abs(dx) + Mathf.Abs(dy); // 曼哈顿距离
                if (distance == 0 || distance > attackRange) continue;

                Vector2Int targetPos = playerPos + new Vector2Int(dx, dy);
                GameObject tile = GetTile(targetPos.x, targetPos.y);
                if (tile != null)
                {
                    EnemyUnit enemy = tile.GetComponentInChildren<EnemyUnit>();
                    GameGrid gridComp = tile.GetComponent<GameGrid>();

                    if (enemy != null)
                    {
                        // 找到敌人 → alpha 1
                        gridComp.SetColor(new Color(1f, 0.5f, 0.5f, 1f));
                        gridComp.isAttackTarget = true;
                        foundTarget = true;
                    }
                    else
                    {
                        // 范围格子 → 半透明
                        gridComp.SetColor(new Color(1f, 0.5f, 0.5f, 0.3f));
                    }
                }
            }
        }

        return foundTarget;
    }

    public void HighlightRangedAttackRangeHover(Vector2Int playerPos, int attackRange)
    {
        ClearHighlight();
        HighlightSingleTile(playerPos);
        for (int dx = -attackRange; dx <= attackRange; dx++)
        {
            for (int dy = -attackRange; dy <= attackRange; dy++)
            {
                Vector2Int targetPos = new Vector2Int(playerPos.x + dx, playerPos.y + dy);
                int distance = Mathf.Abs(dx) + Mathf.Abs(dy); // 曼哈顿距离（菱形范围）
                if (distance == 0 || distance > attackRange) continue;

                GameObject tile = GetTile(targetPos.x, targetPos.y);
                if (tile != null)
                {
                    GameGrid gridComp = tile.GetComponent<GameGrid>();
                    EnemyUnit enemy = tile.GetComponentInChildren<EnemyUnit>();

                    if (enemy != null)
                    {
                        // 敌人在格子上 → alpha 1
                        Color c = new Color(1f, 0.5f, 0.5f, 1f);
                        gridComp.SetColor(c);
                        gridComp.isAttackTarget = true;
                    }
                    else
                    {
                        // 空格子 → 半透明
                        Color c = new Color(1f, 0.5f, 0.5f, 0.3f);
                        gridComp.SetColor(c);
                    }
                }
            }
        }
    }


    public List<GameGrid> GetRangedAttackTiles(Vector2Int playerPos, int attackRange)
    {
        List<GameGrid> tilesInRange = new List<GameGrid>();

        for (int dx = -attackRange; dx <= attackRange; dx++)
        {
            for (int dy = -attackRange; dy <= attackRange; dy++)
            {
                Vector2Int targetPos = new Vector2Int(playerPos.x + dx, playerPos.y + dy);
                int distance = Mathf.Abs(dx) + Mathf.Abs(dy);
                if (distance == 0 || distance > attackRange) continue;

                GameObject tile = GetTile(targetPos.x, targetPos.y);
                if (tile != null)
                {
                    tilesInRange.Add(tile.GetComponent<GameGrid>());
                }
            }
        }

        return tilesInRange;
    }
    /// <summary>
    /// 高亮玩家攻击范围（敌人与空格子）
    /// </summary>
    /// <param name="playerPos">玩家格子坐标</param>
    /// <param name="attackRange">攻击范围</param>
    /// <returns>范围内是否有敌人</returns>
    public bool HighlightAttackArea(Vector2Int playerPos, int attackRange)
    {
        ClearHighlight();
        HighlightSingleTile(playerPos);
        bool hasEnemy = false;

        for (int dx = -attackRange; dx <= attackRange; dx++)
        {
            for (int dy = -attackRange; dy <= attackRange; dy++)
            {
                int distance = Mathf.Abs(dx) + Mathf.Abs(dy); // 曼哈顿距离
                if (distance == 0 || distance > attackRange) continue;

                Vector2Int targetPos = playerPos + new Vector2Int(dx, dy);
                GameObject tile = GetTile(targetPos.x, targetPos.y);

                if (tile != null)
                {
                    GameGrid gridComp = tile.GetComponent<GameGrid>();
                    EnemyUnit enemy = tile.GetComponentInChildren<EnemyUnit>();

                    if (enemy != null)
                    {
                        // 敌人 → 高亮不透明红色
                        gridComp.SetColor(new Color(1f, 0.5f, 0.5f, 1f));
                        gridComp.isAttackTarget = true;
                        hasEnemy = true;
                    }
                    else
                    {
                        // 空格子 → 半透明红色
                        gridComp.SetColor(new Color(1f, 0.5f, 0.5f, 0.3f));
                    }
                }
            }
        }

        

        return hasEnemy;
    }

    /// <summary>
    /// 获取范围内可攻击的格子列表
    /// </summary>
    public List<GameGrid> GetAttackableTiles(Vector2Int playerPos, int attackRange)
    {
        List<GameGrid> tiles = new List<GameGrid>();

        for (int dx = -attackRange; dx <= attackRange; dx++)
        {
            for (int dy = -attackRange; dy <= attackRange; dy++)
            {
                int distance = Mathf.Abs(dx) + Mathf.Abs(dy);
                if (distance == 0 || distance > attackRange) continue;

                Vector2Int targetPos = playerPos + new Vector2Int(dx, dy);
                GameObject tile = GetTile(targetPos.x, targetPos.y);
                if (tile != null)
                    tiles.Add(tile.GetComponent<GameGrid>());
            }
        }

        return tiles;
    }


}
