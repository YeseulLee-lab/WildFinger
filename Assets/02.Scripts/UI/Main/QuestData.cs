using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

[CreateAssetMenu]
public class QuestData : ScriptableObject
{
    [SerializeField]
    private QuestInfo[] _questDatas;
    [SerializeField]
    private Sprite _videoFirstFrame;
    [SerializeField]
    private Sprite[] _videoLastFrames;
    [SerializeField]
    private VideoClip[] _videoClips;
    [SerializeField]
    private bool _isLooping;
    //[SerializeField] private VideoClip _fullVideo; //원래 아라비안 마을에서 썼지만, 지금은 안 씀


    public QuestInfo[] questDatas => _questDatas;
    public Sprite videoFirstFrame => _videoFirstFrame;
    public Sprite[] videoLastFrames => _videoLastFrames;
    public VideoClip[] videoClips => _videoClips;
    public bool isLooping => _isLooping;
//    public VideoClip fullVideo => _fullVideo;
}
