//逻辑业务协议
using PENet;
using System;

namespace PEProtocol
{
    [Serializable]
    public class GameMsg : IOCPMsg
    {
        public CMD cmd;
        public ErrorCode err;

        public ReqLogin reqLogin;
        public RspLogin rspLogin;

        public PshMatch pshMatch;
        public RoomMsg[] roomMsg;
        public DetailRoomMsg detailRoomMsg;
        public PushReady pushReady;
        public int roomID;
    }
    [Serializable]
    public class ReqLogin
    {
        public string acct;
        public string pass;
    }
    [Serializable]
    public class RspLogin
    {
        public PlayerData playerData;
    }
    [Serializable]
    public class PlayerData
    {
        public int id;
        public string name;
        public int lv;
        public int exp;
        public int coin;
        public int diamond;
        public int win;
        public int lose;
        public int winlast;
        public int iconIndex;

    }
    [Serializable]
    public class PshMatch
    {
        public int RoomID;
        public int selfPosIndex;

        public MatchPlayerData[] playerArr;
    }
    [Serializable]
    public class RoomMsg
    {
        public int roomID;
        public string roomOwner;
        public int roomNumber;
    }
    [Serializable]
    public class DetailRoomMsg
    {
        public int roomID;
        public string roomOwner;
        public int roomNumber;

        public MatchPlayerData[] playerArr;
    }
    [Serializable]
    public class MatchPlayerData
    {
        public int id;
        public string name;


        public int posIndex;
        public int iconIndex;

        public bool isReady;
    }
    [Serializable]
    public class PushReady
    {
        public int posIndex;
    }
    public enum ErrorCode
    {
        None,
        AcctIsOnline,
        WrongPass,
        ServerDataError,
        AlreadyInRoomError
    }
    public enum CMD
    {
        None = 0,

        ReqLogin ,
        RspLogin ,

        ReqMatch ,
        RspMatch ,
        PshMatch ,

        RequestAddRoom ,
        ResponseAddRoom ,

        RequestRoomMsg ,
        ResponseRoomMsg ,

        RequestJoinRoomMsg ,
        PushJoinRoomMsg ,

        RequestReady ,
        PushReady ,

        RequestGameStart ,

        OnConnected ,
        OnDisConnected ,
    }
}
