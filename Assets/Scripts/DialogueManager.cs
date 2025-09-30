using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    [Header("对话数据")]
    public TextAsset dialogDataFile;

    [Header("UI")]
    public Image spriteLeft;
    public Image spriteRight;
    public Text nameText;
    public Text dialogText;
    public Button nextButton;
    public GameObject optionButtonPrefab;
    public Transform buttonGroup;

    [Header("角色立绘")]
    public List<Sprite> sprites;
    private Dictionary<string, Sprite> imageDic = new Dictionary<string, Sprite>();

    // 对话数据
    private Dictionary<int, List<DialogueLine>> dialogueDic = new Dictionary<int, List<DialogueLine>>();
    public int currentID = 1;

    private void Awake()
    {
        // 假设 sprites[0] = 医生, sprites[1] = 弗兰 ...
        imageDic["医生"] = sprites[0];
        imageDic["弗兰"] = sprites[1];
    }

    private void Start()
    {
        LoadCSV(dialogDataFile);
        ShowDialogue(currentID);
    }

    void LoadCSV(TextAsset csvFile)
    {
        dialogueDic.Clear(); // 先清空，避免多次加载出错

        string[] lines = csvFile.text.Split(new char[] { '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
        bool hasHeader = false;

        foreach (string rawLine in lines)
        {
            string line = rawLine.Trim(); // 去掉前后空格和 \r
            if (string.IsNullOrEmpty(line)) continue;

            // 判断是不是表头（第一行包含 "ID" 或 "Type" 就跳过）
            if (!hasHeader && (line.Contains("ID") || line.Contains("Type")))
            {
                hasHeader = true;
                continue;
            }

            // 支持中英文逗号 / 分号
            string[] cells = line.Split(',');
            if (cells.Length < 2) continue;

            DialogueLine dia = new DialogueLine(cells);

            if (!dialogueDic.ContainsKey(dia.ID))
                dialogueDic[dia.ID] = new List<DialogueLine>();

            dialogueDic[dia.ID].Add(dia);

            Debug.Log($"解析成功 → ID:{dia.ID}, Speaker:{dia.Speaker}, Pos:{dia.Position}, Text:{dia.Text}, Next:{dia.NextID}");
        }

        Debug.Log($"CSV 读取完成，共载入 {dialogueDic.Count} 个对话节点");
    }


    void ShowDialogue(int id)
    {
        if (!dialogueDic.ContainsKey(id))
        {
            Debug.LogWarning($"未找到对话 ID {id}");
            return;
        }

        for (int i = buttonGroup.childCount - 1; i >= 0; i--)
        {
            var child = buttonGroup.GetChild(i).gameObject;
            if (child != optionButtonPrefab) // 避免误删原始 prefab
                Destroy(child);
        }


        List<DialogueLine> lines = dialogueDic[id];
        DialogueLine first = lines[0];

        if (first.Type == "end")
        {
            Debug.Log("剧情结束");
            nextButton.gameObject.SetActive(false);
            return;
        }
        else if (first.Type == "#") // 普通对话
        {
            UpdateUI(first);
            currentID = first.NextID;
            nextButton.gameObject.SetActive(true);
        }
        else if (first.Type == "@") // 选项
        {
            nextButton.gameObject.SetActive(false);
            foreach (DialogueLine option in lines)
            {
                GameObject btn = Instantiate(optionButtonPrefab, buttonGroup);
                btn.GetComponentInChildren<Text>().text = option.Text; // 注意这里用 Text
                btn.GetComponent<Button>().onClick.AddListener(() =>
                {
                    currentID = option.NextID;
                    ShowDialogue(currentID);
                });
            }
        }
    }

    void UpdateUI(DialogueLine line)
    {
        nameText.text = line.Speaker;
        dialogText.text = line.Text;

        // 默认隐藏两边
        spriteLeft.gameObject.SetActive(false);
        spriteRight.gameObject.SetActive(false);

        if (line.Position == "左" && imageDic.ContainsKey(line.Speaker))
        {
            spriteLeft.sprite = imageDic[line.Speaker];
            spriteLeft.gameObject.SetActive(true);
        }
        else if (line.Position == "右" && imageDic.ContainsKey(line.Speaker))
        {
            spriteRight.sprite = imageDic[line.Speaker];
            spriteRight.gameObject.SetActive(true);
        }
    }


    public void OnClickNext()
    {
        ShowDialogue(currentID);
    }
}

public class DialogueLine
{
    public string Type;     // # / @ / end
    public int ID;
    public string Speaker;
    public string Position;
    public string Text;
    public int NextID;

    public DialogueLine(string[] cells)
    {
        Type = cells[0].Trim();
        int.TryParse(cells[1].Trim(), out ID);
        Speaker = cells[2].Trim();
        Position = cells[3].Trim();
        Text = cells[4].Trim();
        int.TryParse(cells[5].Trim(), out NextID);
    }
}
