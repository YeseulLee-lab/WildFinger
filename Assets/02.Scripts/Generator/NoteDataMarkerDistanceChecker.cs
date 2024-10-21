using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using System;
using System.Runtime.InteropServices;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Text;
using System.IO;
using TMPro;
using UnityEngine.Networking;
using UnityEngine.Video;
using DG.Tweening;

public class NoteDataMarkerDistanceChecker : StreamingAssetPather
{
    [SerializeField]
    private int _startLevel;
    [SerializeField]
    private bool _isEndlessGeneratingMode;
    private static StreamWriter _sw;
    private StageInfo _stageInfo = null;
    private string _stageJson = null;
    public NoteInfo[] noteData { get; set; }
    private List<NoteInfo> _bars;

    private void Start()
    {
        LoadNoteData(ref _startLevel);
    }

    public void LoadNoteData(ref int level)
    {
        if (level > 440)
        {
            Debug.Log($"노트 생성 가능 최대 레벨 달성. 종료합니다.");
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
            return;
        }

        string filePath;
        Define.TownList town = SceneSwitcher.GetTownList(level);

        Debug.Log($"{town}-{level} 노트 생성 시작");

        _stageInfo = new StageInfo();
        filePath = GetFilePathCommon(town, level);
        if (File.Exists(filePath))
        {
            _stageJson = File.ReadAllText(filePath);
            _stageInfo = JsonUtility.FromJson<StageInfo>(_stageJson);
            noteData = _stageInfo.noteDatas.ToArray();
            _bars = new List<NoteInfo>();
            for (int i = 0; i < noteData.Length; i++)
            {
                if (noteData[i].noteType == Define.NoteType.Bar)
                {
                    _bars.Add(noteData[i]);
                }
                if (_bars.Count >= 2)
                {
                    _stageInfo.distancePerMarker = (_bars[1].position - _bars[0].position) / _stageInfo.beatCnt;
                    _stageInfo.bpm = _bars[1].tempo;
                    WriteJson(town, level);
                    break;
                }
            }

            if (_isEndlessGeneratingMode)
            {
                level++;
                LoadNoteData(ref level);
            }
        }
        else
        {
            Debug.LogError("File not found: " + filePath);
            return;
        }
    }

    public void WriteJson(Define.TownList town , int level)
    {
        string filePath = GetFilePathCommon(town, level);

        // 경로가 없으면 폴더 생성
        if (!Directory.Exists(Path.GetDirectoryName(filePath)))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));
        }

        // 파일 생성
        _sw = new StreamWriter(filePath);
        _sw.WriteLine(JsonHelper.ObjectToJson(_stageInfo));
        _sw.Flush();
        _sw.Close();
        Debug.Log($"{TownDataLoader.town}-{TownDataLoader.level}, {filePath} =>  총 마커수: {_stageInfo.totalBeatMarkerCnt}, NoteData 저장 완료 Json");
    }
}
