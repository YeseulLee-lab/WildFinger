using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine.UI;
using UnityEngine.Events;
using HHK.UIEC;

public class GPGSSetup : MonoBehaviour
{
    private void Start()
    {
        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder().EnableSavedGames().Build();
        PlayGamesPlatform.InitializeInstance(config);
        PlayGamesPlatform.DebugLogEnabled = true;
        PlayGamesPlatform.Activate();
    }

#if UNITY_ANDROID
    public void GoogleLogin(UnityAction onSuccess)
    {
        if (Social.localUser.authenticated)
        {
            GamePlayData.Instance.uid = Social.localUser.id;
            Debug.Log("Social.localUser.id: " + Social.localUser.id);
        }
        else
        {
            Social.localUser.Authenticate((bool success) =>
            {
                if (success)
                {
                    Debug.Log("GPGS 로그인 성공");
                    GamePlayData.Instance.uid = Social.localUser.id;
                    onSuccess.Invoke();
                }
                else
                {
                    Debug.Log("GPGS 로그인 실패");
                    return;
                }
            });
        }
    }
#endif
}
