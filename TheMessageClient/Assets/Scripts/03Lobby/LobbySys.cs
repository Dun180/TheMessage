//大厅系统
using UnityEngine;
using PEProtocol;

public class LobbySys : MonoBehaviour
{

    public static LobbySys Instance;
    public LobbyWindow lobbyWindow;
    private NetSvc netSvc;
    private FightSys fightSys;


    public PlayerData playerData{get;private set;} = null;


    private void Awake(){
        Instance = this;
    }
    public void Init()
    {
        netSvc =NetSvc.Instance;
        fightSys = FightSys.Instance;
    }

    public void SetPlayerData(RspLogin data){
        playerData = data.playerData;
    }

    public void EnterLobby(){
        lobbyWindow.InitWindow();
        lobbyWindow.SetWindowState();
        lobbyWindow.RefreshWindow();
        netSvc.SendMsg(new GameMsg { cmd = CMD.RequestRoomMsg });
    }

    public void ReqMatch()
    {
        netSvc.SendMsg(new GameMsg { cmd = CMD.ReqMatch });
        fightSys.EnterFight();
    }

    public void RspMatch(GameMsg msg)
    {
        if(msg.err == ErrorCode.ServerDataError)
        {
            TipsWindow.AddTips("数据异常，请稍后重试");
        }
    }

    //发送创建房间的请求
    public void RequestAddRoom()
    {
        netSvc.SendMsg(new GameMsg { cmd = CMD.RequestAddRoom });
    }

    //接收回传的创建房间请求
    public void ResponseAddRoom(GameMsg msg)
    {
        if (msg.err == ErrorCode.ServerDataError)
        {
            TipsWindow.AddTips("数据异常，请稍后重试");
            return;
        }
        int roomID = msg.roomMsg[0].roomID;
        lobbyWindow.room[roomID].SetActive(true);

        lobbyWindow.SetRoomName(roomID, msg.roomMsg[0].roomOwner);
        lobbyWindow.ShowRoomDetails(msg.roomMsg[0], true);

    }

    public void RefreshRoom(GameMsg msg)
    {
        for(int i = 0; i < msg.roomMsg.Length; i++)
        {
            lobbyWindow.SetRoomActive(i,true);
            lobbyWindow.SetRoomName(i, msg.roomMsg[i].roomOwner);
            lobbyWindow.SetRoomNumber(i, msg.roomMsg[i].roomNumber);
        }
    }

    public void ResponseJoinRoomMsg(GameMsg msg)
    {
        bool isOwner = false;
        if (msg.roomMsg[0].roomOwner.Equals(playerData.name))
        {
            isOwner = true;
        }
       
        lobbyWindow.ShowRoomDetails(msg.roomMsg[0], isOwner);
        netSvc.SendMsg(new GameMsg { cmd = CMD.RequestRoomMsg });
    }
}