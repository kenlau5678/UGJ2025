using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class GridTypePrefab
{
    public int type;               // �������ͣ����� 1,2,3
    public GameObject prefabs;   // ��Ӧ���͵�Prefab���飬����Inspector�����ѡ��
}
public class MapGridManager : MonoBehaviour
{
    public TextAsset csvFile;
    public GameObject grid2Prefab; // ������2���͸��ӵ�Prefab
    public GameObject grid3Prefab; 
    public GameObject playerPrefab;   //���������Ԥ����
    public float cellSize = 1f;


    

    public Dictionary<Vector2Int, GameObject> gridDict = new Dictionary<Vector2Int, GameObject>();

    public GridTypePrefab[] grid1Prefabs; // 1���͸������ѡ���Prefab����
    void Start()
    {
        if (csvFile == null)
        {
            Debug.LogError("���� Inspector ������ CSV �ļ���");
            return;
        }

        LoadMapFromCSV(csvFile.text);

        //��Ϸ��ʼʱ�� (1,5) ���� Player
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
                    // �����������ѡһ��Prefab
                    if (grid1Prefabs.Length > 0)
                    {
                        int index = Random.Range(0, grid1Prefabs.Length);
                        prefabToUse = grid1Prefabs[index].prefabs;
                        type = grid1Prefabs[index].type;
                    }
                    
                }
                else if (values[x] == "2")
                {
                    prefabToUse = grid2Prefab; // ԭ����2����Prefab
                    type = 2;
                }
                else if (values[x] == "3")
                {
                    prefabToUse = grid3Prefab; // ԭ����2����Prefab
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

            //��ʼ���ӱ��
            MapGrid gridComp = gridObj.GetComponent<MapGrid>();
            if (gridComp != null)
            {
                gridComp.SetVisited();
            }
        }
        else
        {
            Debug.LogWarning("Ŀ����Ӳ����ڣ��� PlayerPrefab δ���ã�");
        }
    }

}
