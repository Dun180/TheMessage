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

        messageRoom = new MessageRoom(messageRoomID, playerData.name,playerData.id);//通过roomId和创建者名字创建房间？为什么要用名字 用id更好吧

        MessagePlayer player = new MessagePlayer    //创建玩家信息
        {
            token = pack.token,
            id = playerData.id,
            name = playerData.name,
            iconIndex = playerData.iconIndex,
            posIndex = 0,
        };
        cacheSvc.AddTokenRoomDic(pack.token,messageRoomID);//不知道有什么用= = 
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
        cacheSvc.JoinRoom(roomID, token);
        

    }




}
