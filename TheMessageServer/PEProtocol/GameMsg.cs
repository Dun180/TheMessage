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
    public enum ErrorCode
    {
        None,
        AcctIsOnline,
        WrongPass,
        ServerDataError,
    }
    public enum CMD
    {
        None = 0,

        ReqLogin = 1,
        RspLogin = 2,

        ReqMatch = 3,
        RspMatch = 4,
        PshMatch = 5,

        RequestAddRoom = 6,
        ResponseAddRoom = 7,

        RequestRoomMsg = 8,
        ResponseRoomMsg = 9,

        RequestJoinRoomMsg = 10,
        ResponseJoinRoomMsg = 11,

        RequestReady = 12,
        ResponseReady = 13,

        RequestGameStart = 14,

        OnConnected = 100,
        OnDisConnected = 101,
    }
}
