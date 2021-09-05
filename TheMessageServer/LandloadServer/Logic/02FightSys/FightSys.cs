//战斗系统
using System;
using System.Collections.Generic;
using System.Text;


public class FightSys
{
    private static FightSys instance = null;
    public static FightSys Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new FightSys();

            }
            return instance;
        }
    }
    private CacheSvc cacheSvc;
    public void Init()
    {
        cacheSvc = CacheSvc.Instance;
    }

    public void Update()
    {

    }

    public void AddFightRoom(PokerRoom pokerRoom)
    {
        InitPokerRoom();

        PokerPlayer[] pokerPlayers = pokerRoom.GetPokerPlayers();

        for(int i = 0; i < pokerPlayers.Length; i++)
        {
            cacheSvc.AddTokenRoomDic(pokerPlayers[i].token, pokerRoom.RoomID);
        }

        //cacheSvc.AddIDRoomDic(pokerRoom);
    }
    //战斗房间初始化
    void InitPokerRoom()
    {
       
    }
}

