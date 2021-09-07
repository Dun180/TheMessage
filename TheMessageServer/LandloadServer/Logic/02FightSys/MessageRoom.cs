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
    public MatchPlayerData[] matchPlayerArr = null;

    public int roomNumber { private set; get; }


    public MessageRoom(int roomID,string roomOwner)
    {
        RoomID = roomID;
        this.roomOwner = roomOwner;
        playerArr = new MessagePlayer[5];
        roomState = RoomState.Matching;
        matchPlayerArr = new MatchPlayerData[5];
        roomNumber = 0;
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

    public int GameReady(int id)
    {
        int posIndex = -1;
        for (int i = 0;i< matchPlayerArr.Length; i++)
        {
            if(matchPlayerArr[i]!= null) {
                if (id == matchPlayerArr[i].id)
                {
                    matchPlayerArr[i].isReady = true;
                    posIndex = i;
                }
            }

        }
        return posIndex;
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
