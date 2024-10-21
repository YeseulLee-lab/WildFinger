using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Firestore;
using Firebase.Extensions;
using System;
using System.Linq;
using UnityEngine.Events;

public class FirestoreManager : MonoBehaviour
{
    public static FirestoreManager Instance { get; private set; }

    FirebaseFirestore db;

    #region Unity Life Cycle
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this);
        }
    }

    private void OnDestroy()
    {
        Instance = null;
    }

    void Start()
    {
        db = FirebaseFirestore.DefaultInstance;
    }
    #endregion

    public void GetFriendList(UnityAction<string[]> endAction)
    {

        DocumentReference userRef = db.Collection("users").Document(GamePlayData.Instance.uid);
        userRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            DocumentSnapshot snapshot = task.Result;

            DebugX.Log(String.Format("User: {0}", snapshot.Id));
            Dictionary<string, object> documentDictionary = snapshot.ToDictionary();

            string[] refArr = ((IEnumerable)documentDictionary["Friends"]).Cast<object>()
                             .Select(x => x.ToString())
                             .ToArray();

            DebugX.Log("Read all data from the users collection.");
            endAction?.Invoke(refArr);
        });
    }

    public void SearchFriend(string userId, UnityAction<FriendInfo> endAction = null, UnityAction error = null)
    {
        DocumentReference userRef = db.Collection("users").Document(userId);
        userRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            DocumentSnapshot snapshot = task.Result;
            if (snapshot.Exists)
            {
                DebugX.Log(String.Format("User: {0}", snapshot.Id));
                Dictionary<string, object> documentDictionary = snapshot.ToDictionary();

                FriendInfo info = new FriendInfo
                {
                    name = documentDictionary["Name"].ToString(),
                    uid = userId,
                    email = documentDictionary["Email"].ToString(),
                };
                endAction?.Invoke(info);

                DebugX.Log("Read all data from the users collection.");
            }
            else
            {
                error?.Invoke();
            }
        });
    }

    public void SendFriendRequest(string userId, UnityAction endAction)
    {
        GetFriendList((arr) =>
        {
            foreach (string friendId in arr)
            {
                if (friendId.Equals(userId))
                {
                    GamePlayData.Instance.toastPopup.ShowToastMessage(Define.ToastMessageType.AlreadyFriend);
                    DebugX.Log("이미 친구 목록에 있습니다.");
                    endAction?.Invoke();
                    return;
                }
            }

            //친구 목록에없으면
            DocumentReference userRef = db.Collection("users").Document(userId);
            userRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
            {
                DocumentSnapshot snapshot = task.Result;

                DebugX.Log(String.Format("User: {0}", snapshot.Id));
                Dictionary<string, object> documentDictionary = snapshot.ToDictionary();

                string[] refArr = ((IEnumerable)documentDictionary["Receive"]).Cast<object>()
                                 .Select(x => x.ToString())
                                 .ToArray();

                foreach (string str in refArr)
                {
                    if (str.Equals(GamePlayData.Instance.uid))
                    {
                        GamePlayData.Instance.toastPopup.ShowToastMessage(Define.ToastMessageType.AlreadyReceiveFriend);
                        DebugX.Log("이미 친구 신청을 보냈습니다.");
                        endAction?.Invoke();
                        return;
                    }
                }

                //이미 있는 친구 목록에 내 신청 더하기
                string[] finalArr = new string[refArr.Length + 1];
                for (int i = 0; i < refArr.Length; i++)
                {
                    finalArr[i] = refArr[i];
                }
                finalArr[finalArr.Length - 1] = GamePlayData.Instance.uid;

                //상대가 받은 신청
                DocumentReference docRef = db.Collection("users").Document(userId);
                Dictionary<string, object> update = new Dictionary<string, object>
                {
                        { "Receive", finalArr }
                };
                docRef.SetAsync(update, SetOptions.MergeAll);

                endAction?.Invoke();
                GamePlayData.Instance.toastPopup.ShowToastMessage(Define.ToastMessageType.ReceiveFriendSuccess);
            });
        });
    }

    public void AcceptRequest(string userId, UnityAction<string[]> endAction)
    {
        AddFriend(GamePlayData.Instance.uid, userId, endAction);
        AddFriend(userId, GamePlayData.Instance.uid, null);
    }

    //나랑 상대 친구 목록에 서로 넣어주기
    private void AddFriend(string targetDocId, string addUserId, UnityAction<string[]> endAction)
    {
        DocumentReference userRef = db.Collection("users").Document(targetDocId);
        userRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            DocumentSnapshot snapshot = task.Result;

            DebugX.Log(String.Format("User: {0}", snapshot.Id));
            Dictionary<string, object> documentDictionary = snapshot.ToDictionary();

            string[] refArr = ((IEnumerable)documentDictionary["Friends"]).Cast<object>()
                             .Select(x => x.ToString())
                             .ToArray();

            //이미 있는 내 친구 목록에 수락한 사람 더하기
            string[] finalArr = new string[refArr.Length + 1];
            for (int i = 0; i < refArr.Length; i++)
            {
                finalArr[i] = refArr[i];
            }
            finalArr[finalArr.Length - 1] = addUserId;

            //내가 보낸 신청
            DocumentReference myDocRef = db.Collection("users").Document(targetDocId);
            Dictionary<string, object> myUpdate = new Dictionary<string, object>
            {
                    { "Friends", finalArr }
            };
            myDocRef.SetAsync(myUpdate, SetOptions.MergeAll);

            endAction?.Invoke(finalArr);
        });
    }

    public void RemoveFriend(string userId, UnityAction refresh = null, string dicKey = "Friends")
    {
        #region My List
        DocumentReference userRef = db.Collection("users").Document(GamePlayData.Instance.uid);
        userRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            DocumentSnapshot snapshot = task.Result;

            DebugX.Log(String.Format("User: {0}", snapshot.Id));
            Dictionary<string, object> documentDictionary = snapshot.ToDictionary();

            string[] refArr = ((IEnumerable)documentDictionary[dicKey]).Cast<object>()
                             .Select(x => x.ToString())
                             .ToArray();

            List<string> finalArr = new List<string>();
            for (int i = 0; i < refArr.Length; i++)
            {
                if (refArr[i] != userId)
                    finalArr.Add(refArr[i]);
            }

            DocumentReference docRef = db.Collection("users").Document(GamePlayData.Instance.uid);
            Dictionary<string, object> updateDic = new Dictionary<string, object>
            {
                    { dicKey, finalArr.ToArray() }
            };
            docRef.SetAsync(updateDic, SetOptions.MergeAll);

            refresh.Invoke();
        });
        #endregion

        #region Other List
        DocumentReference otherRef = db.Collection("users").Document(userId);
        otherRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            DocumentSnapshot snapshot = task.Result;

            DebugX.Log(String.Format("User: {0}", snapshot.Id));
            Dictionary<string, object> documentDictionary = snapshot.ToDictionary();

            string[] refArr = ((IEnumerable)documentDictionary[dicKey]).Cast<object>()
                             .Select(x => x.ToString())
                             .ToArray();

            List<string> finalArr = new List<string>();
            for (int i = 0; i < refArr.Length; i++)
            {
                if (refArr[i] != GamePlayData.Instance.uid)
                    finalArr.Add(refArr[i]);
            }

            DocumentReference docRef = db.Collection("users").Document(userId);
            Dictionary<string, object> updateDic = new Dictionary<string, object>
            {
                    { dicKey, finalArr.ToArray() }
            };
            docRef.SetAsync(updateDic, SetOptions.MergeAll);

            refresh.Invoke();
        });
        #endregion
    }

    public void GetReceivedRequestList(UnityAction<string[]> endAction)
    {
        DocumentReference userRef = db.Collection("users").Document(GamePlayData.Instance.uid);
        userRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            DocumentSnapshot snapshot = task.Result;

            DebugX.Log(String.Format("User: {0}", snapshot.Id));
            Dictionary<string, object> documentDictionary = snapshot.ToDictionary();

            string[] refArr = ((IEnumerable)documentDictionary["Receive"]).Cast<object>()
                             .Select(x => x.ToString())
                             .ToArray();

            DebugX.Log("Read all data from the users collection.");
            endAction?.Invoke(refArr);
        });
    }

    public void CheckUserExist(UnityAction success, UnityAction error)
    {
        DocumentReference userRef = db.Collection("users").Document(GamePlayData.Instance.uid);
        userRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Result.ToDictionary() != null)
            {
                Debug.Log("저장 데이터 있음");
                success.Invoke();
            }
            else
            {
                error.Invoke();
            }
        });
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="userId">gpgs id</param>
    /// <param name="userName">gpgs에서 받아오는 닉네임</param>
    public void CreateUser(string userId, string userName, string email)
    {
        DocumentReference userRef = db.Collection("users").Document(userId);
        userRef = db.Collection("users").Document(userId);
        Dictionary<string, object> user = new Dictionary<string, object>
            {
                { "CreatedDate", DateTime.Today },
                { "Name",  userName },
                { "Email",  email },
                { "Friends", new int[0] },
                { "Receive", new int[0] },
                { "CoinCnt",  0 },
                { "QuaverCnt", 0 },
                { "MaxStage", 1 },
                { "Scores", new int[0] },
                { "AllPerfects", new bool[0] },
                { "Item", new int[3] },
            };

        userRef.SetAsync(user).ContinueWithOnMainThread(task =>
        {
            GetComponent<GamePlayData>().joinDate = (DateTime)user["CreatedDate"];
            DebugX.Log("Added data to the " + userId + " document in the users collection.");
        });
    }

    #region Get Data
    public void GetCoinCnt(UnityAction<int> success)
    {
        DocumentReference userRef = db.Collection("users").Document(GamePlayData.Instance.uid);
        userRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            DocumentSnapshot snapshot = task.Result;
            Dictionary<string, object> documentDictionary = snapshot.ToDictionary();
            success?.Invoke(Int32.Parse(documentDictionary["CoinCnt"].ToString()));
        });
    }

    public void GetQuaverCnt(UnityAction<int> success)
    {
        DocumentReference userRef = db.Collection("users").Document(GamePlayData.Instance.uid);
        userRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            DocumentSnapshot snapshot = task.Result;
            Dictionary<string, object> documentDictionary = snapshot.ToDictionary();
            success?.Invoke(Int32.Parse(documentDictionary["QuaverCnt"].ToString()));
        });
    }

    public void GetMaxStage(string uid, UnityAction<int> success)
    {
        DocumentReference userRef = db.Collection("users").Document(uid);
        userRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            DocumentSnapshot snapshot = task.Result;
            Dictionary<string, object> documentDictionary = snapshot.ToDictionary();
            success?.Invoke(Int32.Parse(documentDictionary["MaxStage"].ToString()));
        });
    }

    public void GetScores(UnityAction<List<int>> success)
    {
        DocumentReference userRef = db.Collection("users").Document(GamePlayData.Instance.uid);
        userRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            DocumentSnapshot snapshot = task.Result;
            Dictionary<string, object> documentDictionary = snapshot.ToDictionary();
            int[] arr = ((IEnumerable)documentDictionary["Scores"]).Cast<object>()
                             .Select(x => Int32.Parse(x.ToString()))
                             .ToArray();
            success?.Invoke(arr.ToList());
        });
    }

    public void GetIsAllPerfectLevels(UnityAction<List<bool>> success)
    {
        DocumentReference userRef = db.Collection("users").Document(GamePlayData.Instance.uid);
        userRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            DocumentSnapshot snapshot = task.Result;
            Dictionary<string, object> documentDictionary = snapshot.ToDictionary();
            bool[] arr = ((IEnumerable)documentDictionary["AllPerfects"]).Cast<object>()
                             .Select(x => bool.Parse(x.ToString()))
                             .ToArray();
            success?.Invoke(arr.ToList());
        });
    }

    public void GetItemsCnt(UnityAction<List<int>> success)
    {
        DocumentReference userRef = db.Collection("users").Document(GamePlayData.Instance.uid);
        userRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            DocumentSnapshot snapshot = task.Result;
            Dictionary<string, object> documentDictionary = snapshot.ToDictionary();
            int[] arr = ((IEnumerable)documentDictionary["CreatedDate"]).Cast<object>()
                             .Select(x => Int32.Parse(x.ToString()))
                             .ToArray();
            success?.Invoke(arr.ToList());
        });
    }

    public void GetJoinDate(UnityAction<DateTime> success)
    {
        DocumentReference userRef = db.Collection("users").Document(GamePlayData.Instance.uid);
        userRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            DocumentSnapshot snapshot = task.Result;
            Dictionary<string, object> documentDictionary = snapshot.ToDictionary();
            Debug.Log(documentDictionary["CreatedDate"].ToString());
            DateTime result = DateTime.ParseExact(documentDictionary["CreatedDate"].ToString().Replace("Timestamp:", "").Trim(), "yyyy-MM-ddTHH:mm:ssZ", null);
            success?.Invoke(result);
        });
    }
    #endregion

    #region Set Data
    public void SetCoinCnt(int coinCnt)
    {
        Dictionary<string, object> documentDictionary = new Dictionary<string, object>();
        documentDictionary.Add("CoinCnt", coinCnt);
        DocumentReference userRef = db.Collection("users").Document(GamePlayData.Instance.uid);
        userRef.SetAsync(documentDictionary, SetOptions.MergeAll);
    }

    public void SetQuaverCnt(int quaverCnt)
    {
        Dictionary<string, object> documentDictionary = new Dictionary<string, object>();
        documentDictionary.Add("QuaverCnt", quaverCnt);
        DocumentReference userRef = db.Collection("users").Document(GamePlayData.Instance.uid);
        userRef.SetAsync(documentDictionary, SetOptions.MergeAll);
    }

    public void SetMaxStage(int stage)
    {
        Dictionary<string, object> documentDictionary = new Dictionary<string, object>();
        documentDictionary.Add("MaxStage", stage);
        DocumentReference userRef = db.Collection("users").Document(GamePlayData.Instance.uid);
        userRef.SetAsync(documentDictionary, SetOptions.MergeAll);
    }

    public void SetScores(int stage, int score)
    {
        DocumentReference userRef = db.Collection("users").Document(GamePlayData.Instance.uid);
        userRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            DocumentSnapshot snapshot = task.Result;

            DebugX.Log(String.Format("User: {0}", snapshot.Id));
            Dictionary<string, object> documentDictionary = snapshot.ToDictionary();

            List<int> refList = ((IEnumerable)documentDictionary["Scores"]).Cast<object>()
                             .Select(x => Int32.Parse(x.ToString()))
                             .ToList();
            if (refList.Count < stage)
            {
                refList.Add(score);
            }
            else
            {
                refList[stage] = score;
            }

            Dictionary<string, object> updateDic = new Dictionary<string, object>();
            updateDic.Add("Scores", refList);
            userRef.SetAsync(updateDic, SetOptions.MergeAll);
        });
    }

    public void SetIsAllPerfectLevels(int stage, bool isAllPerfect)
    {
        DocumentReference userRef = db.Collection("users").Document(GamePlayData.Instance.uid);
        userRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            DocumentSnapshot snapshot = task.Result;

            DebugX.Log(String.Format("User: {0}", snapshot.Id));
            Dictionary<string, object> documentDictionary = snapshot.ToDictionary();

            List<bool> refList = ((IEnumerable)documentDictionary["AllPerfects"]).Cast<object>()
                             .Select(x => bool.Parse(x.ToString()))
                             .ToList();

            if (refList.Count < stage)
            {
                refList.Add(isAllPerfect);
            }
            else
            {
                refList[stage] = isAllPerfect;
            }

            Dictionary<string, object> updateDic = new Dictionary<string, object>();
            updateDic.Add("AllPerfects", refList);
            userRef.SetAsync(updateDic, SetOptions.MergeAll);
        });
    }

    public void SetItemsCnt(Define.UsingItemBeforeInGame gameItem, int cnt)
    {
        DocumentReference userRef = db.Collection("users").Document(GamePlayData.Instance.uid);
        userRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            DocumentSnapshot snapshot = task.Result;

            DebugX.Log(String.Format("User: {0}", snapshot.Id));
            Dictionary<string, object> documentDictionary = snapshot.ToDictionary();

            List<int> refList = ((IEnumerable)documentDictionary["Items"]).Cast<object>()
                             .Select(x => Int32.Parse(x.ToString()))
                             .ToList();

            refList[(int)gameItem] = cnt;

            Dictionary<string, object> updateDic = new Dictionary<string, object>();
            updateDic.Add("Items", refList);
            userRef.SetAsync(updateDic, SetOptions.MergeAll);
        });
    }
    #endregion

}

[Serializable]
public class RealtimeUserData
{
    public int maxStage;
    public int coinCnt;
    public int quaverCnt;
}