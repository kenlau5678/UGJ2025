using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TestTools;

public class IsoGrid2D : MonoBehaviour
{
    public static IsoGrid2D instance;
    public int width = 10;          // ��ͼ��
    public int height = 10;         // ��ͼ��
    public float cellSize = 1f;     // ���Ӵ�С
    public GameObject tilePrefab;   // ����Ԥ���壨һ�� Sprite�����θ��ӣ�
    public GameGrid currentSelectedGrid = null;
    public GameObject controller;
    // ��һά�б����ӣ�Inspector �ɼ�
    public List<GameObject> grid = new List<GameObject>();
    public GameGrid currentPlayerGrid = null;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            GenerateGrid();  // ��ǰ����
            
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


    // �߼����� (x,y) -> ��������
    public Vector3 GridToWorld(int x, int y, float cellSize)
    {
        float worldX = (x - y) * cellSize * 1f;
        float worldY = (x + y) * cellSize * 0.5f;
        return new Vector3(worldX, worldY, 0);
    }

    // ���� (x,y) �õ����� GameObject
    public GameObject GetTile(int x, int y)
    {
        if (x < 0 || x >= width || y < 0 || y >= height) return null;
        int index = y * width + x;
        return grid[index];
    }

    public void HighlightMoveRange(Vector2Int playerPos, int moveRange)
    {
        // �Ȱ����и��ӻָ�ԭɫ������� isInRange
        ClearHighlight();

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int distance = Mathf.Abs(x - playerPos.x) + Mathf.Abs(y - playerPos.y); // �����پ���
                if (distance <= moveRange)
                {
                    GameObject tile = GetTile(x, y);
                    if (tile != null)
                    {
                        GameGrid gridComp = tile.GetComponent<GameGrid>();
                        gridComp.SetColor(gridComp.moveRangeColor);
                        gridComp.isInRange = true; // ����Ϊ���ƶ�
                    }
                }
            }
        }
    }


    public void ClearHighlight()
    {
        foreach (var tile in grid)
        {
            GameGrid gridComp = tile.GetComponent<GameGrid>();
            gridComp.ResetColor();       // �ָ���ɫ
            gridComp.isInRange = false;  // ���ÿ��ƶ�״̬
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
                if (!neighbor.walkable || closedList.Contains(neighbor)) continue;

                int newGCost = currentNode.gCost + GetDistance(currentNode, neighbor);
                if (newGCost < neighbor.gCost || !openList.Contains(neighbor))
                {
                    neighbor.gCost = newGCost;
                    neighbor.hCost = GetDistance(neighbor, targetNode);
                    neighbor.parent = currentNode;

                    if (!openList.Contains(neighbor)) openList.Add(neighbor);
                }
            }
        }

        return null; // û��·��
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

}
