//核心对战逻辑

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PEProtocol;

public class FightSys : MonoBehaviour
{
    public static FightSys Instance;

    public MessageWindow messageWindow;
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

    public void EnterMessage()
    {
        messageWindow.InitWindow();
        messageWindow.SetWindowState();
    }

    //请求刷新房间信息
    public void RequestRefreshMessage()
    {
        netSvc.SendMsg(new GameMsg { cmd = CMD.RequestRefreshMessage });
    }

    //刷新房间信息
    public void ResponseRefreshMessage(GameMsg msg)
    {
        messageWindow.RefreshMessage(msg.responseRefreshMessage.selfPosIndex, msg.responseRefreshMessage.playerArr);
    }

    public void PushChar(GameMsg msg)
    {
        messageWindow.SelectChar(msg.pushChar);
    }

    public void PushSelectChar(GameMsg msg)
    {
        messageWindow.RefreshMessage(messageWindow.selfIndex, msg.pushSelectChar.playerArr);

    }
}
