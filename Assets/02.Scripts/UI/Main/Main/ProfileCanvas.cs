using HHK.UIEC;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ProfileCanvas : BaseMainCanvas
{
    [Header("---------------- Profile Canvas Area ----------------")]
    [SerializeField]
    private Button _friendBtn;
    [SerializeField]
    private UnityEngine.UI.Text nickNameText;
    [SerializeField]
    private UnityEngine.UI.Text levelText;
    [SerializeField]
    private UnityEngine.UI.Text joinDate;
    [SerializeField]
    private Image profileImage;
    [SerializeField]
    private int _maxTextLength;

    [Header("---------------- Record ----------------")]
    [SerializeField]
    private UnityEngine.UI.Text oneShot;
    [SerializeField]
    private UnityEngine.UI.Text quaverCnt;
    [SerializeField]
    private UnityEngine.UI.Text fullComboCnt;
    [SerializeField]
    private UnityEngine.UI.Text savedTown;
    [SerializeField]
    private UnityEngine.UI.Text albumCnt;
    [SerializeField]
    private UnityEngine.UI.Text collectedMusicCnt;

    public override void Start()
    {
        base.Start();
        _friendBtn.onClick.AddListener(OnClickFriend);
    }

    private void OnClickFriend()
    {
        MainUIManager.Instance.friendCanvas.ShowCanvas();
    }

    #region SetData
    public override void ShowCanvas()
    {
        SetProfileData();
        base.ShowCanvas();
    }

    private void SetProfileData()
    {
        nickNameText.text = Social.localUser.userName;
        if (nickNameText.preferredWidth > nickNameText.transform.parent.GetComponent<RectTransform>().rect.width)
        {
            nickNameText.text = Social.localUser.userName.Substring(0, _maxTextLength) + "...";
        }
        else
        {
            nickNameText.text = Social.localUser.userName;
        }

        StartCoroutine(UserPictureLoad());
        joinDate.text = GamePlayData.Instance.joinDate.Year + "." + GamePlayData.Instance.joinDate.Month;

        levelText.text = GamePlayData.Instance.maxStage.ToString();

        //기록
        int saveTownNum = 0;
        for (int i = 0; i < (int)GamePlayData.Instance.maxTown; i++)
        {
            saveTownNum ++;
        }
        savedTown.text = saveTownNum.ToString();
        oneShot.text = GamePlayData.Instance.isSuccessfulOnFirstTryCnt.ToString();

        int fullComboCnt = 0;
        for (int i = 0; i < GamePlayData.Instance.maxStage; i++)
        {
            //풀콤보 횟수
            fullComboCnt += PlayerPrefs.GetInt(EncryptedKey.isAllPerfect + i);
            this.fullComboCnt.text = fullComboCnt.ToString();
            //한번에 성공 횟수
        }
        
        quaverCnt.text = GamePlayData.Instance.recordQuaverCnt.ToString();
        albumCnt.text = GamePlayData.Instance.totalAlbumCnt.ToString();
        collectedMusicCnt.text = GamePlayData.Instance.totalMusicCnt.ToString();
    }

    IEnumerator UserPictureLoad()
    {
        Texture2D tex = Social.localUser.image;

        while (tex == null)
        {
            tex = Social.localUser.image;
            yield return null;
        }
        Rect rect = new Rect(0, 0, tex.width, tex.height);
        profileImage.sprite = Sprite.Create(tex, rect, new Vector2(0.5f, 0.5f));
    }
    #endregion
}
