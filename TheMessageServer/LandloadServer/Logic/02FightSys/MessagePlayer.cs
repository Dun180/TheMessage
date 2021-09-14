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


    public List<Card> cards = new List<Card>();//手牌


    public List<Card> messages = new List<Card>();//所持情报

    public int redNum;//红情报数
    public int blueNum;//蓝情报数
    public int blackNum;//黑情报数

    public int charIndex;//人物对应索引
    public string charName;//人物名字

    public void InitMatch()
    {
        redNum = 0;
        blueNum = 0;
        blackNum = 0;
        charIndex = 0;
        charName = null;
        cards.Clear();
        messages.Clear();
    }

    public void AddCard(Card card)
    {
        cards.Add(card);
    }
}

