using System;
using System.Collections.Generic;
using System.Text;

namespace PEProtocol
{
    public class Card
    {
        public CardColor color;
        public CardType type;
        public CardFunction function;

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
            Probing,//试探
            Reinforce,//增援

            Swap,//调包
            RealOrFalse,//真伪莫辨
            PublicDocument,//公开文档

            Intercept,//截获
            Transfer,//转移
            Burn,//烧毁
            Penetrate,//识破
            Gambling,//博弈
            Balance //权衡









        }
    }
}
