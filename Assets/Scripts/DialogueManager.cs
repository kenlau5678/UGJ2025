using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    /// <summary>
    /// �Ի������ı���csv��ʽ
    /// </summary> 
    public TextAsset dialogDataFile;

    /// <summary>
    /// ����ɫͼ��
    /// </summary>
    public SpriteRenderer spriteLeft;
    /// <summary>
    /// �Ҳ��ɫͼ��
    /// </summary>
    public SpriteRenderer spriteRight;

    /// <summary>
    /// ��ɫ�����ı�
    /// </summary>
    public TMP_Text nameText;

    /// <summary>
    /// �Ի������ı�
    /// </summary>
    public TMP_Text dialogText;

    /// <summary>
    /// ��ɫͼƬ�б�
    /// </summary>
    public List<Sprite> sprites = new List<Sprite>();

    /// <summary>
    /// ��ɫ���ֶ�ӦͼƬ���ֵ�
    /// </summary>
    Dictionary<string, Sprite> imageDic = new Dictionary<string, Sprite>();

    /// <summary>
    /// ��ǰ�Ի�����ֵ
    /// </summary>
    public int dialogIndex;

    /// <summary>
    /// �Ի��ı����зָ�
    /// </summary>
    public string[] dialogRows;

    /// <summary>
    /// ������ť
    /// </summary>
    public Button next;

    /// <summary>
    /// ѡ�ť
    /// </summary>
    public GameObject optionButton;
    /// <summary>
    /// ѡ�ť���ڵ�
    /// </summary>
    public Transform buttonGroup;

    // Start is called before the first frame update
    private void Awake()
    {
        imageDic["ҽ��"] = sprites[0];
        imageDic["����"] = sprites[1];
    }

    void Start()
    {
        ReadText(dialogDataFile);
        ShowDiaLogRow();
    }

    // �����ı���Ϣ
    public void UpdateText(string _name, string _text)
    {
        nameText.text = _name;
        dialogText.text = _text;
    }

    // ����ͼƬ��Ϣ
    public void UpdateImage(string _name, string _position)
    {
        if (_position == "��")
        {
            spriteLeft.sprite = imageDic[_name];
        }
        else if (_position == "��")
        {
            spriteRight.sprite = imageDic[_name];
        }
    }

    public void ReadText(TextAsset _textAsset)
    {
        dialogRows = _textAsset.text.Split('\n'); // �Ի������ָ�
        Debug.Log("��ȡ�ɹ�");
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
                next.gameObject.SetActive(false); // ����ԭ���İ�ť
                GenerateOption(i);
            }
            else if (cells[0] == "end" && int.Parse(cells[1]) == dialogIndex)
            {
                Debug.Log("�������"); // �������
                next.gameObject.SetActive(false); // ���ء���һ������ť
                return; // ֱ���˳�
            }
        }
    }

    public void OnClickNext()
    {
        ShowDiaLogRow();
    }

    public void GenerateOption(int _index) // ���ɰ�ť
    {
        if (_index >= dialogRows.Length) return; // ��ֹ����Խ��

        string[] cells = dialogRows[_index].Split(',');
        if (cells[0] == "@")
        {
            GameObject button = Instantiate(optionButton, buttonGroup);

            // �󶨰�ť�¼�
            button.GetComponentInChildren<TMP_Text>().text = cells[4];
            button.GetComponent<Button>().onClick.AddListener(delegate
            {
                OnOptionClick(int.Parse(cells[5]));
            });

            GenerateOption(_index + 1); // �ݹ�������һ��ѡ��
        }
    }

    public void OnOptionClick(int _id)
    {
        dialogIndex = _id;
        ShowDiaLogRow();

        // ��վɰ�ť
        for (int i = 0; i < buttonGroup.childCount; i++)
        {
            Destroy(buttonGroup.GetChild(i).gameObject);
        }
    }
}
