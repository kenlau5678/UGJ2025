using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    [Header("�Ի�����")]
    public TextAsset dialogDataFile;

    [Header("UI")]
    public Image spriteLeft;
    public Image spriteRight;
    public Text nameText;
    public Text dialogText;
    public Button nextButton;
    public GameObject optionButtonPrefab;
    public Transform buttonGroup;

    [Header("��ɫ����")]
    public List<Sprite> sprites;
    private Dictionary<string, Sprite> imageDic = new Dictionary<string, Sprite>();

    // �Ի�����
    private Dictionary<int, List<DialogueLine>> dialogueDic = new Dictionary<int, List<DialogueLine>>();
    public int currentID = 1;

    private void Awake()
    {
        // ���� sprites[0] = ҽ��, sprites[1] = ���� ...
        imageDic["ҽ��"] = sprites[0];
        imageDic["����"] = sprites[1];
    }

    private void Start()
    {
        LoadCSV(dialogDataFile);
        ShowDialogue(currentID);
    }

    void LoadCSV(TextAsset csvFile)
    {
        dialogueDic.Clear(); // ����գ������μ��س���

        string[] lines = csvFile.text.Split(new char[] { '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
        bool hasHeader = false;

        foreach (string rawLine in lines)
        {
            string line = rawLine.Trim(); // ȥ��ǰ��ո�� \r
            if (string.IsNullOrEmpty(line)) continue;

            // �ж��ǲ��Ǳ�ͷ����һ�а��� "ID" �� "Type" ��������
            if (!hasHeader && (line.Contains("ID") || line.Contains("Type")))
            {
                hasHeader = true;
                continue;
            }

            // ֧����Ӣ�Ķ��� / �ֺ�
            string[] cells = line.Split(',');
            if (cells.Length < 2) continue;

            DialogueLine dia = new DialogueLine(cells);

            if (!dialogueDic.ContainsKey(dia.ID))
                dialogueDic[dia.ID] = new List<DialogueLine>();

            dialogueDic[dia.ID].Add(dia);

            Debug.Log($"�����ɹ� �� ID:{dia.ID}, Speaker:{dia.Speaker}, Pos:{dia.Position}, Text:{dia.Text}, Next:{dia.NextID}");
        }

        Debug.Log($"CSV ��ȡ��ɣ������� {dialogueDic.Count} ���Ի��ڵ�");
    }


    void ShowDialogue(int id)
    {
        if (!dialogueDic.ContainsKey(id))
        {
            Debug.LogWarning($"δ�ҵ��Ի� ID {id}");
            return;
        }

        for (int i = buttonGroup.childCount - 1; i >= 0; i--)
        {
            var child = buttonGroup.GetChild(i).gameObject;
            if (child != optionButtonPrefab) // ������ɾԭʼ prefab
                Destroy(child);
        }


        List<DialogueLine> lines = dialogueDic[id];
        DialogueLine first = lines[0];

        if (first.Type == "end")
        {
            Debug.Log("�������");
            nextButton.gameObject.SetActive(false);
            return;
        }
        else if (first.Type == "#") // ��ͨ�Ի�
        {
            UpdateUI(first);
            currentID = first.NextID;
            nextButton.gameObject.SetActive(true);
        }
        else if (first.Type == "@") // ѡ��
        {
            nextButton.gameObject.SetActive(false);
            foreach (DialogueLine option in lines)
            {
                GameObject btn = Instantiate(optionButtonPrefab, buttonGroup);
                btn.GetComponentInChildren<Text>().text = option.Text; // ע�������� Text
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

        // Ĭ����������
        spriteLeft.gameObject.SetActive(false);
        spriteRight.gameObject.SetActive(false);

        if (line.Position == "��" && imageDic.ContainsKey(line.Speaker))
        {
            spriteLeft.sprite = imageDic[line.Speaker];
            spriteLeft.gameObject.SetActive(true);
        }
        else if (line.Position == "��" && imageDic.ContainsKey(line.Speaker))
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
