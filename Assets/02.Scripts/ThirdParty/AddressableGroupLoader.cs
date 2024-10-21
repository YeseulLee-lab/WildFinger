using System;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using Firebase;
using Firebase.Storage;
using Firebase.Extensions;
using UnityEngine.AddressableAssets;
using UnityEngine.Video;
using UnityEngine.ResourceManagement.AsyncOperations;

public class AddressableGroupLoader : MonoBehaviour
{
    private FirebaseStorage storage;
    private StorageReference storageRef;

    #region Unity Life Cycle
    private void Start()
    {
        storage = FirebaseStorage.DefaultInstance;
        storageRef = storage.GetReferenceFromUrl(EncryptedKey.firebaseStorageAddress);
    }
    #endregion

    public void DownloadAndPlayVideo(Define.TownList town, int fileIndex)
    {
        string videoFileName = fileIndex + ".mp4";
        string localFilePath = Path.Combine(Application.persistentDataPath, videoFileName);
        string firebasePath = $"GameData/Town/{((int)town).ToString()}/Video/{videoFileName}";

        DownloadFile(firebasePath, localFilePath, address =>
        {
            Addressables.LoadAssetAsync<VideoClip>(address).Completed += OnVideoLoaded;
        });
    }

    public void DownloadAndShowImage(Define.TownList town, int fileIndex, Action<Texture2D> onImageLoaded)
    {
        string imageFileName = fileIndex + ".png";
        string localFilePath = Path.Combine(Application.persistentDataPath, imageFileName);
        string firebasePath = $"GameData/Town/{((int)town).ToString()}/LastFrame/{imageFileName}";

        DownloadFile(firebasePath, localFilePath, address =>
        {
            Addressables.LoadAssetAsync<Texture2D>(address).Completed += handle =>
            {
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    onImageLoaded?.Invoke(handle.Result);
                }
                else
                {
                    DebugX.LogError("Failed to load image: " + handle.OperationException);
                }
            };
        });
    }

    private void DownloadFile(string firebasePath, string localFilePath, Action<string> onDownloadCompleted)
    {
        storageRef.Child(firebasePath).GetFileAsync(localFilePath).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                DebugX.Log("File downloaded successfully: " + firebasePath);
                string address = "file:///" + localFilePath;
                onDownloadCompleted?.Invoke(address);
            }
            else
            {
                DebugX.LogError("Failed to download file: " + task.Exception);
            }
        });
    }

    private void OnVideoLoaded(AsyncOperationHandle<VideoClip> obj)
    {
        if (obj.Status == AsyncOperationStatus.Succeeded)
        {
            VideoClip videoClip = obj.Result;
            // 동영상 재생 로직 추가 (예: VideoPlayer 컴포넌트 사용)
            VideoPlayer videoPlayer = GetComponent<VideoPlayer>();
            if (videoPlayer != null)
            {
                //videoPlayer.clip = videoClip;
                //videoPlayer.Play();
                //TODO: 비디오를 play하지 않고 특정 UnityAction을 수행할 수 있도록 수정
            }
            else
            {
                DebugX.LogError("VideoPlayer component is missing.");
            }
        }
        else
        {
            DebugX.LogError("Failed to load video: " + obj.OperationException);
        }
    }
}
