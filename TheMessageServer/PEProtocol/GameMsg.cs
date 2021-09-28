//逻辑业务协议
using PENet;
using System;
using System.Collections.Generic;

namespace PEProtocol
{
    [Serializable]
    public class GameMsg : IOCPMsg
    {
        public CMD cmd;
        public ErrorCode err;

        public ReqLogin reqLogin;
        public RspLogin rspLogin;

        public RequestJoinRoomMsg requestJoinRoomMsg;

        public PshMatch pshMatch;
        public RoomMsg[] roomMsg;
        public DetailRoomMsg detailRoomMsg;
        public PushReady pushReady;
        public ResponseRefreshMessage responseRefreshMessage;
        public PushChar pushChar;
        public RequestSelectChar requestSelectChar;
        public PushSelectChar pushSelectChar;
        public PushIdentityInfo pushIdentityInfo;
        public PushCard pushCard;
        public PushDrawCard pushDrawCard;
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


        public int posIndex;//位置索引
        public int iconIndex;//头像索引

        public bool isReady;


        public int cards;//手牌数
        public int redNum;//红情报数
        public int blueNum;//蓝情报数
        public int blackNum;//黑情报数

        public int charIndex;//人物对应索引
        public String charName;//人物名字
    }
    [Serializable]
    public class RequestJoinRoomMsg
    {
        public int roomID;
    }
    [Serializable]
    public class PushReady
    {
        public int posIndex;
        public bool isReady;
    }
    [Serializable]
    public class ResponseRefreshMessage
    {
        public int selfPosIndex;

        public MatchPlayerData[] playerArr;
    }

    [Serializable]
    public class PushChar
    {
        public int char_1;
        public int char_2;
        public int char_3;
    }

    [Serializable]
    public class RequestSelectChar
    {
        public int charIndex;
    }
    [Serializable]
    public class PushSelectChar
    {
        public MatchPlayerData[] playerArr;
    }

    [Serializable]
    public class PushIdentityInfo
    {
        public int identity;
    }

    [Serializable]
    public class PushCard
    {
        public List<Card> cardList;
    }
    [Serializable]
    public class PushDrawCard
    {
        public List<Card> cardList;
        public int index;
    }
    public enum ErrorCode
    {
        None,
        AcctIsOnline,
        WrongPass,
        ServerDataError,
        AlreadyInRoomError,
        NotAllReady,
        FullRoom,//房间内人数已满
        FullRoomCount//房间的数量已满
    }
    public enum CMD
    {
        None = 0,

        //登录
        ReqLogin ,
        RspLogin ,


        //匹配
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
        RequestUnReady,
        PushReady ,
        
        RequestExitRoom,
        PushExitRoom,
        ResponseExitRoom,

        RequestGameStart ,
        PushGameStart,

        //游戏

        RequestRefreshMessage,
        ResponseRefreshMessage,

        PushChar,//推送角色牌索引
        RequestSelectChar,//将选择的角色信息发送给服务器
        PushSelectChar,

        PushIdentityInfo,//推送玩家身份信息

        PushCard,//推送手牌
        PushDrawCard,//推送抽卡
        //其他
        OnConnected ,
        OnDisConnected ,
    }
}
