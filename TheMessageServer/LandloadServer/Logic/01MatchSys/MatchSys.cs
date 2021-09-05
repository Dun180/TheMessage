//匹配系统
using System;
using System.Collections.Generic;
using System.Text;
using PEProtocol;

public class MatchSys
{
    private static MatchSys instance = null;
    public static MatchSys Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new MatchSys();

            }
            return instance;
        }
    }

    private CacheSvc cacheSvc;
    private int PosIndex = 0;
    private int RoomID = 0;
    private PokerRoom matchingRoom = null;
    private MessageRoom messageRoom = null;
    public void Init()
    {
        cacheSvc = CacheSvc.Instance;
        
    }

    public void Update()
    {

    }

    public void ReqMatch(MsgPack pack)
    {
        PlayerData playerData = cacheSvc.GetPlayerDataByToken(pack.token);
        GameMsg msg = new GameMsg
        {
            cmd = CMD.RspMatch
        };
        
        if(playerData == null)
        {
            this.Error("OnlineTokenDic Data Error.");
            msg.err = ErrorCode.ServerDataError;
            pack.token.SendMsg(msg);
            return;
        }

        PokerPlayer player = new PokerPlayer
        {
            token = pack.token,
            id = playerData.id,
            name = playerData.name,
            coin = playerData.coin
        };

        player.posIndex = PosIndex;
        player.iconIndex = PosIndex;
        if (PosIndex == 0)
        {
            matchingRoom = new PokerRoom(GetUniqueRoomID());
            matchingRoom.AddPokerPlayer(player, PosIndex);
        }
        else
        {
            matchingRoom.AddPokerPlayer(player, PosIndex);
        }
        ++PosIndex;
        if(matchingRoom.roomState == RoomState.Matching && PosIndex > 2)
        {
            //房间已满
            matchingRoom.roomState = RoomState.Matched;
            FightSys.Instance.AddFightRoom(matchingRoom);
            ResetMatchingRoom();

        }


        pack.token.SendMsg(msg);

    }

    void ResetMatchingRoom()
    {
        matchingRoom = null;
        PosIndex = 0;
        RoomID = 0;
    }

    private int GetUniqueRoomID()
    {
        if(RoomID == int.MaxValue)
        {
            RoomID = 0;
        }
        return RoomID++;
    }

    public void RequestAddRoom(MsgPack pack)
    {
        int messageRoomID = GetUniqueRoomID();
        PlayerData playerData = cacheSvc.GetPlayerDataByToken(pack.token);
        if (playerData == null)
        {
            GameMsg errMsg = new GameMsg {err = ErrorCode.ServerDataError};
            this.Error("OnlineTokenDic Data Error.");
            pack.token.SendMsg(errMsg);
            return;
        }
        messageRoom = new MessageRoom(messageRoomID, playerData.name);

        MessagePlayer player = new MessagePlayer
        {
            token = pack.token,
            id = playerData.id,
            name = playerData.name,
            iconIndex = playerData.iconIndex,
        };
        cacheSvc.AddTokenRoomDic(pack.token,messageRoomID);
        messageRoom.AddMessagePlayer(player, 0);
        GameMsg msg = new GameMsg
        {
            cmd = CMD.ResponseAddRoom,
            roomMsg = new RoomMsg[1]
            {
                new RoomMsg{
                    roomID = messageRoomID,
                    roomOwner = playerData.name,
                    roomNumber = 1,
                    playerArr = messageRoom.matchPlayerArr
                }
            }
        };
        



        cacheSvc.AddIDRoomDic(messageRoom);

        pack.token.SendMsg(msg);
        messageRoom = null;
    }


    public void RequestJoinRoom(MsgPack pack)
    {
        int roomID = pack.msg.roomID;
        ServerToken token = pack.token;
        cacheSvc.JoinRoom(roomID, token);
        cacheSvc.AddTokenRoomDic(pack.token, roomID);

    }

    public void RequestReady(MsgPack pack)
    {
        int roomID = cacheSvc.GetRoomIDByToken(pack.token);
        cacheSvc.GetIDRoomDic().TryGetValue(roomID, out MessageRoom messageRoom);

    }

    public void RequestGameStart(MsgPack pack)
    {

    }
}
