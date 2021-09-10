//风声房间

using PEProtocol;
using System;
using System.Collections.Generic;
using System.Text;


public class MessageRoom
{
    public int RoomID { private set; get; }
    public MessagePlayer[] playerArr = null;
    public RoomState roomState = RoomState.None;
    public string roomOwner = null;
    public int roomOwnerID;
    public MatchPlayerData[] matchPlayerArr = null;

    public int roomNumber { private set; get; }

    private int readyNumber;//准备的人数
    public MessageRoom(int roomID,string roomOwner,int roomOwnerID)
    {
        RoomID = roomID;
        this.roomOwner = roomOwner;
        this.roomOwnerID = roomOwnerID;
        playerArr = new MessagePlayer[5];
        roomState = RoomState.Matching;
        matchPlayerArr = new MatchPlayerData[5];
        roomNumber = 0;
        readyNumber = 0;
    }

    //添加玩家
    public void AddMessagePlayer(MessagePlayer messagePlayer, int posIndex)
    {
        playerArr[posIndex] = messagePlayer;
        MatchPlayerData matchPlayerData = new MatchPlayerData
        {
            id = messagePlayer.id,
            name = messagePlayer.name,
            posIndex = messagePlayer.posIndex,
            iconIndex = messagePlayer.iconIndex,
            isReady = false
        };

        matchPlayerArr[posIndex] = matchPlayerData;
        roomNumber++;
        SyncRoomInfo();
    }
    public void ExitMessagePlayer(int id)
    {
        int posIndex = -1;
        for (int i = 0; i < matchPlayerArr.Length; i++)
        {
            if (matchPlayerArr[i] != null)
            {
                if (id == matchPlayerArr[i].id)
                {
                    matchPlayerArr[i] = null;
                    playerArr[i] = null;
                    posIndex = i;
                    roomNumber--;
                    
                }
            }

        }
        if (posIndex >= 0)
        {
            for (int i = posIndex+1; i < matchPlayerArr.Length; i++)
            {
                matchPlayerArr[i-1] = matchPlayerArr[i];
                playerArr[i - 1] = playerArr[i];
            }
            matchPlayerArr[matchPlayerArr.Length-1] = null;
            playerArr[playerArr.Length-1] = null;
        }

    }
    //同步房间信息
    private void SyncRoomInfo()
    {
        GameMsg msg = new GameMsg
        {
            cmd = CMD.PshMatch,
            pshMatch = new PshMatch
            {
                RoomID = RoomID,
                playerArr = matchPlayerArr
            }
        };

        for (int i = 0; i < playerArr.Length; i++)
        {
            MessagePlayer player = playerArr[i];
            if (player != null)
            {
                msg.pshMatch.selfPosIndex = player.posIndex;
                player.token.SendMsg(msg);
            }
        }

    }

    public int GameReady(int id)//返回准备者的索引号用于给房间中所有玩家推送准备消息
    {
        int posIndex = -1;
        for (int i = 0;i< matchPlayerArr.Length; i++)
        {
            if(matchPlayerArr[i]!= null) {
                if (id == matchPlayerArr[i].id&&matchPlayerArr[i].isReady == false)
                {
                    matchPlayerArr[i].isReady = true;
                    posIndex = i;
                    readyNumber++;
                }
            }

        }
        return posIndex;
    }

    public int CancelReady(int id)//取消准备
    {
        int posIndex = -1;
        for (int i = 0; i < matchPlayerArr.Length; i++)
        {
            if (matchPlayerArr[i] != null)
            {
                if (id == matchPlayerArr[i].id && matchPlayerArr[i].isReady == true)
                {
                    matchPlayerArr[i].isReady = false;
                    posIndex = i;
                    readyNumber--;
                }
            }

        }
        return posIndex;
    }
    public bool AllReady()
    {
        this.ColorLog(PEUtils.LogColor.Yellow, "ReadyNumber:{0}", readyNumber);
        if(readyNumber == playerArr.Length - 1)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    //TOOL METHONDS
    public MessagePlayer[] GetMessagePlayers()
    {
        return playerArr;
    }

    
    
}

public enum RoomState
{
    None,
    Created,
    Matching,
    Matched,
    CallLord,
    RobLord,
    AddTimes,
    OutPokers,
    End
}
