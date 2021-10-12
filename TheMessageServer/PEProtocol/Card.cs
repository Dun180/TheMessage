using System;
using System.Collections.Generic;
using System.Text;

namespace PEProtocol
{
    [Serializable]
    public class Card
    {
        public CardColor color;
        public CardType type;
        public CardFunction function;

        public bool Equals(Card card)
        {
            return card.color.Equals(color) && card.type.Equals(type) && card.function.Equals(function);
        }
    }

    public enum CardColor
    {
        None = 0,
        Red = 1,
        Blue = 2,
        Black = 3,
        RedBlack = 4,
        BlueBlack = 5
    }

    public enum CardType
    {
        None = 0,
        RestrictedMessage = 1,//密电
        NonstopMessage = 2,//直达
        TextMessage = 3 //文本
    }

    public enum CardFunction
    {
        None = 0,
        Locking,//锁定
        TigerMountain,//调虎离山
        Decipher,//破译

        ProbingLurker_0,//试探潜伏-1（完成）
        ProbingLurker_1,//试探潜伏+1（完成）
        ProbingMilitary_0,//试探军情-1（完成）
        ProbingMilitary_1,//试探军情+1（完成）
        ProbingSoySauce_0,//试探酱油-1（完成）
        ProbingSoySauce_1,//试探酱油+1（完成）
        Reinforce,//增援（完成）

        Swap,//调包
        RealOrFalse,//真伪莫辨
        PublicDocument,//公开文档

        Intercept,//截获
        Transfer,//转移

        Burn,//烧毁（完成）
        Penetrate,//识破（完成）
        Gambling,//博弈（完成）
        Balance //权衡









    }


}
