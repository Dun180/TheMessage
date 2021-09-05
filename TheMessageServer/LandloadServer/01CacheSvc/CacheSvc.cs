//缓存服务
using System;
using System.Collections.Generic;
using System.Text;
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
            this.Warn("Offline Warn:onlineIDDic not exist current playerid:{0}", playerData.id);
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

    public void RequestRoomMsg(MsgPack pack)
    {
        int idRoomDicCount = idRoomDic.Count;

        RoomMsg[] rspRoomMsg = new RoomMsg[idRoomDicCount];
        MessageRoom[] messageRoom = new MessageRoom[idRoomDicCount];
        for(int i = 0;i < idRoomDicCount; i++) {
            idRoomDic.TryGetValue(i, out messageRoom[i]);
            rspRoomMsg[i] = new RoomMsg
            {
                roomID = messageRoom[i].RoomID,
                roomOwner = messageRoom[i].roomOwner,
                roomNumber = messageRoom[i].roomNumber
            };
        }
       
        GameMsg msg = new GameMsg
        {
            cmd = CMD.ResponseRoomMsg,
            roomMsg = rspRoomMsg



        };

        pack.token.SendMsg(msg);
    }


    public void JoinRoom(int roomID,ServerToken token)
    {
        idRoomDic.TryGetValue(roomID, out MessageRoom messageRoom);
        onLineTokenDic.TryGetValue(token, out PlayerData playerData);

        MessagePlayer player = new MessagePlayer
        {
            token = token,
            id = playerData.id,
            name = playerData.name,
            iconIndex = playerData.iconIndex,
        };
        messageRoom.AddMessagePlayer(player, messageRoom.roomNumber);

        GameMsg msg = new GameMsg
        {
            cmd = CMD.ResponseJoinRoomMsg,
            roomMsg = new RoomMsg[1]
{
                new RoomMsg{
                    roomID = roomID,
                    roomOwner = messageRoom.roomOwner,
                    roomNumber = messageRoom.roomNumber,
                    playerArr = messageRoom.matchPlayerArr
                }
}
        };

        for(int i = 0; i < messageRoom.roomNumber; i++)
        {
            messageRoom.playerArr[i].token.SendMsg(msg);
        }

        
    }
}
