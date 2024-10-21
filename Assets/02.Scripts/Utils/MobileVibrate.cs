using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Threading;
using Cysharp.Threading.Tasks;

public class MobileVibrate : MonoBehaviour
{
    private bool _isOn;
    public bool isOn { get { return _isOn;  } set {
            _isOn = value;
            PlayerPrefs.SetInt(UnencryptedKey.isVibOn, value ? 1 : 0);
        } }

#if UNITY_IOS && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void CustomHaptic(float intensity, float sharpness, float duration);

#endif

    private CancellationTokenSource _cts;
    private CancellationToken _ct;
    private const int _duplicatedVibrationDelayMS = 150;

    public async void Vibrates(int times = 2, long milliseconds = InGameKey.defaultVibrateMS, int amplitude = InGameKey.defaultVibrateAmplitude)
    {
        if (!isOn)
        {
            return;
        }

#if UNITY_ANDROID && !UNITY_EDITOR
        _cts = new CancellationTokenSource();
        _ct = _cts.Token;
        
        using (var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            var currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            var vibrator = currentActivity.Call<AndroidJavaObject>("getSystemService", "vibrator");

            for(int i = 0; i<times; i++)
            {
                if (IsAndroidVersionOOrAbove())
                {
                    var vibrationEffectClass = new AndroidJavaClass("android.os.VibrationEffect");
                    var createOneShotMethod = vibrationEffectClass.CallStatic<AndroidJavaObject>("createOneShot", milliseconds, amplitude);
                    vibrator.Call("vibrate", createOneShotMethod);
                }
                else
                {
                    vibrator.Call("vibrate", milliseconds);
                }

                try
                {
                    await UniTask.Delay(_duplicatedVibrationDelayMS, cancellationToken: _ct);
                }
                catch (OperationCanceledException)
                {
                   
                }
            }
        }
#elif UNITY_IOS
        DebugX.Log("IOS 미구현");
#endif
    }

    /// <summary>
    /// 진동 재생
    /// </summary>
    /// <param name="milliseconds">밀리초(1000 이 1초)</param>
    /// <param name="amplitude">진동 세기, 0~255 사이</param>
    public void Vibrate(long milliseconds = InGameKey.defaultVibrateMS, int amplitude = InGameKey.defaultVibrateAmplitude)
    {
        if (!isOn)
        {
            return;
        }

#if UNITY_ANDROID && !UNITY_EDITOR
        using (var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            var currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            var vibrator = currentActivity.Call<AndroidJavaObject>("getSystemService", "vibrator");
            if (IsAndroidVersionOOrAbove())
            {
                var vibrationEffectClass = new AndroidJavaClass("android.os.VibrationEffect");
                var createOneShotMethod = vibrationEffectClass.CallStatic<AndroidJavaObject>("createOneShot", milliseconds, amplitude);
                vibrator.Call("vibrate", createOneShotMethod);
            }
            else
            {
                vibrator.Call("vibrate", milliseconds);
            }
        }
#elif UNITY_IOS
       //intensity, sharpness, duration
        CustomHaptic(1.0f, 1.0f, 1.0f);
#endif
    }


    private static bool IsAndroidVersionOOrAbove()
    {
        using (var version = new AndroidJavaClass("android.os.Build$VERSION"))
        {
            return version.GetStatic<int>("SDK_INT") >= 26; // Android 8.0 (API level 26)
        }
    }

}
