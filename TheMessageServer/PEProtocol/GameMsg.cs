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
        public PushRoundStart pushRoundStart;
        public PushDrawCard pushDrawCard;
        public RequestMessageTransfer requestMessageTransfer;
        public PushMessageTransfer pushMessageTransfer;
        public RequestOutCard requestOutCard;
        public PushOutCard pushOutCard;
        public PushDisCard pushDisCard;
        public RequestDisCard requestDisCard;
        public RequestMessageInfo requestMessageInfo;
        public ResponseMessageInfo responseMessageInfo;
        public PushProbingInfo pushProbingInfo;
        public PushGamblingCard pushGamblingCard;
        public RequestBalanceInfo requestBalanceInfo;
        public PushRealOrFalseInfo pushRealOrFalseInfo;
        public RequestAcceptMessage requestAcceptMessage;
        public PushMessageTransfering pushMessageTransfering;
        public PushSinglePlayerMessageUpdate pushSinglePlayerMessageUpdate;
        public PushConfirmAcceptMessage pushConfirmAcceptMessage;
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
    public class PushRoundStart
    {
        public int index;
    }
    [Serializable]
    public class PushDrawCard
    {
        public List<Card> cardList;
        public int index;

        public int cardLibraryCount;
    }
    [Serializable]
    public class RequestOutCard
    {
        public Card card;
        public int targetIndex;//目标索引
        public bool hasTarget = false;
        public Card burnCard;
    }
    [Serializable]
    public class PushOutCard
    {
        public Card card;
        public int sendIndex;//出牌人索引
        public int targetIndex;//目标索引
        public bool hasTarget = false;
        public Card burnCard;

    }
    [Serializable]
    public class PushDisCard
    {
        public int targetIndex;//弃牌人索引
    }
    [Serializable]
    public class RequestDisCard
    {
        public Card disCard;//弃牌信息
    }
    [Serializable]
    public class RequestMessageInfo
    {
        public int index;
    }
    [Serializable]
    public class ResponseMessageInfo
    {
        public List<Card> messageList;
    }
    [Serializable]
    public class PushProbingInfo
    {
        public int targetIndex;//目标索引
        public int responseAction;//回应动作
    }
    [Serializable]
    public class PushGamblingCard
    {
        public Card card;
        public int index;
    }
    [Serializable]
    public class RequestBalanceInfo
    {
        public List<Card> cardList;
    }
    [Serializable]
    public class PushRealOrFalseInfo
    {
        public List<Card> cardList;
    }
    [Serializable]
    public class RequestMessageTransfer
    {
        public Card message;
        public int targetIndex;//目标人索引
    }
    [Serializable]
    public class PushMessageTransfer
    {
        public Card message;
        public int transferIndex;//发送人索引
        public int targetIndex;//目标人索引

    }
    [Serializable]
    public class RequestAcceptMessage
    {
        public bool isAccept;
    }
    [Serializable]
    public class PushMessageTransfering
    {
        public Card message;
        public int transferIndex;//发送人索引
        public int targetIndex;//目标人索引
    }
    [Serializable]
    public class PushSinglePlayerMessageUpdate
    {
        public int posIndex;//位置索引



        public int cards;//手牌数
        public int redNum;//红情报数
        public int blueNum;//蓝情报数
        public int blackNum;//黑情报数

        public int cardLibraryCount;//牌库总数
    }
    [Serializable]
    public class PushConfirmAcceptMessage
    {
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

        PushRoundStart,//推送回合开始

        PushDrawCard,//推送抽卡

        PushPlayStage,//推送进行出牌阶段

        RequestOutCard,//请求出牌
        PushOutCard,//推送出牌信息

        PushDisCard,//推送弃牌信息
        RequestDisCard,//请求推送给服务器弃牌信息

        RequestMessageInfo,//请求获取情报信息
        ResponseMessageInfo,//回应情报信息

        PushProbingInfo,//推送试探信息
        PushGamblingCard,//推送博弈信息
        PushBalanceInfo,//推送权衡信息
        RequestBalanceInfo,//请求权衡信息
        PushRealOrFalseInfo,//推送真假莫辨信息

        RequestEndResponseStage,//请求结束响应阶段

        RequestEndPlay,//请求出牌阶段结束
        PushEndPlay,//推送出牌阶段结束

        RequestMessageTransfer,//请求情报传递
        PushMessageTransfer,//推送情报传递信息

        PushAcceptSection,//推送接受小节信息
        RequestAcceptMessage,//请求接收情报信息

        PushMessageTransfering,//推送后续传递的情报信息
        PushConfirmAcceptMessage,//推送确认接收情报信息

        PushSinglePlayerMessageUpdate,//推送单个玩家信息更新
        
        PushRoundEnd,//推送回合结束信息
        
        //其他
        OnConnected,
        OnDisConnected ,
    }
}
