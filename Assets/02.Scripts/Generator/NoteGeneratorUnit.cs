using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NoteGeneratorUnit : BaseObjectPoolUnit
{
    [field: SerializeField]
    public Define.NoteTypeN gimmickType;

    [Header("------------------ GUI Setting -----------------")]
    [SerializeField]
    private Image _gimmickImg;
    [SerializeField]
    private UnityEngine.UI.Text _gimmickNameText;
    [SerializeField]
    private InputField _totalCntInputField;
    private int _gimmickCnt = 0;
    /// <summary>
    /// 해당 기믹 노트 개수
    /// </summary>
    public int gimmickCnt { get {
            return _gimmickCnt;
        } set {
            _gimmickCnt = value;
            _totalCntInputField.text = value.ToString();
        } }
    public NoteGeneratorUnitInfo info { get; set; } = null;
    [SerializeField]
    private Color32[] _colors; //available, unavailable

    #region Unity Life Cycle
    private void Start()
    {
        _totalCntInputField.onValueChanged.AddListener((cnt) => OnTotalCntInputFieldValueChanged(cnt));
    }
    #endregion

    public void InitInfo(NoteGeneratorUnitInfo info, bool isAvailable)
    {
        _gimmickImg.sprite = info.gimmickImg;
        _gimmickNameText.text = info.gimmickName;
        gimmickCnt = 0;
        this.info = info;

        this.gameObject.SetActive(isAvailable);
        this.GetComponent<Image>().color = _colors[isAvailable ? 0 : 1];
    }

    private void OnTotalCntInputFieldValueChanged(string cnt)
    {
        int numCnt = 0;
        if (!int.TryParse(cnt, out _) || NoteGeneratorByEditor.Instance == null)
        {
            Debug.Log($"{_gimmickNameText.text} 입력 불가능");
        }
        else
        {
            if (numCnt < 0)
            {
                Debug.Log($"{_gimmickNameText.text} 음수 불가능");
                numCnt = 0;
            }
            numCnt = int.Parse(cnt);
        }

        _gimmickCnt = numCnt;
    }
}