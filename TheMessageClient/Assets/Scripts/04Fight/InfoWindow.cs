//匹配信息界面
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PEProtocol;

public class InfoWindow : WindowRoot
{
    public Image leftChar;
    public Image rightChar;
    public Image selfChar;

    public Transform leftPlayer;
    public Image leftLordIcon;
    public Text leftName;
    public Text leftCoin;


    public Transform rightPlayer;
    public Image rightLordIcon;
    public Text rightName;
    public Text rightCoin;

    public Transform selfPlayer;
    public Image selfLordIcon;
    public Text selfName;
    public Text selfCoin;

    public Text txtMatching;
    public Text txtAddTimes;
    public Text txtBaseScore;

    private FightSys fightSys;
    private AudioSvc audioSvc;

    public override void InitWindow()
    {
        base.InitWindow();

        fightSys = FightSys.Instance;
        audioSvc = AudioSvc.Instance;

        SetActive(selfChar, false);
        SetActive(leftChar, false);
        SetActive(rightChar, false);

        SetActive(selfLordIcon, false);
        SetActive(leftLordIcon, false);
        SetActive(rightLordIcon, false);

        SetActive(selfPlayer, false);
        SetActive(leftPlayer, false);
        SetActive(rightPlayer, false);

        SetActive(txtBaseScore, false);
        SetAddTimes();

        SetMatchingState(true);
    }

    public void RefreshWindow()
    {
        MatchPlayerData selfPlayerData = fightSys.SelfPlayerData;
        MatchPlayerData leftPlayerData = fightSys.LeftPlayerData;
        MatchPlayerData rightPlayerData = fightSys.RightPlayerData;

        //string path = "ResImages/Fight/Char_";
        if(selfPlayerData != null)
        {
            SetActive(selfPlayer);
            SetText(selfName, selfPlayerData.name);
            SetActive(selfChar);
           // SetSprite(selfChar, path + selfPlayerData.iconIndex);
        }
        if (leftPlayerData != null)
        {
            SetActive(leftPlayer);
            SetText(leftName, leftPlayerData.name);
            SetActive(leftChar);
           // SetSprite(leftChar, path + leftPlayerData.iconIndex);
        }
        if (rightPlayerData != null)
        {
            SetActive(rightPlayer);
            SetText(rightName, rightPlayerData.name);
            SetActive(rightChar);
           // SetSprite(rightChar, path + rightPlayerData.iconIndex);
        }
    }

    public void SetAddTimes()
    {
        SetText(txtAddTimes,fightSys.AddTimes);
    }

    public void SetMatchingState(bool state)
    {
        SetActive(txtMatching, state);
    }

    public void ClickSetBtn()
    {
        audioSvc.PlayUIAudio(Constants.NormalClick);
        TipsWindow.AddTips("正在开发中");
    }

    public void ClickAutoBtn()
    {
        audioSvc.PlayUIAudio(Constants.NormalClick);
        TipsWindow.AddTips("正在开发中");
    }

    public void ClickBackBtn()
    {
        audioSvc.PlayUIAudio(Constants.NormalClick);
        TipsWindow.AddTips("正在开发中");
    }
}
