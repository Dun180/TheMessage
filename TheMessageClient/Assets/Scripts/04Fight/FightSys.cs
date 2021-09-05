//ºËÐÄ¶ÔÕ½Âß¼­

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PEProtocol;

public class FightSys : MonoBehaviour
{
    public static FightSys Instance;

    public InfoWindow infoWindow;

    public MatchPlayerData[] PlayerData { get; private set; } = null;
    public MatchPlayerData LeftPlayerData { get; private set; } = null;
    public MatchPlayerData SelfPlayerData { get; private set; } = null;
    public MatchPlayerData RightPlayerData { get; private set; } = null;
    public int LeftPosIndex { get; private set; } = -1;
    public int SelfPosIndex { get; private set; } = -1;
    public int RightPosIndex { get; private set; } = -1;
    public int BaseScore { get; private set; } = 0;
    public int AddTimes { get; private set; } = 1;
    public int RobTimes { get; private set; } = 0;

    private NetSvc netSvc;
    private LobbySys lobbySys;
    private void Awake()
    {
        Instance = this;
    }

    public void Init()
    {
        netSvc = NetSvc.Instance;
        lobbySys = LobbySys.Instance;
    }

    public void EnterFight()
    {
        infoWindow.InitWindow();
        infoWindow.SetWindowState();
    }

    public void PshMatch(PshMatch data)
    {
        SelfPosIndex = data.selfPosIndex;
        PlayerData = data.playerArr;
        switch (SelfPosIndex)
        {
            case 0:
                LeftPosIndex = 2;
                RightPosIndex = 1;
                SelfPlayerData = PlayerData[0];
                RightPlayerData = PlayerData[1];
                LeftPlayerData = PlayerData[2];
                break;
            case 1:
                LeftPosIndex = 0;
                RightPosIndex = 2;
                SelfPlayerData = PlayerData[1];
                RightPlayerData = PlayerData[2];
                LeftPlayerData = PlayerData[0];
                break;
            case 2:
                LeftPosIndex = 1;
                RightPosIndex = 0;
                SelfPlayerData = PlayerData[2];
                RightPlayerData = PlayerData[0];
                LeftPlayerData = PlayerData[1];
                break;
            default:
                break;

        }

        infoWindow.RefreshWindow();
    }
}
