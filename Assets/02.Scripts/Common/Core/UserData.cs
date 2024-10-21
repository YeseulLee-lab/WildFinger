using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Json 파일로 변환하여 저장할 예정
/// </summary>
public class UserData
{
    //현재 PlayerPref에 있는 데이터 중 백업이 필요한 부분을 저장함
    //코인, 하트, 보유 아이템 수 등 재화 복사의 가능성이 있는 데이터는 저장하지 않음
    public bool[] inGameTutorialsTypeUsed;//None 제외
    public bool[] mainTutorialsTypeUsed;//None이 0임
    public bool isSFXOn;
    public bool isBGMOn;
    public bool isVibOn;
    public bool isNotiOn;
    //public int recordQuaverCnt;
    public int remainQuaverCnt; //TODO: Used Quaver Cnt 로 바뀔 예정
    //public Define.TownList maxTown;
    //public int maxLevel; //최대 달성 레벨(플레이 해야하는 레벨)
    public int maxAsset; //마을 내 최대 달성 에셋 Index
    //public bool[] isAllPerfectLevels; //해당 레벨 올퍼펙트 여부(T/F) //MaxLevel 개로 초기화
    //public int[] scores; //해당 레벨 클리어 점수(음표 수 1~3개) //MaxLevel 개로 초기화
    public int[] townMusicCollectCnts; //마을 별 완성한 수집 음악 수(최대 8개) //None, TBC 제외
    public int totalMusicCnt; //전체 마을 음악에서 수집 완료한 음악 수
    public bool[] isMusicHasPlayed; //수집한 음원이 재생 됐는지 여부(T/F) //각 마을별 음원 수 * 마을 수, 2차원 배열을 1차원으로 변환해야 함
    public bool[] isMusicCollected; //해당 음원을 수집했는지 여부(T/F) //각 마을별 음원 수 * 마을 수, 2차원 배열을 1차원으로 변환해야 함
    public string recentDataUploadTime; //유저 데이터를 업로드한 최근 시간(yyyyMMddHHmmss)
    public string recentDataDownloadTime; //유저 데이터를 다운로드한 최근 시간(yyyyMMddHHmmss)
    public int isSuccessfulOnFirstTryCnt; //한 번에 레벨 성공 Cnt

    #region Get Data
    public static bool[] GetInGameTutorialsType(int cnt)
    {
        bool[] types = new bool[cnt];

        for(int i=0; i< cnt; i++)
        {
            types[i] = PlayerPrefs.GetInt("Tut" + ((Define.InGameTutorialType)i).ToString()) == 1;
        }

        return types;
    }

    public static bool[] GetMainTutorialsType(int cnt)
    {
        bool[] types = new bool[cnt];

        for (int i = 0; i < cnt; i++)
        {
            types[i] = PlayerPrefs.GetInt("Tut" + ((Define.MainTutorialType)i).ToString()) == 1;
        }

        return types;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="cnt">Max Level - 1</param>
    /// <returns></returns>
    public static bool[] GetIsAllPerfectLevels(int cnt)
    {
        bool[] infos = new bool[cnt + 1];

        for(int i = 0; i <= cnt; i++)
        {
            infos[i] = PlayerPrefs.GetInt(EncryptedKey.isAllPerfect + i.ToString()) == 1;
        }

        return infos;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="cnt">Max Level - 1</param>
    /// <returns></returns>
    public static int[] GetScores(int cnt)
    {
        int[] infos = new int[cnt + 1];

        for (int i = 0; i <= cnt; i++)
        {
            infos[i] = PlayerPrefs.GetInt(EncryptedKey.score + i.ToString());
        }

        return infos;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="cnt">마을 수</param>
    /// <returns></returns>
    public static int[] GetTownMusicCollectCnts(int cnt)
    {
        int[] infos = new int[cnt];

        for (int i = 0; i < cnt; i++)
        {
            infos[i] = PlayerPrefs.GetInt(EncryptedKey.musicCollect + ((Define.TownList)i).ToString());
        }

        return infos;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="cnt">2차원 배열을 1차원에 저장, music ID</param>
    /// <param name="townCnt">2차원 배열을 1차원에 저장, 마을 수</param>
    /// <returns></returns>
    public static bool[] GetIsMusicHasPlayed(int musicCnt, int townCnt)
    {
        int cnt = musicCnt * townCnt;
        bool[] infos = new bool[cnt];
        int curTown = 0;
        int curUuid = 0;

        for (int i = 0; i < cnt; i++)
        {
            infos[i] = PlayerPrefs.GetInt(EncryptedKey.musicCollect + ((Define.TownList)curTown).ToString() + (curUuid++).ToString() + UnencryptedKey.hasPlayed) == 1;

            if(curUuid > musicCnt)
            {
                curUuid = 0;
                curTown++;
            }
        }

        return infos;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="musicCnt">2차원 배열을 1차원에 저장, music ID</param>
    /// <param name="townCnt">2차원 배열을 1차원에 저장, 마을 수</param>
    /// <returns></returns>
    public static bool[] GetIsMusicCollected(int musicCnt, int townCnt)
    {
        int cnt = musicCnt * townCnt;
        bool[] infos = new bool[cnt];
        int curTown = 0;
        int curUuid = 0;

        for (int i = 0; i < cnt; i++)
        {
            infos[i] = PlayerPrefs.GetInt(EncryptedKey.musicCollect + ((Define.TownList)curTown).ToString() + (curUuid++).ToString()) == 1;

            if (curUuid > musicCnt)
            {
                curUuid = 0;
                curTown++;
            }
        }

        return infos;
    }

    public static UserData GetUserData()
    {
        UserData data = new UserData();
        if (GamePlayData.Instance == null)
        {
            DebugX.Log("GamePlayData.Instance == null 유저 데이터 업로드 불가능");
            return data;
        }

        data.inGameTutorialsTypeUsed = GetInGameTutorialsType(Enum.GetNames(typeof(Define.InGameTutorialType)).Length - 1);
        data.mainTutorialsTypeUsed = GetMainTutorialsType(Enum.GetNames(typeof(Define.MainTutorialType)).Length);
        data.isSFXOn = GamePlayData.Instance.isCommonSFXOn;
        data.isBGMOn = GamePlayData.Instance.isCommonBGMOn;
        data.isVibOn = GamePlayData.Instance.mobileVibrater.isOn;
        data.isNotiOn = GamePlayData.Instance.isNotiOn;
        //data.recordQuaverCnt = GamePlayData.Instance.recordQuaverCnt;
        data.remainQuaverCnt = GamePlayData.Instance.remainQuaverCnt;
        //data.maxTown = GamePlayData.Instance.maxTown;
        //data.maxLevel = GamePlayData.Instance.maxStage;
        data.maxAsset = GamePlayData.Instance.maxAssetIdx;
        //data.isAllPerfectLevels = GetIsAllPerfectLevels(data.maxLevel - 1);
        //data.scores = GetScores(data.maxLevel - 1);
        data.townMusicCollectCnts = GetTownMusicCollectCnts(Enum.GetNames(typeof(Define.TownList)).Length - 2);
        data.totalMusicCnt = GamePlayData.Instance.totalMusicCnt;
        data.isMusicHasPlayed = GetIsMusicHasPlayed(MainKey.musicCntEachLand, Enum.GetNames(typeof(Define.TownList)).Length - 2);
        data.isMusicCollected = GetIsMusicCollected(MainKey.musicCntEachLand, Enum.GetNames(typeof(Define.TownList)).Length - 2);
        GamePlayData.Instance.recentDataUploadTime = DateTime.Now;
        data.recentDataUploadTime = GamePlayData.Instance.recentDataUploadTime.ToString("yyyyMMddHHmmss");
        data.isSuccessfulOnFirstTryCnt = GamePlayData.Instance.isSuccessfulOnFirstTryCnt;

        return data;
    }
    #endregion

    #region Set Data
    public static void SetInGameTutorialsType(bool[] infos)
    {
        for (int i = 0; i < infos.Length; i++)
        {
            PlayerPrefs.SetInt("Tut" + ((Define.InGameTutorialType)i).ToString(), infos[i] == true? 1 : 0);
        }
    }

    public static void SetMainTutorialsType(bool[] infos)
    {
        for (int i = 0; i < infos.Length; i++)
        {
            PlayerPrefs.SetInt("Tut" + ((Define.MainTutorialType)i).ToString(), infos[i] == true ? 1 : 0);
        }
    }

    public static void SetIsAllPerfectLevels(bool[] infos)
    {
        for (int i = 0; i < infos.Length; i++)
        {
            PlayerPrefs.SetInt(EncryptedKey.isAllPerfect + i.ToString(), infos[i] == true ? 1 : 0);
        }
    }

    public static void SetScores(int[] infos)
    {
        for (int i = 0; i < infos.Length; i++)
        {
            PlayerPrefs.SetInt(EncryptedKey.score + i.ToString(), infos[i]);
        }
    }

    public static int[] SetTownMusicCollectCnts(int[] infos)
    {
        for (int i = 0; i < infos.Length; i++)
        {
            PlayerPrefs.SetInt(EncryptedKey.musicCollect + ((Define.TownList)i).ToString(), infos[i]);
        }

        return infos;
    }

    public static void SetIsMusicHasPlayed(bool[] infos, int musicCnt)
    {
        int curTown = 0;
        int curUuid = 0;

        for (int i = 0; i < infos.Length; i++)
        {
            PlayerPrefs.SetInt(EncryptedKey.musicCollect + ((Define.TownList)curTown).ToString() + (curUuid++).ToString() + UnencryptedKey.hasPlayed, infos[i] == true? 1:0);

            if (curUuid > musicCnt)
            {
                curUuid = 0;
                curTown++;
            }
        }
    }

    public static void SetIsMusicCollected(bool[] infos, int musicCnt)
    {
        int curTown = 0;
        int curUuid = 0;

        for (int i = 0; i < infos.Length; i++)
        {
            PlayerPrefs.SetInt(EncryptedKey.musicCollect + ((Define.TownList)curTown).ToString() + (curUuid++).ToString(), infos[i] == true ? 1 : 0);

            if (curUuid > musicCnt)
            {
                curUuid = 0;
                curTown++;
            }
        }
    }

    public static void SetUserData(UserData data, UnityAction completeAction = null)
    {
        if (GamePlayData.Instance == null)
        {
            DebugX.Log("GamePlayData.Instance == null 유저 데이터 저장 불가능");
            return;
        }

        //TODO: UserData 저장
        SetInGameTutorialsType(data.inGameTutorialsTypeUsed);
        SetMainTutorialsType(data.mainTutorialsTypeUsed);
        GamePlayData.Instance.isCommonSFXOn = data.isSFXOn;
        GamePlayData.Instance.isCommonBGMOn = data.isBGMOn;
        GamePlayData.Instance.mobileVibrater.isOn = data.isVibOn;
        GamePlayData.Instance.isNotiOn = data.isNotiOn;
        //GamePlayData.Instance.recordQuaverCnt = data.recordQuaverCnt;
        GamePlayData.Instance.remainQuaverCnt = data.remainQuaverCnt;
        //GamePlayData.Instance.maxTown = data.maxTown;
        //GamePlayData.Instance.maxStage = data.maxLevel;
        GamePlayData.Instance.maxAssetIdx = data.maxAsset;
        //SetIsAllPerfectLevels(data.isAllPerfectLevels);
        //SetScores(data.scores);
        SetTownMusicCollectCnts(data.townMusicCollectCnts);
        GamePlayData.Instance.totalMusicCnt = data.totalMusicCnt;
        SetIsMusicHasPlayed(data.isMusicHasPlayed, MainKey.musicCntEachLand);
        SetIsMusicCollected(data.isMusicCollected, MainKey.musicCntEachLand);
        GamePlayData.Instance.recentDataDownloadTime = DateTime.Now;
        GamePlayData.Instance.recentDataUploadTime = DateTime.ParseExact(data.recentDataUploadTime, "yyyyMMddHHmmss", null);
        GamePlayData.Instance.isSuccessfulOnFirstTryCnt = data.isSuccessfulOnFirstTryCnt;
        completeAction?.Invoke();
    }
    #endregion
}