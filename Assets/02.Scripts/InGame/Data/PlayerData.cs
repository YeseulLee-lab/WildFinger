using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData : MonoBehaviour
{
    public static PlayerData Instance {get; private set; }

    public PlayerInfo playerInfo { get; set; } = new PlayerInfo();
    public OutGameInfo outGameInfo { get; set; } = new OutGameInfo();

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

        playerInfo.SetInfo("T_ID", "T_PSWD", "T_NICKNAME");
        outGameInfo.SetInfo(0, 500);
        //playerInfo.SetGameInfo();
    }
    #endregion
}
