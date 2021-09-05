//对战房间



using PEProtocol;
using System;
using System.Collections.Generic;
using System.Text;


public class PokerRoom
{
    public int RoomID { private set; get; }
    public PokerPlayer[] playerArr = null;
    public RoomState roomState = RoomState.None;

    private MatchPlayerData[] matchPlayerArr = null;


    public PokerRoom(int roomID) {
        RoomID = roomID;
        playerArr = new PokerPlayer[3];
        roomState = RoomState.Matching;
        matchPlayerArr = new MatchPlayerData[3];
    }


    public void AddPokerPlayer(PokerPlayer pokerPlayer,int posIndex)
    {
        playerArr[posIndex] = pokerPlayer;
        MatchPlayerData matchPlayerData = new MatchPlayerData
        {
            id = pokerPlayer.id,
            name = pokerPlayer.name,
            //coin = pokerPlayer.coin,
            posIndex = pokerPlayer.posIndex,

        };

        matchPlayerArr[posIndex] = matchPlayerData;

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

        for(int i = 0; i < playerArr.Length; i++)
        {
            PokerPlayer player = playerArr[i];
            if(player != null)
            {
                msg.pshMatch.selfPosIndex = player.posIndex;
                player.token.SendMsg(msg);
            }
        }

    }

    //TOOL METHONDS
    public PokerPlayer[] GetPokerPlayers()
    {
        return playerArr;
    }
}


