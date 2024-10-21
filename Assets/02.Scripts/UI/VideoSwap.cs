using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoSwap : MonoBehaviour
{
    [SerializeField]
    private VideoClip[] _videoClips;
    public VideoClip[] videoClips
    {
        get { return _videoClips; }
        set { _videoClips = value; }
    }

    [SerializeField]
    private RawImage[] _videoRawImages;
    [SerializeField]
    private VideoPlayer[] _videoPlayers;
    [SerializeField]
    private float fadeRate;

    private int _curVideoIdx = 0;
    private bool _isVideoEnd = false;
    public bool isVideoEnd
    {
        get { return _isVideoEnd; }
        set { _isVideoEnd = value; }
    }

    private bool _isLooping = false;
    public bool isLooping
    {
        get { return _isLooping; }
        set 
        {
            _isLooping = value;
            _videoPlayers[(_curVideoIdx + 1) % 2].isLooping = _isLooping;
            _videoPlayers[(_curVideoIdx + 1) % 2].Play();
        }
    }

    public UnityAction OnVideoEnd;
    public UnityAction OnLastVideo;

    private Sequence _sequence;

    private void OnEnable()
    {
        Init();
    }

    private void OnDisable()
    {
        for (int i = 0; i < _videoPlayers.Length; i++)
        {
            _videoPlayers[i].isLooping = false;
            _videoPlayers[i].Stop();
            _videoPlayers[i].loopPointReached -= VideoEnded;
            _videoPlayers[i].clip = null;
            //rawimage 초기화
            RenderTexture rt = (RenderTexture)_videoRawImages[i].texture;
            rt.Release();
            _videoRawImages[i].texture = rt;

            _videoRawImages[i].color = new Color(1f, 1f, 1f, 0f);

            _sequence.Kill();
        }
    }

    private void Init()
    {
        _curVideoIdx = 0;
        for (int i = 0; i < _videoPlayers.Length; i++)
        {
            _videoPlayers[i].loopPointReached += VideoEnded;
        }
        //영상 자동재생
        SetVideoPlayer(0, 1);
    }

    private void SetVideoPlayer(int curIdx, int nextIdx)
    {
        _videoPlayers[curIdx].clip = _videoClips[_curVideoIdx];
        _videoPlayers[curIdx].Prepare();
        _videoPlayers[curIdx].frame = 1;
        _videoPlayers[curIdx].Play();

        _videoPlayers[curIdx].prepareCompleted += IntroVideoPlayer_prepareCompleted;

        Sequence sequence = DOTween.Sequence();
        sequence.Append(_videoRawImages[curIdx].DOFade(1f, fadeRate))
            .Append(_videoRawImages[nextIdx].DOFade(0f, fadeRate))
            .OnComplete(() =>
            {
                _videoPlayers[nextIdx].Pause();
            });
        _sequence = sequence;

        _curVideoIdx++;
    }

    private void IntroVideoPlayer_prepareCompleted(VideoPlayer source)
    {
        source.Play();
    }

    private void VideoEnded(VideoPlayer vp)
    {
        isVideoEnd = true;
        if (OnVideoEnd != null)
        {
            OnVideoEnd.Invoke();
        }
    }

    public void NextVideo()
    {
        if (!isVideoEnd)
        {
            return;
        }
        
        isVideoEnd = false;

        if (videoClips.Length <= _curVideoIdx)
        {
            if(OnLastVideo != null)
                OnLastVideo.Invoke();
            else
            {
                DebugX.Log("마지막 비디오 재생");
            }
        }
        else
        {
            SetVideoPlayer(_curVideoIdx % 2, (_curVideoIdx + 1) % 2);
        }
    }

    public void ClearOutRenderTexture(RenderTexture renderTexture)
    {
        RenderTexture rt = RenderTexture.active;
        RenderTexture.active = renderTexture;
        GL.Clear(true, true, Color.clear);
        RenderTexture.active = rt;
    }
}
