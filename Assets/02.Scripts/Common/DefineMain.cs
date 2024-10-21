using FMODUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public partial class Define
{
    public enum anythingType
    {
        None,
    }
    public enum TutorialType
    {
        None = -1,
        Conversation,
        UnmaskPop,
        Pop,
        ConversationImage,
    }
    public enum MainTutorialType
    {
        //추후 예슬님 스크립트로 이동
        None,
        PlayBtn,
        TrainingBtn,
        OpenAsset1,
        OpenAsset2,
        CollectionPage1,
        CollectionPage2,
        CollectionPage3,
        CollectionPage4,
        LandPage1,
        LandPage2,
        LevelPage,
        MainItemShield,
        MainItemIncreaseHP,
        MainItemHPPotion,
        NextLand,
    }

    public enum ToastMessageType
    {
        None,
        NoCoinTitle,
        NoInternetConnection,
        UnexpectedError,
        BoostUnavailable,
        AdUnavailable,
        FullHeart,
        HeartCharged,
        NoQuaver,
        ReachMaxStage,
        ComingSoon,
        AdCharging1,
        AdCharging2,
        StorageUploadComplete,
        StorageUploadFailed,
        StorageDownloadComplete,
        StorageDownloadFailedNoFile,
        StorageDownloadFailed,
        StorageUploadDateNotAvailable,
        StorageDownloadDateNotAvailable,

        AlreadyFriend,
        AlreadyReceiveFriend,
        ReceiveFriendSuccess,
        RemoveFriend,
        NoExistentUser,
        AcceptFriend,
        NoSearchText,
        CopyToClipboard,
    }

    public enum PopupTitle
    {
        ConfirmRemoveFriend,
    }
}

[Serializable]
public class PlayerInfo
{
    public string id;
    public string pswd;
    public string nickname;
    //public string title; //칭호
    public int level;
    public Image profile;
    public InGameItemInfo[] items = new InGameItemInfo[Enum.GetNames(typeof(Define.GameItem)).Length];

    public void SetInfo(string id, string pswd, string nickname)
    {
        this.id = id;
        this.pswd = pswd;
        this.nickname = nickname;
    }
}

public class MainKey
{
    public const int buttonVibrateMS = 22;
    public const int buttonVibrateAmplitude = 210;
    public const string full = "가득 참";

    //아이템
    public const int shieldUnlockStage = 6;
    public const int maxHealthUnlockStage = 7;
    public const int increaseHPUnlockStage = 8;
    public const int inGameItemPrice = 1900;

    public const int adMaxChargingCnt = 2;
    public const int adChargingCycleMin = 10; //10분에 1번 씩 광고 볼 수 있음
    public const int adDailyLimit = 14; //하루에 14번 까지 볼 수 있음

    public const int tutorialCanvasSortOrder = 200;
    public const float tutorialDescMargin = 40f;

    public const int musicCntEachLand = 8;
}

public class MainTownKey
{
    public const string townQuaverKey = "TQ";
    public const string townLevelKey = "TL";
}

public class OutGameInfo
{
    public const int maxHeartCnt = 5;
    public const int remainHeartSec = 600;
    public int quaverCnt;
    public int coinCnt;
    //다른 재화 추가되면 추가해주세요

    public void SetInfo(int quaverCnt, int coinCnt)
    {
        this.quaverCnt = quaverCnt;
        this.coinCnt = coinCnt;
    }

    public void UseQuaver(int amount)
    {
        quaverCnt -= amount;
        SetInfo(quaverCnt, coinCnt);
    }
}

// 인앱 결제 관련
[Serializable]
public class CoinStoreItem
{
    public string _storeItemID;
    public int _coinCnt;
}

[Serializable]
public class SpecialStoreItem : CoinStoreItem
{
    public bool isSpecialist;

    public int _shieldCnt;
    public int _potionCnt;
    public int _meatCnt;
}

[Serializable]
public class SpecialHeartStoreItem : CoinStoreItem
{
    public int _shieldCnt;
    public int _potionCnt;
    public int _meatCnt;
    public int _infiniteHeartTime;
}

[Serializable]
public class TownInfo
{
    public Define.TownList townType;
    public string townName;
    public Sprite townTitlePanel;
    public UnityEngine.Gradient _effectGradient = new UnityEngine.Gradient() { colorKeys = new GradientColorKey[] { new GradientColorKey(Color.black, 0), new GradientColorKey(Color.white, 1) } };
    public Color shadow;
    public Color outline;
    public Sprite townThumb;
    public QuestData quests;
    public int levelAmount;
    public int assetCnt = 8;
    public AlbumInfo albumInfo;
}

//수집관련
public class MainCollectionKey
{
}

[Serializable]
public class AlbumInfo
{
    [HideInInspector]public string albumName;
    [HideInInspector]public Define.TownList townList;
    public int unlockLevel;
    public Sprite albumSp;
    //음악 배열
    public CollectMusicInfo[] collectMusics;
}

[Serializable]
public class CollectMusicInfo
{
    public int uuid;
    [HideInInspector] public Define.TownList townList;
    public EventReference collectMusic;
    public string musicId;
    public Sprite collectMusicImage;
    public int needQuaver;
}

[Serializable]
public class QuestInfo
{
    public string assetName;
    public Sprite assetSprite;
    public Sprite lockAssetSprite;
    public int unlockStage;
}
[Serializable]
public class LevelInfo
{
    public int level;
    public int score;
    public int completeQuaverCnt;
}
[Serializable]
public class TutorialInfo
{
    public string tutorial_name;
    public string name_id;
    public string sentence_id;
    public Define.TutorialType tutorial_type;
}

[Serializable]
public class TrainingInfo
{
    public Define.InGameTutorialType tutorialType;
    public Sprite sprite;
    public string trainingDesc;
}

public class FriendInfo
{
    public string uid;
    public string name;
    public string email;
}

public partial class EncryptedKey
{
    public const string joinDate = "JDK"; //가입일

    public const string ItemShieldCnt = "SCK"; //보유 중인 아이템 [실드] 수
    public const string ItemMaxHealthCnt = "MHK"; //보유 중인 아이템 [최대체력증가물약] 수
    public const string ItemIncreaseHPCnt = "IHK"; //보유 중인 아이템 [회복수치증가] 수
    public const string adFullChargingTime = "FCT"; //광고 충전 수가 Max 되는 시간(yyyyMMddHHmmss)
    public const string adDailyViewCnt = "ADV"; //일일 전면 광고 시청횟수, [adDailyViewCnt] + yyyyMMdd 로 저장
    public const string adCharingCnt = "ACC"; //광고 충전 수

    //하트
    public const string heartCnt = "HC"; //보유 중인 하트 수
    public const string lastLoginTime = "LLT"; //마지막으로 로그인한 시간(yyyyMMddHHmmss)
    public const string infiniteHeartTime = "IHT"; //무한 하트 기한(yyyyMMddHHmmss)
    public const string isInfiniteHeartTimeMode = "IHM"; //무한 하트타임 여부(T/F)
    public const string remainHeartSec = "RHT"; //하트 충전까지 남은 시간 (초)

    //음표
    public const string getQuaverCnt = "GQK"; //획득한 음표 수(획득 애니메이션 후 보유중인 음표에 합쳐지면서 0으로 초기화)
    public const string remainQuaverCnt = "QK"; //보유한 전체 음표 수(획득한 전체 음표 수에서 사용한 음표 수를 뺌)
    public const string recordQuaverCnt = "RQK"; //획득한 전체 음표 수

    //코인
    public const string coinCnt = "CCK"; //보유 중인 코인 수
    public const string getCoinCnt = "GCK"; //획득한 코인 수(획득 애니메이션 후 보유중인 코인에 합쳐지면서 0으로 초기화)

    //Town & Stage 관련
    public const string maxAsset = "MA"; //마을 내 최대 달성한 에셋 index

    public const string musicCollect = "MCN"; //음악 수집 관련, 조합해서 사용함
    public const string totalMusicCnt = "TMC"; //전체 앨범에서 수집 완료한 음악 수(전체 마을 수)
    public const string totalAlbumCnt = "TAC";
}

public partial class UnencryptedKey
{
    public const string hasPlayed = "HPK"; //수집한 음원이 재생 됐는지 여부(T/F)
    public const string trainingLocked = "TU"; //훈련도감이 열렸는지 여부(T/F)
}