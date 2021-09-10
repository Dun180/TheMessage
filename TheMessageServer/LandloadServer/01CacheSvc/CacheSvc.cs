﻿//缓存服务
using System;
using System.Collections.Generic;
using System.Text;
using PENet;
using PEProtocol;

public class CacheSvc
{
    private static CacheSvc instance = null;
    public static CacheSvc Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new CacheSvc();

            }
            return instance;
        }
    }
    private DBSvc dBSvc;
    private Dictionary<string, ServerToken> onLineAcctDic = new Dictionary<string, ServerToken>();
    private Dictionary<ServerToken, PlayerData> onLineTokenDic = new Dictionary<ServerToken, PlayerData>();
    private Dictionary<int, PlayerData> onLineIdDic = new Dictionary<int, PlayerData>();

    private Dictionary<ServerToken, int> tokenRoomDic = new Dictionary<ServerToken, int>();
    private Dictionary<int, MessageRoom> idRoomDic = new Dictionary<int, MessageRoom>();
    public void Init()
    {
        dBSvc = DBSvc.Instance;
    }

    public void Update()
    {

    }



    public void AddTokenRoomDic(ServerToken token,int roomID)
    {
        if (!tokenRoomDic.TryGetValue(token, out int room))
        {
            tokenRoomDic.Add(token, roomID);
        }
    }

    public void AddIDRoomDic(MessageRoom messageRoom)
    {
        idRoomDic.Add(messageRoom.RoomID, messageRoom);
    }

    public MessageRoom GetMessageRoomByToken(ServerToken token)
    {
        if(tokenRoomDic.TryGetValue(token,out int roomID)){
            if(idRoomDic.TryGetValue(roomID,out MessageRoom messageRoom))
            {
                return messageRoom;
            }
            else
            {
                this.Warn("PokerRoom is NULL.");
            }
        }
        else
        {
            this.Warn("TokenID:{0} no pokerRoom exist.", token.tokenID);

        }
        return null;
    }

    public Dictionary<int, MessageRoom> GetIDRoomDic()
    {
        return idRoomDic;
    }

    public bool IsAcctOnline(string acct)
    {

        return onLineAcctDic.ContainsKey(acct);
    }

    public PlayerData GetPlayerData(string acct,string pass)
    {
        PlayerData playerData = dBSvc.QueryAcctDataByAcctPass(acct, pass);
        return playerData;
    }

    public void AcctOnline(string acct,ServerToken token,PlayerData playerData)
    {
        onLineAcctDic.Add(acct, token);
        onLineTokenDic.Add(token, playerData);
        onLineIdDic.Add(playerData.id, playerData);
    }

    public void AcctOffline(ServerToken token)
    {
        foreach(var item in onLineAcctDic)
        {
            if(item.Value == token)
            {
                onLineAcctDic.Remove(item.Key);
                break;
            }
        }

        if(onLineTokenDic.TryGetValue(token,out PlayerData playerData))
        {
            onLineTokenDic.Remove(token);
            if (onLineIdDic.ContainsKey(playerData.id))
            {
                onLineIdDic.Remove(playerData.id);
            }
            else
            {
                this.Warn("Offline Warn:onlineIDDic not exist current playerid:{0}", playerData.id);
            }
        }
        else
        {
            this.Warn("token PlayerData is Null");
        }
    }

    public PlayerData GetPlayerDataByToken(ServerToken token)
    {
        if(onLineTokenDic.TryGetValue(token,out PlayerData playerData))
        {
            return playerData;
        }
        else
        {
            return null;
        }
    }

    public int GetRoomIDByToken(ServerToken token)
    {
        if (tokenRoomDic.TryGetValue(token, out int roomID))
        {
            return roomID;
        }
        else
        {
            return -1;
        }
    }

    public int GetUniqueRoomID()
    {
        int id = -1;
        for(int i = 0; i < 5; i++)
        {
            idRoomDic.TryGetValue(i, out MessageRoom messageRoom);
            if(messageRoom == null)
            {
                id = i;
                return id;
            }
        }
        return id;
    }

    public void RequestRoomMsg(MsgPack pack)
    {
        int idRoomDicCount = 5;

        RoomMsg[] rspRoomMsg = new RoomMsg[idRoomDicCount];
        MessageRoom[] messageRoom = new MessageRoom[idRoomDicCount];
        for(int i = 0;i < idRoomDicCount; i++) {
            idRoomDic.TryGetValue(i, out messageRoom[i]);
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

    //加入房间
    public void JoinRoom(int roomID,ServerToken token)
    {
        idRoomDic.TryGetValue(roomID, out MessageRoom messageRoom);//获得想要加入的房间
        onLineTokenDic.TryGetValue(token, out PlayerData playerData);//获得加入者的数据
        //判断人数是否已满
        if(messageRoom.roomNumber == 5)
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
        AddTokenRoomDic(token, roomID);
        //将加入房间推送给房间中的所有人


        GameMsg msg = new GameMsg
        {
            cmd = CMD.PushJoinRoomMsg,
            detailRoomMsg =new DetailRoomMsg{
                    roomID = roomID,
                    roomOwner = messageRoom.roomOwner,
                    roomNumber = messageRoom.roomNumber,
                    playerArr = messageRoom.matchPlayerArr
                }

        };

        SendMsgAll(messageRoom, msg);

        
    }

    public bool IsInRoom(ServerToken token)
    {
        if (!tokenRoomDic.TryGetValue(token, out int room))
        {
            return false;
        }
        return true;
    }



    public void RequestReady(MsgPack pack)
    {
        
        tokenRoomDic.TryGetValue(pack.token, out int roomID);
        idRoomDic.TryGetValue(roomID, out MessageRoom messageRoom);
        PlayerData playerData = GetPlayerDataByToken(pack.token);
        
        int mPosIndex = messageRoom.GameReady(playerData.id);
        if (mPosIndex >= 0)
        {
            GameMsg msg = new GameMsg
            {
                cmd = CMD.PushReady,
                pushReady = new PushReady { posIndex = mPosIndex,isReady = true }
            };
            SendMsgAll(messageRoom, msg);
        }
    }

    public void RequestUnReady(MsgPack pack)
    {
        tokenRoomDic.TryGetValue(pack.token, out int roomID);
        idRoomDic.TryGetValue(roomID, out MessageRoom messageRoom);
        PlayerData playerData = GetPlayerDataByToken(pack.token);

        int mPosIndex = messageRoom.CancelReady(playerData.id);
        if (mPosIndex >= 0)
        {
            GameMsg msg = new GameMsg
            {
                cmd = CMD.PushReady,
                pushReady = new PushReady { posIndex = mPosIndex, isReady = false }
            };
            SendMsgAll(messageRoom, msg);
        }
    }

    public void RequestExitRoom(MsgPack pack)
    {
        tokenRoomDic.TryGetValue(pack.token, out int roomID);
        idRoomDic.TryGetValue(roomID, out MessageRoom messageRoom);
        PlayerData playerData = GetPlayerDataByToken(pack.token);

        

        //需要根据退出房间的人是否为房主来判断，若为房主，则所有人一起退出，若不是，则一人退出
        if(messageRoom.roomOwnerID == playerData.id)
        {
            //若为房主退出房间

            //将所有玩家移出tokenRoomDic
            MessagePlayer[] messagePlayers =  messageRoom.GetMessagePlayers();
            for(int i = 0; i < messagePlayers.Length; i++)
            {
                if (messagePlayers[i] != null) {
                    tokenRoomDic.Remove(messagePlayers[i].token);
                }
                
            }
            //将房间移出idRoomDic
            idRoomDic.Remove(roomID);
            //给所有玩家分发推送消息
            GameMsg msg = new GameMsg
            {
                cmd = CMD.ResponseExitRoom,
            };
            SendMsgAll(messageRoom, msg);
        }
        else
        {
            //若为成员退出房间
            messageRoom.ExitMessagePlayer(playerData.id);
            tokenRoomDic.Remove(pack.token);
            GameMsg msg = new GameMsg
            {
                cmd = CMD.PushExitRoom,
                detailRoomMsg = new DetailRoomMsg
                {
                    roomID = roomID,
                    roomOwner = messageRoom.roomOwner,
                    roomNumber = messageRoom.roomNumber,
                    playerArr = messageRoom.matchPlayerArr
                }

            };

            SendMsgAll(messageRoom, msg);

            GameMsg rspMsg = new GameMsg
            {
                cmd = CMD.ResponseExitRoom,
            };

            pack.token.SendMsg(rspMsg);
        }
    }

    public void RequestGameStart(MsgPack pack)
    {
        tokenRoomDic.TryGetValue(pack.token, out int roomID);
        idRoomDic.TryGetValue(roomID, out MessageRoom messageRoom);
        GameMsg msg = new GameMsg { cmd = CMD.PushGameStart };
        if (messageRoom.AllReady())
        {
            SendMsgAll(messageRoom, msg);
        }
        else
        {
            msg.err = ErrorCode.NotAllReady;
            pack.token.SendMsg(msg);
        }
    }


    //TOOL METHONDS
    //群发消息
    void SendMsgAll(MessageRoom messageRoom,GameMsg msg)
    {
        //将消息序列化成二进制后直接发送序列化后的数据，因发送的消息相同，可减少序列化的次数，提升性能
        //本来每次发送都要序列化一次，先序列化再发送便只需序列化一次
        byte[] data = IOCPTool.PackLenInfo(IOCPTool.Serialize(msg));
        for (int i = 0; i < messageRoom.playerArr.Length; i++)
        {
            if (messageRoom.playerArr[i] != null)
            {
                messageRoom.playerArr[i].token.SendMsg(data);
            }
        }
    }

    
}