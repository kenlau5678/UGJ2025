using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class GridTypePrefab
{
    public int type;               // 格子类型，例如 1,2,3
    public GameObject prefabs;   // 对应类型的Prefab数组，可在Inspector中随机选择
}
public class MapGridManager : MonoBehaviour
{
    public TextAsset csvFile;
    public GameObject grid2Prefab; // 新增：2类型格子的Prefab
    public GameObject grid3Prefab; 
    public GameObject playerPrefab;   //新增：玩家预制体
    public float cellSize = 1f;


    

    public Dictionary<Vector2Int, GameObject> gridDict = new Dictionary<Vector2Int, GameObject>();

    public GridTypePrefab[] grid1Prefabs; // 1类型格子随机选择的Prefab数组
    void Start()
    {
        if (csvFile == null)
        {
            Debug.LogError("请在 Inspector 中拖入 CSV 文件！");
            return;
        }

        LoadMapFromCSV(csvFile.text);

        //游戏开始时在 (1,5) 生成 Player
        SpawnPlayerAt(new Vector2Int(1, 5));
    }

    void LoadMapFromCSV(string csvText)
    {
        string[] lines = csvText.Split('\n');
        int type = 0;
        for (int y = 0; y < lines.Length; y++)
        {
            string line = lines[y].Trim();
            if (string.IsNullOrEmpty(line)) continue;

            string[] values = line.Split(new char[] { '\t', ',' });

            for (int x = 0; x < values.Length; x++)
            {
                Vector2Int gridPos = new Vector2Int(x, y);
                Vector3 localPos = new Vector3(x * cellSize, -y * cellSize, 0);

                GameObject prefabToUse = null;

                if (values[x] == "1")
                {
                    // 从数组中随机选一个Prefab
                    if (grid1Prefabs.Length > 0)
                    {
                        int index = Random.Range(0, grid1Prefabs.Length);
                        prefabToUse = grid1Prefabs[index].prefabs;
                        type = grid1Prefabs[index].type;
                    }
                    
                }
                else if (values[x] == "2")
                {
                    prefabToUse = grid2Prefab; // 原来的2格子Prefab
                    type = 2;
                }
                else if (values[x] == "3")
                {
                    prefabToUse = grid3Prefab; // 原来的2格子Prefab
                    type = 3;
                }



                if (prefabToUse != null)
                {
                    GameObject obj = Instantiate(prefabToUse, transform);
                    obj.transform.localPosition = localPos;

                    MapGrid gridComp = obj.GetComponent<MapGrid>();
                    if (gridComp != null)
                    {
                        gridComp.gridPos = gridPos;
                        gridComp.type = type;
                        type = 0;
                    }

                    gridDict[gridPos] = obj;
                }
            }
        }
    }



    public GameObject GetGridObject(Vector2Int gridPos)
    {
        if (gridDict.TryGetValue(gridPos, out GameObject obj))
        {
            return obj;
        }
        return null;
    }

    public void SpawnPlayerAt(Vector2Int gridPos)
    {
        GameObject gridObj = GetGridObject(gridPos);
        if (gridObj != null && playerPrefab != null)
        {
            GameObject player = Instantiate(playerPrefab, gridObj.transform);
            player.transform.localPosition = Vector3.zero;

            //起始格子变灰
            MapGrid gridComp = gridObj.GetComponent<MapGrid>();
            if (gridComp != null)
            {
                gridComp.SetVisited();
            }
        }
        else
        {
            Debug.LogWarning("目标格子不存在，或 PlayerPrefab 未设置！");
        }
    }

}
