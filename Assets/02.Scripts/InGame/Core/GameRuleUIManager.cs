using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class GameRuleUIManager : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _curStageText;
    [SerializeField]
    private TextMeshProUGUI _curStageShadowText;

    [Header("------------------ Input Areas -----------------")]
    [SerializeField]
    private GameObject[] _inputAreas;
    [SerializeField]
    private Button[] _rspBtns;
    [SerializeField]
    private Button[] _logicBtns;

    [Header("------------------ Line Areas -----------------")]
    [SerializeField]
    private Image _lineImg;
    [SerializeField]
    private Sprite[] _lineSelectedImgs; // Perfect, Good, Miss, Default
    [SerializeField]
    private Image _lineVFXImg;
    [SerializeField]
    private Sprite[] _lineVFXImgs; // Perfect, Good, Miss, Default
    private float _lineVFXImgHeight = 204f;

    #region Unity Life Cycle
    private void Awake()
    {
        _lineVFXImgHeight = _lineVFXImg.rectTransform.sizeDelta.y;
    }

    private void OnDestroy()
    {
        _inputAreas = null;
        _lineImg = null;
        _curStageShadowText = null;
        _curStageText = null;
    }
    #endregion

    /// <summary>
    /// 위에 뜨는 법칙 세팅
    /// </summary>
    /// <param name="ruleType"></param>
    /// <param name="inputType">0: RSP, 1: Logic</param>
    public void SetInputUI(Define.InputType inputType, InitInputActive initInputActive)
    {
        //DebugX.Log("InitInputUI: " + inputType);
        _inputAreas[(int)inputType].SetActive(true);
        
        if(inputType == Define.InputType.RSP)
        {
            for (int i = 0; i < initInputActive.inputs.Length; i++)
            {
                SetRSPBtn((Define.RSPType)i, initInputActive.inputs[i]);
            }
        }
        else
        {
            for (int i = 0; i < initInputActive.inputs.Length; i++)
            {
                SetLogicBtn((Define.LogicType)i, initInputActive.inputs[i]);
            }
        }
        
    }

    /// <summary> 
    /// RuleType 이 StLogic, StRSP, StRSPRvs에서 사용. 
    /// </summary>
    /// <param name="type">RSP or Logic Type을 정수로 형 변환해서 사용. RSP => 0: Rock, 1: Scissor, 2: Paper, Logic => 0: Win, 1: Draw, 2: Lose</param>
    public void SetSituationUI(int type = 0)
    {
        //DebugX.Log("현재 상황: " + (Define.LogicType)type);
    }

    public void SetCurStageNum(int curStage)
    {
        _curStageText.text = curStage.ToString();
        _curStageShadowText.text = curStage.ToString();
    }

    #region VFX
    public void SelectLine(Define.NoteJudge judge = Define.NoteJudge.None, bool isProtected = false)
    {
        //시간차
        if(judge == Define.NoteJudge.None)
        {
            _lineImg.sprite = _lineSelectedImgs[3];
            _lineVFXImg.gameObject.SetActive(false);
            return;
        }

        _lineImg.sprite = _lineSelectedImgs[(int)judge];
        _lineVFXImg.gameObject.SetActive(true);
        _lineVFXImg.rectTransform.sizeDelta = new Vector2(_lineVFXImg.rectTransform.sizeDelta.x, 0f);
        _lineVFXImg.rectTransform.DOSizeDelta(new Vector2(_lineVFXImg.rectTransform.sizeDelta.x, _lineVFXImgHeight), 0.2f);
        _lineVFXImg.sprite = _lineVFXImgs[(int)judge];
    }
    #endregion

    #region Buttons Situation
    public void SetRSPBtn(Define.RSPType type, bool active)
    {
        if (type == Define.RSPType.None)
        {
            return;
        }

        _rspBtns[(int)type].gameObject.SetActive(active);
    }

    public void SetLogicBtn(Define.LogicType type, bool active)
    {
        if (type == Define.LogicType.None)
        {
            return;
        }

        _logicBtns[(int)type].gameObject.SetActive(active);
    }
    #endregion
}
