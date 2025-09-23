using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    /// <summary>
    /// 对话内容文本，csv格式
    /// </summary> 
    public TextAsset dialogDataFile;

    /// <summary>
    /// 左侧角色图像
    /// </summary>
    public SpriteRenderer spriteLeft;
    /// <summary>
    /// 右侧角色图像
    /// </summary>
    public SpriteRenderer spriteRight;

    /// <summary>
    /// 角色名字文本
    /// </summary>
    public TMP_Text nameText;

    /// <summary>
    /// 对话内容文本
    /// </summary>
    public TMP_Text dialogText;

    /// <summary>
    /// 角色图片列表
    /// </summary>
    public List<Sprite> sprites = new List<Sprite>();

    /// <summary>
    /// 角色名字对应图片的字典
    /// </summary>
    Dictionary<string, Sprite> imageDic = new Dictionary<string, Sprite>();

    /// <summary>
    /// 当前对话索引值
    /// </summary>
    public int dialogIndex;

    /// <summary>
    /// 对话文本按行分割
    /// </summary>
    public string[] dialogRows;

    /// <summary>
    /// 继续按钮
    /// </summary>
    public Button next;

    /// <summary>
    /// 选项按钮
    /// </summary>
    public GameObject optionButton;
    /// <summary>
    /// 选项按钮父节点
    /// </summary>
    public Transform buttonGroup;

    // Start is called before the first frame update
    private void Awake()
    {
        imageDic["医生"] = sprites[0];
        imageDic["弗兰"] = sprites[1];
    }

    void Start()
    {
        ReadText(dialogDataFile);
        ShowDiaLogRow();
    }

    // 更新文本信息
    public void UpdateText(string _name, string _text)
    {
        nameText.text = _name;
        dialogText.text = _text;
    }

    // 更新图片信息
    public void UpdateImage(string _name, string _position)
    {
        if (_position == "左")
        {
            spriteLeft.sprite = imageDic[_name];
        }
        else if (_position == "右")
        {
            spriteRight.sprite = imageDic[_name];
        }
    }

    public void ReadText(TextAsset _textAsset)
    {
        dialogRows = _textAsset.text.Split('\n'); // 以换行来分割
        Debug.Log("读取成果");
    }

    public void ShowDiaLogRow()
    {
        for (int i = 0; i < dialogRows.Length; i++)
        {
            string[] cells = dialogRows[i].Split(',');

            if (cells[0] == "#" && int.Parse(cells[1]) == dialogIndex)
            {
                UpdateText(cells[2], cells[4]);
                UpdateImage(cells[2], cells[3]);

                dialogIndex = int.Parse(cells[5]);
                next.gameObject.SetActive(true);
                break;
            }
            else if (cells[0] == "@" && int.Parse(cells[1]) == dialogIndex)
            {
                next.gameObject.SetActive(false); // 隐藏原来的按钮
                GenerateOption(i);
            }
            else if (cells[0] == "end" && int.Parse(cells[1]) == dialogIndex)
            {
                Debug.Log("剧情结束"); // 这里结束
                next.gameObject.SetActive(false); // 隐藏“下一步”按钮
                return; // 直接退出
            }
        }
    }

    public void OnClickNext()
    {
        ShowDiaLogRow();
    }

    public void GenerateOption(int _index) // 生成按钮
    {
        if (_index >= dialogRows.Length) return; // 防止数组越界

        string[] cells = dialogRows[_index].Split(',');
        if (cells[0] == "@")
        {
            GameObject button = Instantiate(optionButton, buttonGroup);

            // 绑定按钮事件
            button.GetComponentInChildren<TMP_Text>().text = cells[4];
            button.GetComponent<Button>().onClick.AddListener(delegate
            {
                OnOptionClick(int.Parse(cells[5]));
            });

            GenerateOption(_index + 1); // 递归生成下一个选项
        }
    }

    public void OnOptionClick(int _id)
    {
        dialogIndex = _id;
        ShowDiaLogRow();

        // 清空旧按钮
        for (int i = 0; i < buttonGroup.childCount; i++)
        {
            Destroy(buttonGroup.GetChild(i).gameObject);
        }
    }
}
