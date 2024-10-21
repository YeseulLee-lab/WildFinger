using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class InGameNoteRoadManager : MonoBehaviour
{
    [Header("------------------ Background Setting -----------------")]
    [SerializeField]
    private Image _backgroundFrontImg;
    [SerializeField]
    private Sprite[] _backgrounds;
    [SerializeField]
    private Sprite[] _feverBackgrounds;
    private int _curBackgroundIndex;
    private int _perBeatCnt;

    [Header("------------------ Road Setting -----------------")]
    [SerializeField]
    private BaseInGameNoteRoad _noteRoad;
    public BaseInGameNoteRoad noteRoad => _noteRoad;
    private const float _backgroundChangeDelay = 0.45f;

    #region Unity Life Cycle
    private void Start()
    {
        _perBeatCnt = 0;
        _curBackgroundIndex = 0;
        SetBackground(false);
    }

    private void OnDestroy()
    {
        //TODO: 리소스 할당 해제
        _backgroundFrontImg = null;
        _backgrounds = null;
        _noteRoad = null;
    }
    #endregion

    #region note Place
    public void StartNoteMoving(MusicInfo curMusicInfo, float distancePerBeat, Queue<NoteInfo> noteQueue)
    {
        _noteRoad.gameObject.SetActive(true);
        _noteRoad.StartNoteMoving(distancePerBeat, noteQueue);
    }

    public void EndNoteMoving()
    {
        if(_noteRoad == null)
        {
            return;
        }
        _noteRoad.EndNoteMoving();
    }

    public void NextNoteMoving()
    {
        _noteRoad.NextNoteMoving();

        _backgroundFrontImg.sprite = BeatGridTracker.Instance.feverManager.isFever? _feverBackgrounds[++_perBeatCnt % _feverBackgrounds.Length] : _backgrounds[++_perBeatCnt % _backgrounds.Length];
    }

    public void PrevNoteMoving()
    {
        _noteRoad.PrevNoteMoving();
    }

    public void StayNoteMoving()
    {
        _noteRoad.StayNoteMoving();
    }
    #endregion

    #region UI Action
    public void SetBackground(bool isFever)
    {
        _backgroundFrontImg.sprite = isFever? _feverBackgrounds[0] : _backgrounds[0];
    }
    #endregion
}
