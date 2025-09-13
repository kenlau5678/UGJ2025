using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerInMap : MonoBehaviour
{
    private MapGridManager mapManager;
    public MapGrid currentGrid;
    private List<MapGrid> highlightedGrids = new List<MapGrid>();

    void Start()
    {
        mapManager = FindObjectOfType<MapGridManager>();
        if (mapManager == null)
        {
            Debug.LogError("δ�ҵ� MapGridManager��");
            return;
        }

        currentGrid = GetComponentInParent<MapGrid>();
        if (currentGrid != null)
        {
            currentGrid.SetVisited();
            SetUpperGridsGray();
            HighlightNeighbors();
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);

            RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);
            if (hit.collider != null)
            {
                MapGrid clickedGrid = hit.collider.GetComponent<MapGrid>();
                if (clickedGrid != null)
                {
                    TryMoveTo(clickedGrid);
                    
                }
            }
        }
    }

    private IEnumerator LoadSceneAfterDelay()
    {
        yield return new WaitForSeconds(1f); // �ȴ� delayTime ��
        SceneManager.LoadScene("Test");          // ���س���
    }

    void HighlightNeighbors()
    {
        ClearHighlights();

        Vector2Int[] directions = new Vector2Int[]
        {
            new Vector2Int(0, -1), // ��
            new Vector2Int(-1, 0), // ��
            new Vector2Int(1, 0),  // ��
            new Vector2Int(0, 1)   // ��
        };

        //����ھ����Ƿ�������� (2,0)
        MapGrid priorityGrid = null;
        foreach (var dir in directions)
        {
            Vector2Int neighborPos = currentGrid.gridPos + dir;
            if (neighborPos == new Vector2Int(2, 0))
            {
                GameObject neighborObj = mapManager.GetGridObject(neighborPos);
                if (neighborObj != null)
                {
                    priorityGrid = neighborObj.GetComponent<MapGrid>();
                    break;
                }
            }
        }

        if (priorityGrid != null)
        {
            // ֻ�� (2,0) �����������ھ�������ɫ
            foreach (var dir in directions)
            {
                Vector2Int neighborPos = currentGrid.gridPos + dir;
                GameObject neighborObj = mapManager.GetGridObject(neighborPos);
                if (neighborObj == null) continue;

                MapGrid grid = neighborObj.GetComponent<MapGrid>();
                if (grid != null)
                {
                    if (grid == priorityGrid)
                    {
                        grid.SetHighlight(grid.gridColor);
                        highlightedGrids.Add(grid);
                    }
                    else
                    {
                        grid.SetVisited();
                    }
                }
            }
            return;
        }

        foreach (var dir in directions)
        {
            Vector2Int neighborPos = currentGrid.gridPos + dir;
            GameObject neighborObj = mapManager.GetGridObject(neighborPos);
            if (neighborObj == null) continue;

            MapGrid grid = neighborObj.GetComponent<MapGrid>();
            if (grid != null)
            {
                // ��������ѻ�ɫ����������
                if (grid.IsGray()) continue;

                // ��ĸ����߼�
                grid.SetHighlight(grid.gridColor);
                highlightedGrids.Add(grid);
            }
        }

    }

    void ClearHighlights()
    {
        foreach (var grid in highlightedGrids)
        {
            // ���߹��ĸ��ӱ��ֻ�ɫ�����ָ�
            if (grid != null) continue;
        }
        highlightedGrids.Clear();
    }

    public void TryMoveTo(MapGrid targetGrid)
    {
        if (highlightedGrids.Contains(targetGrid))
        {
            transform.SetParent(targetGrid.transform);
            transform.localPosition = Vector3.zero;

            currentGrid = targetGrid;
            currentGrid.SetVisited();
            SetUpperGridsGray();
            HighlightNeighbors();
            ResetFarGrids(); // ��֤Զ����ҵĸ��ӻָ���ɫ

            if(targetGrid.type == 2)
            {
                SceneManager.LoadScene("Store");
            }
        }
    }

    void SetUpperGridsGray()
    {
        foreach (var kvp in mapManager.gridDict)
        {
            MapGrid grid = kvp.Value.GetComponent<MapGrid>();
            if (grid != null && grid.gridPos.y > currentGrid.gridPos.y)
            {
                grid.SetVisited();
            }
        }
    }


    void ResetFarGrids()
    {
        Vector2Int[] directions = new Vector2Int[]
        {
        new Vector2Int(0, -1), // ��
        new Vector2Int(-1, 0), // ��
        new Vector2Int(1, 0),  // ��
        new Vector2Int(0, 1)   // ��
        };

        foreach (var kvp in mapManager.gridDict)
        {
            MapGrid grid = kvp.Value.GetComponent<MapGrid>();
            if (grid == null) continue;

            // ������ҵ�ǰ���Ӽ����������ھ�
            bool keepColor = grid == currentGrid;
            foreach (var dir in directions)
            {
                if (grid.gridPos == currentGrid.gridPos + dir)
                {
                    keepColor = true;
                    break;
                }
            }

            if (!keepColor)
            {
                grid.ResetIfFarFromPlayer(); // �����ھ� �� �ָ���ɫ
            }
        }
    }

}
