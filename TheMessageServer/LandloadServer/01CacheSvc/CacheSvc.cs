//缓存服务
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

    private Dictionary<int, string> indexCharDic = new Dictionary<int, string>() 
    {
        {0,"默认角色" },
        {1,"老鬼" },
        {2,"老枪" },
        {3,"老金" },
        {4,"蝮蛇" },
        {5,"钢铁特工" },
        {6,"戴笠" },
        {7,"怪盗九九" },
        {8,"小马哥" },
        {9,"职业杀手" },
        {10,"贝雷帽" },
        {11,"黄雀" },
        {12,"峨眉峰" },
        {13,"柒佰" },
        {14,"闪灵" },
        {15,"译电员" },
        {16,"福尔摩斯" },
        {17,"礼服蒙面人" },
        {18,"刀锋" },
        {19,"情报处长" },
        {20,"黑玫瑰" },
        {21,"致命香水" },
        {22,"小白" },
        {23,"浮萍" },
        {24,"六姐" },
        {25,"大美女" },
    };
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

    public void RemoveTokenRoomDic(ServerToken token)
    {
        tokenRoomDic.Remove(token);
    }
    public void RemoveIDRoomDic(int id)
    {
        idRoomDic.Remove(id);
    }

    public string GetCharNameByIndex(int index)
    {
        if(indexCharDic.TryGetValue(index, out string name))
        {
            return name;
        }
        else
        {
            return null;
        }
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
                this.Warn("MessageRoom is NULL.");
            }
        }
        else
        {
            this.Warn("TokenID:{0} no pokerRoom exist.", token.tokenID);

        }
        return null;
    }
    public MessageRoom GetMessageRoomById(int id)
    {
        if (idRoomDic.TryGetValue(id, out MessageRoom messageRoom))
        {
            return messageRoom;
        }
        return null;
    }
    public PlayerData GetPlayerDataByToken(ServerToken token)
    {
        if (onLineTokenDic.TryGetValue(token, out PlayerData playerData))
        {
            return playerData;
        }
        else
        {
            return null;
        }
    }
    public PlayerData GetPlayerData(string acct, string pass)
    {
        PlayerData playerData = dBSvc.QueryAcctDataByAcctPass(acct, pass);
        return playerData;
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
        for (int i = 0; i < 5; i++)
        {
            idRoomDic.TryGetValue(i, out MessageRoom messageRoom);
            if (messageRoom == null)
            {
                id = i;
                return id;
            }
        }
        return id;
    }

    public bool IsAcctOnline(string acct)
    {

        return onLineAcctDic.ContainsKey(acct);
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



    public bool IsInRoom(ServerToken token)
    {
        if (!tokenRoomDic.TryGetValue(token, out int room))
        {
            return false;
        }
        return true;
    }






    //TOOL METHONDS
    //群发消息
    public void SendMsgAll(MessageRoom messageRoom,GameMsg msg)
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
