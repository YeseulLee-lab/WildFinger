using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HHK.UIEC;

[Serializable]
public class InGameNotePlace
{
    public RectTransform[] notePlaceRects; //갯수 각각 통일
}

public class BaseInGameNoteRoad : MonoBehaviour
{
    [Header("------------------ Road Setting -----------------")]
    [SerializeField]
    private InGameNotePlace[] _notePlaces; // 첫 칸이 판정존
    [SerializeField]
    private UIECAnimator _judgeLineAnim;
    [SerializeField]
    private UIECAnimator _bridgeAnim;
    private float _distancePerBeat { get; set; } = 0f;
    private Queue<NoteInfo> _noteQueue = null;
    private List<BaseNoteUnit> _activeNoteUnits = null;
    private List<BaseNoteUnit> _deactiveNoteUnits = null;

    [Header("---------------- Internal Note Data ---------------")]
    [SerializeField]
    private GameObject[] _lines;
    [SerializeField] 
    private InGameLineBG _lineBG;
    public NoteInfo curNoteInfo { get; private set; }
    public int curHeadIndex { get; set; }
    public int maxRoadPlaceCnt { get; private set; }
    private int _maxLineCnt = 1;

    #region Unity Life Cycle
    private void OnDestroy()
    {
        _notePlaces = null;
        _judgeLineAnim = null;
        _noteQueue = null;
        _activeNoteUnits = null;
        _lines = null;
        _lineBG = null;
        curNoteInfo = null;
        _bridgeAnim = null;
    }
    #endregion

    #region Note Action(public)
    public void SetLine(int maxLineCnt)
    {
        _maxLineCnt = maxLineCnt;
        for (int i=0; i<_lines.Length; i++)
        {
            _lines[i].SetActive(i <= maxLineCnt - 1);
            _lineBG.SetLineBGPattern(maxLineCnt, 0);
        }
    }

    public void StartNoteMoving(float distancePerBeat, Queue<NoteInfo> noteQueue)
    {
        if(BeatGridTracker.noteLoader.stageInfo == null)
        {
            DebugX.Log("[BaseInGameMonsterRoad] 노트데이터 로드 실패.");
            return;
        }

        //TODO: 시작 전 노트 Unit 미리 세팅(length 만큼)
        if (TownDataLoader.curMusicInfo == null)
        {
            DebugX.Log("TownDataLoader 현재 스테이지 정보 읽어오기 실패 => 노트 생성 불가능");
            return;
        }

        // DebugX.Log("[BaseInGameMonsterRoad] StartNoteMoving");
        //초기화
        _distancePerBeat = distancePerBeat;
        _noteQueue = new Queue<NoteInfo>(noteQueue);
        maxRoadPlaceCnt = _notePlaces[0].notePlaceRects.Length;
        curHeadIndex = maxRoadPlaceCnt - 1;
        _activeNoteUnits = new List<BaseNoteUnit>(maxRoadPlaceCnt + 1);
        _deactiveNoteUnits = new List<BaseNoteUnit>(maxRoadPlaceCnt + 1);
        //처음 노트 위치 세팅
        curNoteInfo = _noteQueue.Dequeue();
        if (curNoteInfo == null)
        {
            DebugX.Log("Note Road: curNoteInfo 없음");
            return;
        }
    }

    public void EndNoteMoving()
    {
        _distancePerBeat = 0;
        if(_noteQueue != null)
        {
            _noteQueue.Clear();
            _noteQueue = null;
        }
        curHeadIndex = maxRoadPlaceCnt;

        if (_activeNoteUnits != null)
        {
            _activeNoteUnits.Clear();
            _activeNoteUnits = null;
        }
    }

    /// <summary>
    /// 무조건 길 1개로 일단 구현, 이후 수정
    /// </summary>
    public void NextNoteMoving()
    {
        //다음 노트
        curHeadIndex++;
        PlayBarAnim(_maxLineCnt, curHeadIndex);
        if (curNoteInfo != null)
        {
            //DebugX.Log("curNoteInfo.position: " + curNoteInfo.position);
            if (_noteQueue.Count < 1)
            {
                //DebugX.Log("NextNoteMoving: Queue is Empty");
            }
            else
            {
                if (curNoteInfo.position < (float)(_distancePerBeat * (float)(curHeadIndex - 1)))
                {
                    //TODO: Note 배치
                    GameObject note = BaseObjectPool.Instance.Spawn(TownDataLoader.GetNoteKey(curNoteInfo.noteGimmickType));
                    _activeNoteUnits.Add(note.GetComponent<BaseNoteUnit>());
                    //DebugX.Log("_activeNoteUnitQueue Count : " + _activeNoteUnitQueue.Count);
                    if (TownDataLoader.curMusicInfo.lineCnt > 3)
                    {
                        TownDataLoader.curMusicInfo.lineCnt = 3;
                    }

                    if (curNoteInfo.noteGimmickType == Define.NoteTypeN.N_PAL ||
                        curNoteInfo.noteGimmickType == Define.NoteTypeN.N_PCK)
                    {
                        note.GetComponent<BaseNoteUnit>().SetUnit(_notePlaces[1].notePlaceRects, _notePlaces[1].notePlaceRects.Length - 1, curNoteInfo);
                    }
                    else
                    {
                        int randomRoadIndex = UnityEngine.Random.Range(0, TownDataLoader.curMusicInfo.lineCnt);
                        note.GetComponent<BaseNoteUnit>().SetUnit(_notePlaces[randomRoadIndex].notePlaceRects, _notePlaces[randomRoadIndex].notePlaceRects.Length - 1, curNoteInfo);
                    }
                    curNoteInfo = _noteQueue.Dequeue();
                }
            }
        }

        for (int i=0; i< _deactiveNoteUnits.Count; i++)
        {
            if (_activeNoteUnits.Contains(_deactiveNoteUnits[i]))
            {
                _activeNoteUnits.Remove(_deactiveNoteUnits[i]);
            }
        }
        _deactiveNoteUnits = new List<BaseNoteUnit>();

        //기존 노트 이동
        for (int i=0; i< _activeNoteUnits.Count; i++)
        {
            //Debug.Log("Move Note: " + i);
            _activeNoteUnits[i].MoveNote();
        }
        //DebugX.Log("_activeNoteUnits.Count: " + _activeNoteUnits.Count);
    }

    public void ReserveDeletingFromMovingNoteList(BaseNoteUnit noteUnit)
    {
        //Debug.Log("ReserveDeletingFromMovingNoteList: " + noteUnit.noteInfo.position);
        _deactiveNoteUnits.Add(noteUnit);
    }

    public void PrevNoteMoving()
    {
        if (_distancePerBeat <= 0f || _noteQueue == null)
        {
            return;
        }
    }

    public void StayNoteMoving()
    {
        if (_distancePerBeat <= 0f || _noteQueue == null)
        {
            return;
        }
    }

    private void PlayBarAnim(int maxLineCnt, int index)
    {
        _bridgeAnim.OnCustomChannel();
        _judgeLineAnim.OnCustomChannel();
        _lineBG.SetLineBGPattern(maxLineCnt, index);
    }
    #endregion
}
