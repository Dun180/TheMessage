//大厅系统
using UnityEngine;
using PEProtocol;



public enum PlayerState
{
    None,
    UnReady,
    Ready,
    RoomOwner
}
public class LobbySys : MonoBehaviour
{

    public static LobbySys Instance;
    public LobbyWindow lobbyWindow;
    
    private NetSvc netSvc;
    private FightSys fightSys;


    public PlayerData playerData{get;private set;} = null;
    public PlayerState playerState = PlayerState.None;

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
        }else if(msg.err == ErrorCode.AlreadyInRoomError)
        {
            TipsWindow.AddTips("您已在房间中，请退出重试");
            return;
        }else if(msg.err == ErrorCode.FullRoomCount)
        {
            TipsWindow.AddTips("房间已满，请稍后重试");
            return;
        }
        int roomID = msg.detailRoomMsg.roomID;
        lobbyWindow.room[roomID].SetActive(true);

        lobbyWindow.SetRoomName(roomID, msg.detailRoomMsg.roomOwner);
        lobbyWindow.SetRoomNumber(roomID, 1);
        playerState = PlayerState.RoomOwner;
        lobbyWindow.ShowRoomDetails(msg.detailRoomMsg);

        netSvc.SendMsg(new GameMsg { cmd = CMD.RequestRoomMsg });


    }

    //刷新房间
    public void RefreshRoom(GameMsg msg)
    {
        this.Log("msg:{0}",msg.roomMsg.ToString());
        for(int i = 0; i < msg.roomMsg.Length; i++)
        {
            if (msg.roomMsg[i] != null) {
                lobbyWindow.SetRoomActive(i, true);
                lobbyWindow.SetRoomName(i, msg.roomMsg[i].roomOwner);
                lobbyWindow.SetRoomNumber(i, msg.roomMsg[i].roomNumber);
            }
            else
            {
                lobbyWindow.SetRoomActive(i, false);

            }

        }
    }

    //处理推送的加入房间请求
    public void PushJoinRoom(GameMsg msg)
    {
        if (msg.err == ErrorCode.FullRoom)
        {
            TipsWindow.AddTips("人数已满");
            return;
        }
        if(playerState == PlayerState.None)
        {
            playerState = PlayerState.UnReady;
        }
              
        lobbyWindow.ShowRoomDetails(msg.detailRoomMsg);
        netSvc.SendMsg(new GameMsg { cmd = CMD.RequestRoomMsg });
    }

    //处理推送的玩家准备请求
    public void PushReady(GameMsg msg)
    {
        lobbyWindow.ShowReady(msg.pushReady.posIndex,msg.pushReady.isReady);
    }

    //处理推送的退出房间请求
    public void PushExitRoom(GameMsg msg)
    {

        lobbyWindow.ShowRoomDetails(msg.detailRoomMsg);
        netSvc.SendMsg(new GameMsg { cmd = CMD.RequestRoomMsg });
    }

    public void ResponseExitRoom()
    {
        lobbyWindow.ExitRoom();
        netSvc.SendMsg(new GameMsg { cmd = CMD.RequestRoomMsg });
    }
    //处理游戏开始
    public void PushGameStart(GameMsg msg)
    {
        if (msg.err == ErrorCode.NotAllReady)
        {
            TipsWindow.AddTips("请确认所有玩家准备后再开始");
            return;
        }
        TipsWindow.AddTips("游戏开始");
        lobbyWindow.SetWindowState(false);
        fightSys.EnterMessage();

    }
}