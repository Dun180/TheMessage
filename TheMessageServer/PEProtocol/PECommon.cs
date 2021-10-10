using PEProtocol;
using System;
using System.Collections.Generic;
using System.Text;


public class PECommon
{
    public static int GetLevelUpExpCount(int level)
    {
        return 100 * level;
    }

    public static bool PlayStageUsability(Card card)
    {
        bool flag = false;
        if (card.function == CardFunction.Locking) flag = true;
        if (card.function == CardFunction.ProbingLurker_0) flag = true;
        if (card.function == CardFunction.ProbingLurker_1) flag = true;
        if (card.function == CardFunction.ProbingMilitary_0) flag = true;
        if (card.function == CardFunction.ProbingMilitary_1) flag = true;
        if (card.function == CardFunction.ProbingSoySauce_0) flag = true;
        if (card.function == CardFunction.ProbingSoySauce_1) flag = true;
        if (card.function == CardFunction.Reinforce) flag = true;
        if (card.function == CardFunction.RealOrFalse) flag = true;
        if (card.function == CardFunction.Burn) flag = true;
        if (card.function == CardFunction.Gambling) flag = true;
        if (card.function == CardFunction.Balance) flag = true;


        return flag;
    }

    public static bool ResponseStageUsability(Card card)
    {
        bool flag = false;
        if (card.function == CardFunction.Penetrate) flag = true;
        return flag;
    }

    public static bool TransferSectionUsability(Card card)
    {
        bool flag = false;

        if (card.function == CardFunction.Locking) flag = true;
        if (card.function == CardFunction.TigerMountain) flag = true;
        if (card.function == CardFunction.Swap) flag = true;
        if (card.function == CardFunction.Intercept) flag = true;
        if (card.function == CardFunction.Burn) flag = true;


        return flag;


    }

    public static bool ArriveSectionUsability(Card card)
    {
        bool flag = false;

        if (card.function == CardFunction.Decipher) flag = true;
        if (card.function == CardFunction.Transfer) flag = true;
        if (card.function == CardFunction.Burn) flag = true;


        return flag;

    }
}

