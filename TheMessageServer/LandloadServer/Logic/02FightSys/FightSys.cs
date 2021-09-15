//战斗系统
using PEProtocol;
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

    public void GameStart(MessageRoom messageRoom)
    {
        messageRoom.GameStart();//战斗房间初始化
        DistributionRole(messageRoom);
    }

    //分发角色牌
    public void DistributionRole(MessageRoom messageRoom)
    {
        //生成随机数组
        List<int> list = new List<int>();
        Random rand = new Random();
        int k = 0;
        do
        {
            k = rand.Next(1, 26);
            if (!list.Contains(k))
                list.Add(k);
        }
        while (list.Count < 20);

        //分发角色牌索引
        MessagePlayer[] playerArr = messageRoom.playerArr;

        for (int i = 0; i < playerArr.Length; i++)
        {
            GameMsg msg = new GameMsg
            {
                cmd = CMD.PushChar,
                pushChar = new PushChar
                {
                    char_1 = list[i * 3],
                    char_2 = list[i * 3 + 1],
                    char_3 = list[i * 3 + 2]
                }
            };
            playerArr[i].token.SendMsg(msg);
        }



    }

}

