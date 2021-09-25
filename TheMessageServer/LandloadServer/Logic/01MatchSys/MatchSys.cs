//匹配系统
using System;
using System.Collections.Generic;
using System.Text;
using PENet;
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
    private MessageRoom messageRoom = null;
    public void Init()
    {
        cacheSvc = CacheSvc.Instance;
        
    }

    public void Update()
    {

    }






    public void RequestAddRoom(MsgPack pack)
    {
        if (cacheSvc.IsInRoom(pack.token))//判断是否已在房间中
        {
            GameMsg errMsg = new GameMsg { cmd = CMD.ResponseAddRoom,err = ErrorCode.AlreadyInRoomError };
            
            pack.token.SendMsg(errMsg);
            return;
        }
        int messageRoomID = cacheSvc.GetUniqueRoomID();//获得房间ID

        if(messageRoomID == -1)
        {
            GameMsg errMsg = new GameMsg { cmd = CMD.ResponseAddRoom, err = ErrorCode.FullRoomCount };
            pack.token.SendMsg(errMsg);
            return;
        }

        PlayerData playerData = cacheSvc.GetPlayerDataByToken(pack.token);//通过token获得创建者的信息
        if (playerData == null)//创建者信息为空时返回错误
        {
            GameMsg errMsg = new GameMsg { cmd = CMD.ResponseAddRoom, err = ErrorCode.ServerDataError};
            this.Error("OnlineTokenDic Data Error.");
            pack.token.SendMsg(errMsg);
            return;
        }

        messageRoom = new MessageRoom(messageRoomID, playerData.name,playerData.id);//通过roomId和创建者名字创建房间

        MessagePlayer player = new MessagePlayer    //创建玩家信息
        {
            token = pack.token,
            id = playerData.id,
            name = playerData.name,
            iconIndex = playerData.iconIndex,
            posIndex = 0,
        };
        cacheSvc.AddTokenRoomDic(pack.token,messageRoomID);
        messageRoom.AddMessagePlayer(player, 0);//在房间中添加玩家，并设置索引号
        GameMsg msg = new GameMsg
        {
            cmd = CMD.ResponseAddRoom,
            detailRoomMsg = new DetailRoomMsg
            {
                roomID = messageRoomID,
                roomOwner = messageRoom.roomOwner,
                roomNumber = messageRoom.roomNumber,
                playerArr = messageRoom.matchPlayerArr
            }
        };
        



        cacheSvc.AddIDRoomDic(messageRoom);//将已创建的房间存到cacheSvc中

        pack.token.SendMsg(msg);//将响应传回
        messageRoom = null;
    }


    public void RequestJoinRoom(MsgPack pack)
    {
        if (cacheSvc.IsInRoom(pack.token))//判断是否已在房间中
        {
            GameMsg errMsg = new GameMsg { cmd = CMD.ResponseAddRoom, err = ErrorCode.AlreadyInRoomError };
            
            pack.token.SendMsg(errMsg);
            return;
        }
        int roomID = pack.msg.requestJoinRoomMsg.roomID;
        ServerToken token = pack.token;

        MessageRoom messageRoom = cacheSvc.GetMessageRoomById(pack.msg.requestJoinRoomMsg.roomID);
        PlayerData playerData = cacheSvc.GetPlayerDataByToken(pack.token);

        //判断人数是否已满
        if (messageRoom.roomNumber == 5)
        {
            GameMsg errMsg = new GameMsg
            {
                cmd = CMD.PushJoinRoomMsg,
                err = ErrorCode.FullRoom

            };
            token.SendMsg(errMsg);
            return;
        }


        MessagePlayer player = new MessagePlayer
        {
            token = token,
            id = playerData.id,
            name = playerData.name,
            iconIndex = playerData.iconIndex,
            posIndex = messageRoom.roomNumber
        };
        messageRoom.AddMessagePlayer(player, messageRoom.roomNumber);
        cacheSvc.AddTokenRoomDic(token, roomID);
        //将加入房间推送给房间中的所有人


        GameMsg msg = new GameMsg
        {
            cmd = CMD.PushJoinRoomMsg,
            detailRoomMsg = new DetailRoomMsg
            {
                roomID = roomID,
                roomOwner = messageRoom.roomOwner,
                roomNumber = messageRoom.roomNumber,
                playerArr = messageRoom.matchPlayerArr
            }

        };

        cacheSvc.SendMsgAll(messageRoom, msg);
    }


    public void RequestRoomMsg(MsgPack pack)
    {
        int idRoomDicCount = 5;

        RoomMsg[] rspRoomMsg = new RoomMsg[idRoomDicCount];
        MessageRoom[] messageRoom = new MessageRoom[idRoomDicCount];
        for (int i = 0; i < idRoomDicCount; i++)
        {
            messageRoom[i] = cacheSvc.GetMessageRoomById(i);
            if (messageRoom[i] != null)
            {
                rspRoomMsg[i] = new RoomMsg
                {
                    roomID = messageRoom[i].RoomID,
                    roomOwner = messageRoom[i].roomOwner,
                    roomNumber = messageRoom[i].roomNumber
                };
            }
            else
            {
                rspRoomMsg[i] = null;
            }

        }

        GameMsg msg = new GameMsg
        {
            cmd = CMD.ResponseRoomMsg,
            roomMsg = rspRoomMsg



        };

        pack.token.SendMsg(msg);
    }

    public void RequestReady(MsgPack pack)
    {
        MessageRoom messageRoom = cacheSvc.GetMessageRoomByToken(pack.token);
        PlayerData playerData = cacheSvc.GetPlayerDataByToken(pack.token);

        int mPosIndex = messageRoom.GameReady(playerData.id);
        if (mPosIndex >= 0)
        {
            GameMsg msg = new GameMsg
            {
                cmd = CMD.PushReady,
                pushReady = new PushReady { posIndex = mPosIndex, isReady = true }
            };
            cacheSvc.SendMsgAll(messageRoom, msg);
        }
    }

    public void RequestUnReady(MsgPack pack)
    {
        MessageRoom messageRoom = cacheSvc.GetMessageRoomByToken(pack.token);
        PlayerData playerData = cacheSvc.GetPlayerDataByToken(pack.token);

        int mPosIndex = messageRoom.CancelReady(playerData.id);
        if (mPosIndex >= 0)
        {
            GameMsg msg = new GameMsg
            {
                cmd = CMD.PushReady,
                pushReady = new PushReady { posIndex = mPosIndex, isReady = false }
            };
            cacheSvc.SendMsgAll(messageRoom, msg);
        }
    }

    public void RequestExitRoom(MsgPack pack)
    {
        MessageRoom messageRoom = cacheSvc.GetMessageRoomByToken(pack.token);
        PlayerData playerData = cacheSvc.GetPlayerDataByToken(pack.token);

        if (messageRoom == null) return;

        //需要根据退出房间的人是否为房主来判断，若为房主，则所有人一起退出，若不是，则一人退出
        if (messageRoom.roomOwnerID == playerData.id)
        {
            //若为房主退出房间

            //将所有玩家移出tokenRoomDic
            MessagePlayer[] messagePlayers = messageRoom.playerArr;
            for (int i = 0; i < messagePlayers.Length; i++)
            {
                if (messagePlayers[i] != null)
                {
                    cacheSvc.RemoveTokenRoomDic(messagePlayers[i].token);
                }

            }
            //将房间移出idRoomDic
            cacheSvc.RemoveIDRoomDic(messageRoom.RoomID);
            //给所有玩家分发推送消息
            GameMsg msg = new GameMsg
            {
                cmd = CMD.ResponseExitRoom,
            };
            cacheSvc.SendMsgAll(messageRoom, msg);
        }
        else
        {
            //若为成员退出房间
            messageRoom.ExitMessagePlayer(playerData.id);
            cacheSvc.RemoveTokenRoomDic(pack.token);
            GameMsg msg = new GameMsg
            {
                cmd = CMD.PushExitRoom,
                detailRoomMsg = new DetailRoomMsg
                {
                    roomID = messageRoom.RoomID,
                    roomOwner = messageRoom.roomOwner,
                    roomNumber = messageRoom.roomNumber,
                    playerArr = messageRoom.matchPlayerArr
                }

            };

            cacheSvc.SendMsgAll(messageRoom, msg);

            GameMsg rspMsg = new GameMsg
            {
                cmd = CMD.ResponseExitRoom,
            };

            pack.token.SendMsg(rspMsg);
        }
    }

    public void RequestGameStart(MsgPack pack)
    {
        MessageRoom messageRoom = cacheSvc.GetMessageRoomByToken(pack.token);
        GameMsg msg = new GameMsg { cmd = CMD.PushGameStart };
        if (messageRoom.AllReady())
        {
            cacheSvc.SendMsgAll(messageRoom, msg);
            FightSys.Instance.GameStart(messageRoom);

        }
        else
        {
            msg.err = ErrorCode.NotAllReady;
            pack.token.SendMsg(msg);
        }
    }


}
