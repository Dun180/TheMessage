//风声玩家

using PEProtocol;
using System;
using System.Collections.Generic;
using System.Text;


public class MessagePlayer
{
    public ServerToken token;
    public int id;
    public string name;



    public int posIndex;//位置索引号
    public int iconIndex;//头像图片索引


    public List<Card> cardList = new List<Card>();//手牌


    public List<Card> messages = new List<Card>();//所持情报

    public int redNum;//红情报数
    public int blueNum;//蓝情报数
    public int blackNum;//黑情报数

    public int charIndex;//人物对应索引
    public string charName;//人物名字

    public PlayerState playerState;
    public PlayerIdentity playerIdentity;

    public void InitMatch()
    {
        redNum = 0;
        blueNum = 0;
        blackNum = 0;
        charIndex = 0;
        charName = null;
        cardList.Clear();
        messages.Clear();
        playerState = PlayerState.None;
        playerIdentity = PlayerIdentity.None;
    }

    public void AddCard(Card card)
    {
        cardList.Add(card);
    }


    public void SetIdentity(int index)
    {
        switch (index)
        {
            case 1:
                playerIdentity = PlayerIdentity.Lurker;
                break;
            case 2:
                playerIdentity = PlayerIdentity.Lurker;
                break;
            case 3:
                playerIdentity = PlayerIdentity.Military;
                break;
            case 4:
                playerIdentity = PlayerIdentity.Military;
                break;
            case 5:
                playerIdentity = PlayerIdentity.SoySauce;
                break;
            default:
                break;
        }
    }
}

public enum PlayerState
{
    None = 0,
    Free,//自由态
    Locking,//锁定
    Transfer,//调离
    Intercept,//截获
    Receive //接收

}

public enum PlayerIdentity
{
    None = 0,
    Lurker,//潜伏
    Military,//军情
    SoySauce //酱油
}

