using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HHK.UIEC;
using FMODUnity;
using FMOD.Studio;
using System;
using UnityEngine.Video;
using GooglePlayGames;

public class LoginCanvas : MonoBehaviour
{
    [SerializeField]
    private Image loginPanel;
    [SerializeField]
    private Button _loginBtn;
    [SerializeField]
    private EventReference _loginBGM;
    private static EventInstance _loginInstance;
    [SerializeField]
    private UIECAnimator[] _anims;
    [SerializeField]
    private UnityEngine.UI.Text _versionText;

    [Header("------------- Intro ----------------")]
    [SerializeField]
    private GameObject introPanel;

    private void Awake()
    {
        _loginInstance = RuntimeManager.CreateInstance(_loginBGM);
    }

    private void Start()
    {
        if (GamePlayData.Instance.maxAssetIdx == 0)
        {
            loginPanel.sprite = GamePlayData.Instance.maxTownInfo.quests.videoFirstFrame;
        }
        else
        {
            loginPanel.sprite = GamePlayData.Instance.maxTownInfo.quests.videoLastFrames[GamePlayData.Instance.maxAssetIdx - 1];
        }

        _loginInstance.start();
        _loginBtn?.onClick.AddListener(OnClickLoginBtn);
        _versionText.text = Application.version + " Version";
        StartCoroutine(RandomDelayCoroutine());

        //Main 볼륨 세팅
        if (GamePlayData.Instance != null)
        {
            _loginInstance.setVolume(GamePlayData.Instance.isCommonBGMOn ? 1f : 0f);
        }
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
        _loginInstance.setUserData(IntPtr.Zero);
        _loginInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        _loginInstance.release();

        _loginBtn = null;
    }

    private void OnClickLoginBtn()
    {
        if (GamePlayData.Instance != null)
        {
            GamePlayData.Instance.OnClickBtnEffect();
            GamePlayData.Instance.ShowLoading();
        }

        //인터넷 연결 없을 시 접속 불가능
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            GamePlayData.Instance.toastPopup.ShowToastMessage(Define.ToastMessageType.NoInternetConnection, true);
            return;
        }

#if UNITY_EDITOR
        if (GamePlayData.Instance.isFirst == 0)
        {
            //최초접속
            GamePlayData.Instance.isFirst = 1;
            GamePlayData.Instance.GetFireStoreData();
        }
        SceneSwitcher.Instance.SwitchScene(Define.SceneName.Main);
#elif UNITY_ANDROID
        GetComponent<GPGSSetup>().GoogleLogin(() =>
        {
            if (GamePlayData.Instance.isFirst == 0)
            {
                //최초접속
                GamePlayData.Instance.isFirst = 1;
                GamePlayData.Instance.GetFireStoreData();
            }
            SceneSwitcher.Instance.SwitchScene(Define.SceneName.Main);
        });
#endif
    }

    private IEnumerator RandomDelayCoroutine()
    {
        int index = 0;
        while (true)
        {
            float randomDelay = UnityEngine.Random.Range(0.2f, 0.8f);
            yield return new WaitForSeconds(randomDelay);
            _anims[index++].OnCustomChannel();

            if(index >= _anims.Length)
            {
                index = 0;
            }
        }
    }
}
