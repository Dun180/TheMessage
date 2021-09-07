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
            RestrictedMessage = 1,
            NonstopMessage = 2,
            TextMessage = 3
        }

        public enum CardFunction
        {

        }
    }
}
