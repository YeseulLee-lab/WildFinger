using System;
using Firebase;
using Firebase.Extensions;
using Firebase.Storage;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Threading;
using UnityEngine;
using System.Threading.Tasks;

/// <summary>
/// 파이어베이스 스토리지에 유저 데이터가 저장된 Json 파일을 업로드하고 다운로드함.
/// </summary>
public class FirebaseStorageManager : MonoBehaviour
{
    private FirebaseStorage storage;
    private StorageReference storageReference;

    private void Start()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Result == DependencyStatus.Available)
            {
                storage = FirebaseStorage.DefaultInstance;
                storageReference = storage.GetReferenceFromUrl(EncryptedKey.firebaseStorageAddress);
            }
            else
            {
                DebugX.LogError($"Could not resolve all Firebase dependencies: {task.Result}");
            }
        });
    }

    /// <summary>
    /// 유저 데이터를 Json 파일로 변환하여 파이어베이스 스토리지 내 UserData 폴더 안에 저장함
    /// </summary>
    /// <param name="userData">Json 형태로 저장할 데이터가 담긴 클래스</param>
    /// <param name="gpgEmailID">저장할 파일 제목(ex: gpgEmailID.json 으로 저장)</param>
    public async void UploadUserData(UserData userData, string gpgEmailID, UnityAction completeAction = null, UnityAction failAction = null)
    {
        if (string.IsNullOrEmpty(gpgEmailID))
        {
            DebugX.Log("GPG 이메일 아이디 NULL");
            gpgEmailID = "test";
        }
        
        string jsonData = JsonHelper.ObjectToJson(userData);
        byte[] jsonBytes = System.Text.Encoding.UTF8.GetBytes(jsonData);

        StorageReference userRef = storageReference.Child("UserData/" + gpgEmailID + ".json");

        await userRef.PutBytesAsync(jsonBytes).ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                failAction?.Invoke();
                DebugX.LogWarning("UploadUserData encountered an error: " + task.Exception);
            }
            else
            {
                completeAction?.Invoke();
                DebugX.Log("UploadUserData completed successfully.");
            }
        });
    }

    /// <summary>
    /// 파이어베이스 스토리지 내 UserData 폴더 안에 저장된 Json파일을 다운로드해서 UserData로 변환해서 리턴.
    /// </summary>
    /// <param name="gpgEmailID">다운로드할 파일 제목(ex: gpgEmailID.json 으로 저장되어 있음)</param>
    /// <returns></returns>
    public async Task<UserData> DownloadUserData(string gpgEmailID, UnityAction completeAction = null, UnityAction failAction = null)
    {
        UserData userData = null;

        if (string.IsNullOrEmpty(gpgEmailID))
        {
            DebugX.Log("GPG 이메일 아이디 NULL");
            gpgEmailID = "test";
        }

        StorageReference userRef = storageReference.Child("UserData/" + gpgEmailID + ".json");

        await userRef.GetBytesAsync(long.MaxValue).ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                failAction?.Invoke();
                DebugX.LogWarning("DownloadUserData encountered an error: " + task.Exception);
            }
            else
            {
                completeAction?.Invoke();
                string jsonData = System.Text.Encoding.UTF8.GetString(task.Result);
                userData = JsonHelper.JsonToObject<UserData>(jsonData);
                DebugX.Log("DownloadUserData completed successfully.");
            }
        });

        return userData;
    }
}
