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
        

        
    }








}
