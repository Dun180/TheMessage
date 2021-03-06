//战斗界面


using DG.Tweening;
using PEProtocol;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum MessageStage
{
    None,
    SelectChar,
    RoundStart,//回合开始阶段
    DrawPhase,//抽牌阶段
    PlayStage,//出牌阶段
    ResponseStage,//响应阶段
    ResponseWaitStage,//响应等待阶段
    DisCardStage,//弃牌阶段
    BalanceStage,//权衡阶段
    MessageTransfer,//情报传递阶段
    TransferSection,//传递小节
    ArriveSection,//到达小节
    AcceptSection,//接受小节
    RoundEnd //回合结束阶段
}

public class MessageWindow : WindowRoot
{

    MessageStage messageStage = MessageStage.None;

    public RectTransform[] player;

    public Image[] charImg;

    public Text[] charName;
    public Text[] playerName;
    public Text[] cards;
    public Text[] redNum;
    public Text[] blueNum;
    public Text[] blackNum;
    public Text[] playerState;

    public RectTransform[] messageClickTrans;//情报点击区域

    public Text btn1txt;
    public Image btn1bg;
    public Text btn2txt;
    public Image btn2bg;


    public Text selfIdentity;

    public Image selectionBox;
    public Image char_1;
    public Image char_2;
    public Image char_3;

    public Image turnIdentification;

    public Transform selfCardTrans;//手牌
    public Transform btnGroupTrans;//按钮组
    public Transform messageTrans;//情报传播区
    public Transform ExhibitioAreaTrans;//卡牌展示区
    public RectTransform messageRegionTrans;//情报展示区

    public Material outLineMaterial;

    public Text txtTimer;
    public Text cardLibrary;

    private AudioSvc audioSvc;
    private NetSvc netSvc;

    public int selfIndex { private set; get; }
    private int charIndex;
    private int[] charList;

    private List<CardEntity> selfCardEntityList = new List<CardEntity>();

    private List<CardEntity> outCardEntityList = new List<CardEntity>();

    private List<CardEntity> showCardEntityList = new List<CardEntity>();

    private List<CardEntity> prepareBalanceEntityList = new List<CardEntity>();


    private CardEntity prepareEntity = null;//准备使用的卡牌实体
    private GameObject cardObj = null;

    private CardEntity transEntity = null;//传递中的情报实体
    private bool isMyTurn = false;//判断是否是自己的回合
    private bool isTransMsg = false;//判断是否已经传递过情报

    private int handCardNum;

    public RectTransform trail;

    public RectTransform probingTrans;
    public Text probingTxt;

    private bool isMyDisCard = false;//判断是否是自己的弃牌阶段

    private bool isMessageClick = false;//判断是否需要给情报挂载点击事件

    private int showMessageRegionIndex = -1;//当前展示的情报区索引

    private bool isLocking = false;//是否被锁定

    private bool isTigerMountain = false;//是否被调虎离山

    private int messagePositionIndex = -1;

    public override void InitWindow()
    {
        base.InitWindow();
        audioSvc = AudioSvc.Instance;
        netSvc = NetSvc.Instance;

        SetActive(selectionBox, false);
        SetActive(turnIdentification, false);
        handCardNum = 0;
        selfIndex = -1;
        charIndex = -1;
        charList = new int[3];
        SetText(selfIdentity, "");
        //重置数据
        for (int i = 0; i < Constants.fivePersonFieldCount; i++)
        {
            SetText(cards[i], "4");
            SetText(redNum[i], "0");
            SetText(blueNum[i], "0");
            SetText(blackNum[i], "0");
            SetActive(playerState[i],false);
        }

        SetText(cardLibrary, "93");



        //清理上局留下的残留牌
        selfCardEntityList.Clear();

        for (int i = 0; i < selfCardTrans.childCount; i++)
        {
            Destroy(selfCardTrans.GetChild(i).gameObject);
        }

        //加载资源中的UI
        cardObj = Resources.Load("UIWindow/CardItem") as GameObject;
        selfCardTrans.localPosition = new Vector3(-580, -380, 0);


        //创建新的牌
        for (int i = 0; i < 4; i++)
        {
            GameObject go = Instantiate(cardObj);
            RectTransform rectTrans = go.GetComponent<RectTransform>();
            rectTrans.SetParent(selfCardTrans);
            rectTrans.sizeDelta = new Vector2(170, 240);
            rectTrans.localScale = Vector3.one;
            go.name = "msg_" + i;


            OnClickUp(rectTrans.gameObject, OnCardItemSelected);


            CardEntity cardEntity = new CardEntity(rectTrans, i);

            if (i != 0)
            {
                cardEntity.SetRectPos(new Vector3(Constants.cardDistance * (i - 1), 0, 0));
            }
            else
            {
                cardEntity.SetRectPos(Vector3.zero);
            }
            SetActive(rectTrans, false);
            selfCardEntityList.Add(cardEntity);
            handCardNum++;

        }

        for (int i = 0; i < Constants.fivePersonFieldCount; i++)
        {
            OnClickUp(messageClickTrans[i].gameObject, OnMessageRegionClicked);
        }
    }

    public void SetMessageStage(MessageStage state)
    {
        messageStage = state;
        SetActive(btnGroupTrans);
        StopClock(txtTimer.gameObject);
        switch (messageStage)
        {
            case MessageStage.SelectChar:
                SetActive(btn1bg.transform, false);
                SetActive(btn2bg.transform, false);
                break;
            case MessageStage.PlayStage:
                TipsWindow.AddTips("出牌阶段");

                if (isMyTurn)
                {
                    SetActive(btn1bg.transform);
                    SetBtnInfo(btn1bg, btn1txt, "使用", 1, new Vector2(-175, -100));


                    SetActive(btn2bg.transform);
                    SetBtnInfo(btn2bg, btn2txt, "跳过", 2, new Vector2(200, -100));


                    btnGroupTrans.localPosition = new Vector3(0, -75, 0);
                }
                else
                {
                    SetActive(btn1bg.transform);
                    SetBtnInfo(btn1bg, btn1txt, "使用", 1, new Vector2(-50, -100));

                    SetActive(btn2bg.transform, false);
                }
                SetClockCallBack(Constants.playStageCounter, ClickBtn2);
                break;
            case MessageStage.ResponseStage:
                TipsWindow.AddTips("响应阶段");

                SetActive(btn1bg.transform);
                SetBtnInfo(btn1bg, btn1txt, "使用", 1, new Vector2(-175, -100));

                SetActive(btn2bg.transform);
                SetBtnInfo(btn2bg, btn2txt, "跳过", 2, new Vector2(200, -100));

                btnGroupTrans.localPosition = new Vector3(0, -75, 0);

                SetClockCallBack(Constants.responseStageCounter, ClickBtn2);


                break;
            case MessageStage.ResponseWaitStage:
                TipsWindow.AddTips("响应阶段");

                SetActive(btnGroupTrans, false);

                SetClockCallBack(Constants.responseStageCounter, ClickBtn1);

                break;
            case MessageStage.DisCardStage:
                if (isMyDisCard)
                {
                    TipsWindow.AddTips("请选择一张牌进行丢弃");
                    SetActive(btn1bg.transform);
                    SetBtnInfo(btn1bg, btn1txt, "弃牌", 1, new Vector2(-50, -100));

                    SetActive(btn2bg.transform, false);
                }
                else
                {
                    TipsWindow.AddTips("请等待玩家弃牌");

                    SetActive(btn1bg.transform, false);
                    SetActive(btn2bg.transform, false);
                }
                break;
            case MessageStage.BalanceStage:
                if (isMyTurn)
                {
                    TipsWindow.AddTips("请选择你要权衡的牌");
                    SetActive(btn1bg.transform);
                    SetBtnInfo(btn1bg, btn1txt, "权衡", 1, new Vector2(-50, -100));

                    SetActive(btn2bg.transform, false);

                    ChangeCardClickEvent(OnCardItemListSelected);
                }
                else
                {
                    TipsWindow.AddTips("请等待玩家权衡");

                    SetActive(btn1bg.transform, false);
                    SetActive(btn2bg.transform, false);
                }
                break;
            case MessageStage.MessageTransfer:
                TipsWindow.AddTips("情报传递阶段");

                isTransMsg = false;
                if (isMyTurn)
                {
                    SetActive(btn1bg.transform);
                    SetBtnInfo(btn1bg, btn1txt, "传递情报", 1, new Vector2(-50, -100));

                    SetActive(btn2bg.transform, false);
                }
                else
                {
                    SetActive(btn1bg.transform, false);
                    SetActive(btn2bg.transform, false);
                }

                //SetClockCallBack(Constants.messageTransferCounter, ClickBtn1);
                break;
            case MessageStage.ArriveSection:
                TipsWindow.AddTips("情报到达阶段");

                SetActive(btn1bg.transform);
                SetBtnInfo(btn1bg, btn1txt, "使用", 1, new Vector2(-175, -100));

                SetActive(btn2bg.transform);
                SetBtnInfo(btn2bg, btn2txt, "跳过", 2, new Vector2(200, -100));

                btnGroupTrans.localPosition = new Vector3(0, -75, 0);

                if (isMyTurn)
                {
                    SetClockCallBack(Constants.arriveSectionCounter, EndArriveSection);

                }
                else
                {
                    SetClockCallBack(Constants.arriveSectionCounter, ClickBtn2);

                }
                break;
            case MessageStage.AcceptSection:
                TipsWindow.AddTips("情报接收阶段");

                if (isLocking)
                {
                    ClickBtn1();
                }else if (isTigerMountain)
                {
                    ClickBtn2();
                }
                else
                {
                    TipsWindow.AddTips("请选择是否接收情报");
                    SetActive(btn1bg.transform);
                    SetBtnInfo(btn1bg, btn1txt, "要", 1, new Vector2(-175, -100));

                    SetActive(btn2bg.transform);
                    SetBtnInfo(btn2bg, btn2txt, "不要", 2, new Vector2(200, -100));
                }

                break;
            default:
                break;
        }

    }

    void SetBtnInfo(Image img, Text txt, string info, int index, Vector2 pos)
    {
        if (!img.IsActive())
        {
            SetActive(img.transform);
        }
        img.transform.localPosition = pos;
        SetSprite(img, "ResImages/Common/btnBG" + index);
        Vector2 size = img.rectTransform.sizeDelta;
        img.rectTransform.sizeDelta = new Vector2(info.Length * 45 + 60, size.y);
        SetText(txt, info);
    }

    public void ClickChar1Btn()
    {
        audioSvc.PlayUIAudio(Constants.NormalClick);

        if (charIndex != charList[0])
        {
            charIndex = charList[0];
            char_1.material = outLineMaterial;
            char_2.material = null;
            char_3.material = null;
        }
        else
        {
            charIndex = -1;
            char_1.material = null;
        }

    }

    public void ClickChar2Btn()
    {
        audioSvc.PlayUIAudio(Constants.NormalClick);

        if (charIndex != charList[1])
        {
            charIndex = charList[1];
            char_1.material = null;
            char_2.material = outLineMaterial;
            char_3.material = null;
        }
        else
        {
            charIndex = -1;
            char_2.material = null;
        }

    }

    public void ClickChar3Btn()
    {
        audioSvc.PlayUIAudio(Constants.NormalClick);

        if (charIndex != charList[2])
        {
            charIndex = charList[2];
            char_1.material = null;
            char_2.material = null;
            char_3.material = outLineMaterial;
        }
        else
        {
            charIndex = -1;
            char_3.material = null;
        }

    }

    public void ClickBtn1()
    {
        switch (messageStage)
        {
            case MessageStage.SelectChar:
                break;
            case MessageStage.PlayStage:
                if (isMyTurn)
                {
                    if (PECommon.PlayStageUsability(prepareEntity.cardData))
                    {

                        switch (prepareEntity.cardData.function)
                        {
                            case CardFunction.Locking:
                            case CardFunction.ProbingLurker_0:
                            case CardFunction.ProbingLurker_1:
                            case CardFunction.ProbingMilitary_0:
                            case CardFunction.ProbingMilitary_1:
                            case CardFunction.ProbingSoySauce_0:
                            case CardFunction.ProbingSoySauce_1:
                            case CardFunction.Gambling:
                                TipsWindow.AddTips("请点击人物头像以选择目标");
                                AddPlayerClickEvent();
                                break;
                            case CardFunction.Burn:
                                //TODO
                                TipsWindow.AddTips("请点击情报以选择目标");

                                AddMessageClickEvent();
                                break;
                            default:

                                GameMsg outCardMsg = new GameMsg
                                {
                                    cmd = CMD.RequestOutCard,
                                    requestOutCard = new RequestOutCard { card = prepareEntity.cardData }
                                };
                                netSvc.SendMsg(outCardMsg);
                                OutCardAni();
                                break;
                        }

                        SetActive(btnGroupTrans, false);
                    }
                    else
                    {
                        TipsWindow.AddTips("暂时无法使用该牌");
                    }
                }
                else
                {
                    if (prepareEntity.cardData.function == CardFunction.Burn)
                    {
                        TipsWindow.AddTips("请点击情报以选择目标");

                        AddMessageClickEvent();
                    }
                    else
                    {
                        TipsWindow.AddTips("暂时无法使用该牌");
                    }
                }



                break;
            case MessageStage.ResponseStage:
                if (PECommon.ResponseStageUsability(prepareEntity.cardData))
                {
                    GameMsg outCardMsg = new GameMsg
                    {
                        cmd = CMD.RequestOutCard,
                        requestOutCard = new RequestOutCard { card = prepareEntity.cardData }
                    };
                    netSvc.SendMsg(outCardMsg);
                    SetActive(btnGroupTrans, false);
                    OutCardAni();
                }
                else
                {
                    TipsWindow.AddTips("暂时无法使用该牌");
                }
                break;
            case MessageStage.ResponseWaitStage:
                //发送响应阶段结束的信息
                GameMsg endResMsg = new GameMsg
                {
                    cmd = CMD.RequestEndResponseStage
                };
                netSvc.SendMsg(endResMsg);
                break;
            case MessageStage.DisCardStage:
                if (prepareEntity != null)
                {
                    GameMsg disCardMsg = new GameMsg
                    {
                        cmd = CMD.RequestDisCard,
                        requestDisCard = new RequestDisCard
                        {
                            disCard = prepareEntity.cardData
                        }
                    };
                    netSvc.SendMsg(disCardMsg);
                    SetActive(btnGroupTrans, false);
                    isMyDisCard = false;
                    OutCardAni();

                }

                break;
            case MessageStage.BalanceStage:


                List<Card> tempList = new List<Card>();
                for(int i = 0; i < prepareBalanceEntityList.Count; i++)
                {
                    tempList.Add(prepareBalanceEntityList[i].cardData);
                }

                GameMsg balanceMsg = new GameMsg
                {
                    cmd = CMD.RequestBalanceInfo,
                    requestBalanceInfo = new RequestBalanceInfo
                    {
                        cardList = tempList
                    }
                };
                netSvc.SendMsg(balanceMsg);
                SetActive(btnGroupTrans, false);
                OutCardListAni();
                ChangeCardClickEvent(OnCardItemSelected);

                break;
            case MessageStage.MessageTransfer:
                if (prepareEntity != null)
                {
                    if (prepareEntity.cardData.type == CardType.RestrictedMessage || prepareEntity.cardData.type == CardType.TextMessage)
                    {
                        if (!isTransMsg)
                        {
                            GameMsg transMsg = new GameMsg
                            {
                                cmd = CMD.RequestMessageTransfer,
                                requestMessageTransfer = new RequestMessageTransfer { message = prepareEntity.cardData }
                            };
                            netSvc.SendMsg(transMsg);
                            isTransMsg = true;
                            SetActive(btnGroupTrans, false);
                            OutCardAni();

                        }

                    }
                    else
                    {
                        if (!isTransMsg)
                        {
                            isTransMsg = true;

                            TipsWindow.AddTips("请点击人物头像以选择目标");

                            AddPlayerClickEvent();


                            SetActive(btnGroupTrans, false);

                        }
                    }

                }
                break;
            case MessageStage.ArriveSection:
                if (prepareEntity != null)
                {
                    this.Log("messagePositionIndex:{0}", messagePositionIndex);
                    if (prepareEntity.cardData.function == CardFunction.Intercept && isMyTurn)
                    {
                        TipsWindow.AddTips("暂时无法使用该牌");
                    }
                    else if (prepareEntity.cardData.function == CardFunction.Transfer && selfIndex!=messagePositionIndex)
                    {
                        this.Log("messagePositionIndex:{0}", messagePositionIndex);
                        TipsWindow.AddTips("暂时无法使用该牌");
                    }
                    else if (prepareEntity.cardData.function == CardFunction.Decipher && selfIndex != messagePositionIndex)
                    {
                        TipsWindow.AddTips("暂时无法使用该牌");
                    }
                    else if (PECommon.ArriveSectionUsability(prepareEntity.cardData))
                    {
                        switch (prepareEntity.cardData.function)
                        {
                            case CardFunction.Locking:
                            case CardFunction.TigerMountain:
                            case CardFunction.Transfer:
                                TipsWindow.AddTips("请点击人物头像以选择目标");
                                AddPlayerClickEvent();
                                break;
                            case CardFunction.Burn:
                                //TODO
                                TipsWindow.AddTips("请点击情报以选择目标");

                                AddMessageClickEvent();
                                break;
                            case CardFunction.Intercept:
                                GameMsg interceptMsg = new GameMsg
                                {
                                    cmd = CMD.RequestOutCard,
                                    requestOutCard = new RequestOutCard { card = prepareEntity.cardData }
                                };
                                netSvc.SendMsg(interceptMsg);
                                OutCardAni();

                                break;

                            default:

                                GameMsg outCardMsg = new GameMsg
                                {
                                    cmd = CMD.RequestOutCard,
                                    requestOutCard = new RequestOutCard { card = prepareEntity.cardData }
                                };
                                netSvc.SendMsg(outCardMsg);
                                OutCardAni();
                                break;
                        }

                        SetActive(btnGroupTrans, false);
                    }
                    else
                    {
                        TipsWindow.AddTips("暂时无法使用该牌");
                    }
                }
                break;
            case MessageStage.AcceptSection:
                GameMsg acceptMsg = new GameMsg
                {
                    cmd = CMD.RequestAcceptMessage,
                    requestAcceptMessage = new RequestAcceptMessage
                    {
                        isAccept = true
                    }
                };
                netSvc.SendMsg(acceptMsg);
                SetActive(btnGroupTrans, false);

                break;

            default:
                break;
        }
    }
    public void ClickBtn2()
    {
        switch (messageStage)
        {
            case MessageStage.SelectChar:
                break;
            case MessageStage.PlayStage:
                if (isMyTurn)
                {
                    GameMsg msg = new GameMsg
                    {
                        cmd = CMD.RequestEndPlay
                    };
                    netSvc.SendMsg(msg);
                }
                break;
            case MessageStage.ResponseStage:

                SetActive(btnGroupTrans, false);

                break;
            case MessageStage.ArriveSection:

                SetActive(btnGroupTrans, false);

                break;
            case MessageStage.AcceptSection:
                GameMsg acceptMsg = new GameMsg
                {
                    cmd = CMD.RequestAcceptMessage,
                    requestAcceptMessage = new RequestAcceptMessage
                    {
                        isAccept = false
                    }
                };
                netSvc.SendMsg(acceptMsg);
                SetActive(btnGroupTrans, false);

                break;
            default:
                break;
        }
    }
    public void ClickConfirmCharBtn()
    {
        audioSvc.PlayUIAudio(Constants.NormalClick);
        if (charIndex < 0)
        {
            TipsWindow.AddTips("请选择人物");
        }
        else
        {
            StopClock(txtTimer.gameObject);
            selectionBox.gameObject.SetActive(false);

            GameMsg msg = new GameMsg
            {
                cmd = CMD.RequestSelectChar,
                requestSelectChar = new RequestSelectChar { charIndex = this.charIndex }
            };
            netSvc.SendMsg(msg);
            TipsWindow.AddTips("请等待其他玩家选择");
        }
    }

    public void ClickCloseMessageRegion()
    {
        SetActive(messageRegionTrans, false);
        showMessageRegionIndex = -1;
        for (int i = showCardEntityList.Count - 1; i >= 0; i--)
        {
            CardEntity cardEntity = showCardEntityList[i];
            showCardEntityList.RemoveAt(i);
            Destroy(cardEntity.mRectTrans.gameObject);
        }
    }


    public void SetCardLibrary(int count)
    {
        SetText(cardLibrary, count);
    }

    private Vector3[] turnIdTransPos = new Vector3[5]
{
        new Vector3(-776,-211,0),
        new Vector3(909,148,0),
        new Vector3(377,490,0),
        new Vector3(-216,490,0),
        new Vector3(-780,305,0)
};

    public void SetTurnIdentification(int index)
    {
        int flag = 0 - (selfIndex - index);
        if (flag >= 5)
        {
            flag -= 5;
        }
        else if (flag < 0)
        {
            flag += 5;
        }
        SetActive(turnIdentification, true);
        turnIdentification.transform.localPosition = turnIdTransPos[flag];
    }

    public void RefreshMessage(int index, MatchPlayerData[] matchplayer)
    {
        string path = "ResImages/Char/Char_";
        if (selfIndex < 0)
        {
            selfIndex = index;
        }

        for (int i = 0; i < matchplayer.Length; i++)
        {
            int flag = i + selfIndex;
            if (flag >= matchplayer.Length)
            {
                flag -= matchplayer.Length;
            }

            SetSprite(charImg[i], path + matchplayer[flag].charIndex);
            SetText(playerName[i], matchplayer[flag].name);
            SetText(charName[i], matchplayer[flag].charName);
            SetText(cards[i], matchplayer[flag].cards);
            SetText(redNum[i], matchplayer[flag].redNum);
            SetText(blueNum[i], matchplayer[flag].blueNum);
            SetText(blackNum[i], matchplayer[flag].blackNum);
        }

    }

    public void SelectChar(PushChar pushChar)
    {
        this.Log(pushChar.char_1.ToString() + "+" + pushChar.char_2.ToString() + "+" + pushChar.char_3.ToString());
        selectionBox.gameObject.SetActive(true);
        string path = "ResImages/Char/Char_";
        charList[0] = pushChar.char_1;
        charList[1] = pushChar.char_2;
        charList[2] = pushChar.char_3;
        SetSprite(char_1, path + pushChar.char_1.ToString());
        SetSprite(char_2, path + pushChar.char_2.ToString());
        SetSprite(char_3, path + pushChar.char_3.ToString());

        SetClockCallBack(30, PushDefaultChar);


    }

    public void PushDefaultChar()
    {
        if (charIndex < 0)
        {
            charIndex = charList[0];
            selectionBox.gameObject.SetActive(false);

            GameMsg msg = new GameMsg
            {
                cmd = CMD.RequestSelectChar,
                requestSelectChar = new RequestSelectChar { charIndex = this.charIndex }
            };
            netSvc.SendMsg(msg);
        }

    }

    public void SetIdentity(string identity)
    {
        SetText(selfIdentity, identity);

        StartCoroutine(DelayMoveCard());

    }

    public void AddCard(Card data)
    {
        GameObject go = Instantiate(cardObj);
        RectTransform rectTrans = go.GetComponent<RectTransform>();
        rectTrans.SetParent(selfCardTrans);
        rectTrans.sizeDelta = new Vector2(170, 240);
        rectTrans.localScale = Vector3.one;
        go.name = "msg_" + handCardNum;

        OnClickUp(rectTrans.gameObject, OnCardItemSelected);

        CardEntity cardEntity = new CardEntity(rectTrans, handCardNum);

        cardEntity.SetRectPos(new Vector3(0, 0, 0));

        cardEntity.SetEntityData(data);
        SetActive(rectTrans, false);
        selfCardEntityList.Add(cardEntity);
        StartCoroutine(DrawMoveCard(cardEntity));
        handCardNum++;


    }

    public void SetAddMessageInfo(int posIndex, int addCards, int addRedNum, int addBlueNum, int addBlackNum)
    {
        int flag = 0 - (selfIndex - posIndex);
        if (flag >= 5)
        {
            flag -= 5;
        } else if (flag < 0)
        {
            flag += 5;
        }
        SetText(cards[flag], int.Parse(cards[flag].text) + addCards);
        SetText(redNum[flag], int.Parse(redNum[flag].text) + addRedNum);
        SetText(blueNum[flag], int.Parse(blueNum[flag].text) + addBlueNum);
        SetText(blackNum[flag], int.Parse(blackNum[flag].text) + addBlackNum);

    }

    public void SetMessageInfo(int posIndex, int cards, int redNum, int blueNum, int blackNum)
    {
        int flag = 0 - (selfIndex - posIndex);
        if (flag >= 5)
        {
            flag -= 5;
        }
        else if (flag < 0)
        {
            flag += 5;
        }
        SetText(this.cards[flag], cards);
        SetText(this.redNum[flag], redNum);
        SetText(this.blueNum[flag], blueNum);
        SetText(this.blackNum[flag], blackNum);

    }

    private Vector3[] acceptPos = new Vector3[5]
{
        new Vector3(-845,-340,0),
        new Vector3(837,42,0),
        new Vector3(309,394,0),
        new Vector3(-284,394,0),
        new Vector3(-847,183,0)
};

    public void ConfirmAcceptMessage(int index)
    {

        int targetPos = 0 - (selfIndex - index);
        if (targetPos >= 5)
        {
            targetPos -= 5;
        }
        else if (targetPos < 0)
        {
            targetPos += 5;
        }

        transEntity.turnOver();



        StartCoroutine(ConfirmAcceptMessageAni(targetPos));


    }

    IEnumerator ConfirmAcceptMessageAni(int index)
    {
        yield return new WaitForSeconds(Constants.acceptMessageTime);
        transEntity.MoveTargetPosInTime(Constants.messageMoveTime, acceptPos[index], null, Constants.acceptMessageSize);
        yield return new WaitForSeconds(Constants.messageMoveTime);

        Destroy(transEntity.mRectTrans.gameObject);
        transEntity = null;
    }

    public void EndArriveSection()
    {
        GameMsg msg = new GameMsg
        {
            cmd = CMD.RequestEndArriveSection
        };
        netSvc.SendMsg(msg);
    }

    public void endPlay()
    {
        StopClock(txtTimer.gameObject);
        SetMessageStage(MessageStage.MessageTransfer);
    }

    //密电情报传递动画
    public void MessageTransferAni(Card message, int posIndex, int targetIndex = -1)
    {
        GameObject go = Instantiate(cardObj);
        RectTransform rectTrans = go.GetComponent<RectTransform>();
        rectTrans.SetParent(messageTrans);
        rectTrans.sizeDelta = new Vector2(170, 240);
        rectTrans.localScale = Vector3.one;
        go.name = "transMsg";




        CardEntity cardEntity = new CardEntity(rectTrans, -1);
        cardEntity.SetEntityData(message, true);
        cardEntity.SetRectPos(new Vector3(0, 0, 0));

        transEntity = cardEntity;

        int actualPos = 0 - (selfIndex - posIndex);
        if (actualPos >= 5)
        {
            actualPos -= 5;
        }
        else if (actualPos < 0)
        {
            actualPos += 5;
        }

        int targetPos;
        if (targetIndex < 0)
        {
            targetPos = actualPos + 1;
            if (targetPos >= 5)
            {
                targetPos -= 5;
            }
            else if (targetPos < 0)
            {
                targetPos += 5;
            }
        }
        else
        {
            targetPos = 0 - (selfIndex - targetIndex);
            if (targetPos >= 5)
            {
                targetPos -= 5;
            }
            else if (targetPos < 0)
            {
                targetPos += 5;
            }
        }


        StartCoroutine(MessageTransferAni(transEntity, actualPos, targetPos));


    }
    //密电文本情报传递中动画
    public void MessageTransferingAni(int posIndex)
    {
        int targetPos = 0 - (selfIndex - posIndex);
        if (targetPos >= 5)
        {
            targetPos -= 5;
        }
        else if (targetPos < 0)
        {
            targetPos += 5;
        }
        int sendPos = targetPos - 1;
        if (sendPos >= 5)
        {
            sendPos -= 5;
        }
        else if (sendPos < 0)
        {
            sendPos += 5;
        }
        StartCoroutine(MessageTransferAni(transEntity, sendPos, targetPos));

    }
    //直达情报传递中动画
    public void NonstopMessageTransAni(int sendIndex, int targetIndex)
    {
        int targetPos = 0 - (selfIndex - targetIndex);
        if (targetPos >= 5)
        {
            targetPos -= 5;
        }
        else if (targetPos < 0)
        {
            targetPos += 5;
        }
        int sendPos = 0 - (selfIndex - sendIndex);
        if (sendPos >= 5)
        {
            sendPos -= 5;
        }
        else if (sendPos < 0)
        {
            sendPos += 5;
        }
        StartCoroutine(MessageTransferAni(transEntity, sendPos, targetPos));
    }

    //展示手牌
    public void ShowSelfCard(List<Card> cardList)
    {
        SetActive(selfCardTrans);

        for (int i = 0; i < cardList.Count; i++)
        {
            selfCardEntityList[i].SetEntityData(cardList[i]);
        }
    }

    //倒计时设置
    public void SetClockCallBack(int countTime, Action callback)
    {


        SetText(txtTimer, countTime);

        PETimerTask pTask = new PETimerTask
        {
            rateTime = 1,
            rateCB = () =>
            {
                countTime -= 1;
                SetText(txtTimer, countTime);

            },
            endTime = countTime,
            endCB = callback
        };
        PETimer pt = (PETimer)GetOrAddComponent<PETimer>(txtTimer.gameObject);
        pt.AddTimerTask(pTask);

    }

    //停止计时
    public void StopClock(GameObject gameObject)
    {

        PETimer pt = (PETimer)GetOrAddComponent<PETimer>(gameObject);
        pt.OnDisable();
        SetText(txtTimer, "0");

    }

    //发牌动画
    IEnumerator DelayMoveCard()
    {

        for (int i = 0; i < 4; i++)
        {
            CardEntity selfPoker = selfCardEntityList[i];
            SetActive(selfPoker.mRectTrans);
            if (i != 0)
            {
                selfPoker.MoveLocalPosInTime(Constants.moveTime, new Vector3(Constants.cardDistance, 0, 0));
            }
            yield return new WaitForSeconds(Constants.moveTime);
        }


    }

    //抽卡动画
    IEnumerator DrawMoveCard(CardEntity cardEntity)
    {

        if (handCardNum < 8)
        {
            SetActive(cardEntity.mRectTrans);
            cardEntity.MoveLocalPosInTime(Constants.moveTime, new Vector3(Constants.cardDistance * handCardNum, 0, 0));
            yield return new WaitForSeconds(Constants.moveTime);
        }




    }
    //出牌动画
    public void OutCardAni()
    {
        handCardNum--;
        int flag = -1;
        for (int i = 0; i < selfCardEntityList.Count; i++)
        {
            if (flag >= 0)
            {
                selfCardEntityList[i].MoveLocalPosInTime(Constants.outCardMoveTime, new Vector3(-Constants.cardDistance, 0, 0));
            }

            if (selfCardEntityList[i].mState == CardState.Prepare)
            {
                selfCardEntityList[i].MoveLocalPosInTime(Constants.outCardMoveTime, new Vector3(0, Constants.upDistance, 0));

                flag = i;
            }

        }

        if (flag >= 0)
        {
            selfCardEntityList.RemoveAt(flag);

        }

        for (int i = 0; i < selfCardEntityList.Count; i++)
        {
            selfCardEntityList[i].mRectTrans.name = "msg_" + i;
            selfCardEntityList[i].index = i;
        }

        Destroy(prepareEntity.mRectTrans.gameObject);
        prepareEntity = null;

    }

    public void OutCardListAni()
    {
        

        for (int i = selfCardEntityList.Count-1; i >=0 ; i--)
        {


            if (selfCardEntityList[i].mState == CardState.Prepare)
            {
                selfCardEntityList[i].MoveLocalPosInTime(Constants.outCardMoveTime, new Vector3(0, Constants.upDistance, 0));
                CardEntity temp = selfCardEntityList[i];
                selfCardEntityList.RemoveAt(i);
                Destroy(temp.mRectTrans.gameObject);
                handCardNum--;

            }

        }


        for (int i = 0; i < selfCardEntityList.Count; i++)
        {
            selfCardEntityList[i].mRectTrans.name = "msg_" + i;
            selfCardEntityList[i].index = i;
            selfCardEntityList[i].MoveTargetPosInTime(Constants.outCardMoveTime, new Vector3(Constants.cardDistance*i, 0, 0));
        }


    }
    public void UseCard(Card card, int index, bool isBurnCard = false)
    {
        //创建卡牌实体
        GameObject go = Instantiate(cardObj);
        RectTransform rectTrans = go.GetComponent<RectTransform>();
        rectTrans.SetParent(ExhibitioAreaTrans);
        rectTrans.sizeDelta = new Vector2(170, 240);
        rectTrans.localScale = Vector3.one;
        go.name = "outCard";

        CardEntity cardEntity = new CardEntity(rectTrans, -1);
        cardEntity.SetEntityData(card);
        //cardEntity.SetRectPos(new Vector3(0, 0, 0));

        SetActive(cardEntity.mRectTrans.gameObject, false);

        if (PECommon.IsProbing(card) && !isMyTurn && !isBurnCard)
        {
            Image img = cardEntity.mRectTrans.GetComponent<Image>();
            string spName = "Probing";
            Sprite sp = GetRes<Sprite>("ResImages/Message/" + spName);
            img.sprite = sp;
        }

        outCardEntityList.Add(cardEntity);

        int sendPos = 0 - (selfIndex - index);
        if (sendPos >= 5)
        {
            sendPos -= 5;
        }
        else if (sendPos < 0)
        {
            sendPos += 5;
        }

        if (isBurnCard)
        {
            StartCoroutine(WaitAni(cardEntity, sendPos));

        }
        else
        {
            StartCoroutine(UseCardAni(cardEntity, sendPos));
        }





    }

    IEnumerator WaitAni(CardEntity cardEntity, int sendPos)
    {
        yield return new WaitForSeconds(1);
        StartCoroutine(UseCardAni(cardEntity, sendPos));

    }

    //卡牌使用动画
    IEnumerator UseCardAni(CardEntity cardEntity, int sendPos)
    {
        SetActive(cardEntity.mRectTrans.gameObject);

        cardEntity.SetRectPos(acceptPos[sendPos]);


        if (outCardEntityList.Count > 4)
        {
            cardEntity.MoveTargetPosInTime(Constants.messageMoveTime / 2, new Vector3(Constants.cardDistance * (outCardEntityList.Count - 4), 0, 0));
        }
        else
        {
            cardEntity.MoveTargetPosInTime(Constants.messageMoveTime / 2, new Vector3(0, 0, 0));

            for (int i = 0; i < outCardEntityList.Count - 1; i++)
            {
                outCardEntityList[i].MoveLocalPosInTime(Constants.messageMoveTime / 2, new Vector3(-Constants.cardDistance, 0, 0));
            }
        }



        yield return new WaitForSeconds(Constants.messageMoveTime);


    }

    private Vector3[] messageTransPos = new Vector3[5]
{
        new Vector3(-567,-178,0),
        new Vector3(636,0,0),
        new Vector3(514,396,0),
        new Vector3(-79,396,0),
        new Vector3(-633,209,0)
};

    //情报传递动画
    IEnumerator MessageTransferAni(CardEntity cardEntity, int actualPos, int targetPos)
    {

        cardEntity.SetRectPos(messageTransPos[actualPos]);
        cardEntity.MoveTargetPosInTime(Constants.messageMoveTime, messageTransPos[targetPos]);

        yield return new WaitForSeconds(Constants.messageMoveTime);


    }

    //情报被选中时的函数
    void OnMessageClicked(GameObject go)
    {
        int index = -1;
        string numstr = go.name.Substring(4, go.name.Length - 4);
        int.TryParse(numstr, out index);
        if (index != -1)
        {
            if (showCardEntityList[index].cardData.color == CardColor.Blue || showCardEntityList[index].cardData.color == CardColor.Red)
            {
                TipsWindow.AddTips("不能烧毁非黑情报，请重新选择");
            }
            else
            {
                int serverIndex = GetServerIndex(showMessageRegionIndex);
                GameMsg msg = new GameMsg
                {
                    cmd = CMD.RequestOutCard,
                    requestOutCard = new RequestOutCard
                    {
                        card = prepareEntity.cardData,
                        burnCard = showCardEntityList[index].cardData,
                        targetIndex = serverIndex,
                        hasTarget = true
                    }
                };
                netSvc.SendMsg(msg);
                OutCardAni();
                isMessageClick = false;
                ClickCloseMessageRegion();
            }

        }
    }

    //情报区域被选中时的函数
    void OnMessageRegionClicked(GameObject go)
    {


        int index = -1;
        string numstr = go.name.Substring(7, go.name.Length - 7);
        int.TryParse(numstr, out index);
        if (index != -1)
        {
            showMessageRegionIndex = index;
            int serverIndex = GetServerIndex(index);
            GameMsg msg = new GameMsg
            {
                cmd = CMD.RequestMessageInfo,
                requestMessageInfo = new RequestMessageInfo
                {
                    index = serverIndex
                }
            };
            netSvc.SendMsg(msg);
        }
    }

    //手牌被选中时的函数
    void OnCardItemSelected(GameObject go)
    {

        audioSvc.PlayUIAudio(Constants.SelectCard);

        int index = -1;
        string numstr = go.name.Substring(4, go.name.Length - 4);
        int.TryParse(numstr, out index);
        if (index != -1)
        {
            ChangeCardItemState(index);
        }
    }
    //改变手牌被选中时的状态
    void ChangeCardItemState(int index)
    {
        CardEntity ce = selfCardEntityList[index];
        if (ce.mState == CardState.Normal)
        {
            if (prepareEntity != null)
            {
                prepareEntity.SetCardEntityState(CardState.Normal);
                prepareEntity = null;
            }
            ce.SetCardEntityState(CardState.Prepare);
            prepareEntity = ce;
        }
        else
        {
            ce.SetCardEntityState(CardState.Normal);
            prepareEntity = null;
        }
    }

    //手牌群体被选中时的函数
    void OnCardItemListSelected(GameObject go)
    {

        audioSvc.PlayUIAudio(Constants.SelectCard);

        int index = -1;
        string numstr = go.name.Substring(4, go.name.Length - 4);
        int.TryParse(numstr, out index);
        if (index != -1)
        {
            ChangeCardItemListState(index);
        }
    }

    //改变手牌滑动被选中时的状态
    void ChangeCardItemListState(int index)
    {
        CardEntity ce = selfCardEntityList[index];
        if (ce.mState == CardState.Normal)
        {
            ce.SetCardEntityState(CardState.Prepare);
            prepareBalanceEntityList.Add(ce);
        }
        else
        {
            ce.SetCardEntityState(CardState.Normal);
            for (int i = 0; i < prepareBalanceEntityList.Count; i++)
            {
                if (prepareBalanceEntityList[i].Equals(ce))
                {
                    prepareBalanceEntityList.RemoveAt(i);
                }
            }
        }
    }
    //给人物挂载点击事件
    public void AddPlayerClickEvent()
    {
        for (int i = 0; i < charImg.Length; i++)
        {
            OnClickUp(charImg[i].gameObject, OnPlayerSelected);

        }
    }

    //给情报挂载点击事件
    public void AddMessageClickEvent()
    {
        isMessageClick = true;
    }

    //人物被选中时的函数
    public void OnPlayerSelected(GameObject go)
    {
        int index = -1;
        string numstr = go.name.Substring(7, go.name.Length - 7);
        int.TryParse(numstr, out index);
        if (index != -1)
        {
            int targetPos = selfIndex + index;

            if (targetPos >= 5)
            {
                targetPos -= 5;
            }
            else if (targetPos < 0)
            {
                targetPos += 5;
            }

            switch (messageStage)
            {
                case MessageStage.PlayStage:
                    GameMsg outCardMsg = new GameMsg
                    {
                        cmd = CMD.RequestOutCard,
                        requestOutCard = new RequestOutCard
                        {
                            card = prepareEntity.cardData,
                            targetIndex = targetPos,
                            hasTarget = true
                        }
                    };
                    netSvc.SendMsg(outCardMsg);
                    break;
                case MessageStage.MessageTransfer:
                    GameMsg transMsg = new GameMsg
                    {
                        cmd = CMD.RequestMessageTransfer,
                        requestMessageTransfer = new RequestMessageTransfer
                        {
                            message = prepareEntity.cardData,
                            targetIndex = targetPos
                        }
                    };
                    netSvc.SendMsg(transMsg);
                    break;
                case MessageStage.ArriveSection:
                    GameMsg arrCardMsg = new GameMsg
                    {
                        cmd = CMD.RequestOutCard,
                        requestOutCard = new RequestOutCard
                        {
                            card = prepareEntity.cardData,
                            targetIndex = targetPos,
                            hasTarget = true
                        }
                    };
                    netSvc.SendMsg(arrCardMsg);
                    break;
                default:
                    break;
            }

            DestroyPlayerClickEvent();
            OutCardAni();


        }
    }

    //销毁人物点击事件
    public void DestroyPlayerClickEvent()
    {
        for (int i = 0; i < charImg.Length; i++)
        {
            PEListener listener = charImg[i].gameObject.GetComponent<PEListener>();

            Destroy(listener);
        }
    }

    //更改手牌点击事件
    public void ChangeCardClickEvent(Action<GameObject> action)
    {
        for(int i = 0; i < selfCardEntityList.Count; i++)
        {
            PEListener listener = selfCardEntityList[i].mRectTrans.gameObject.GetComponent<PEListener>();

            listener.onClickUp = action;

        }
    }



    public void DestroyOutCard()
    {
        for(int i = 0; i < outCardEntityList.Count; i++)
        {
            Destroy(outCardEntityList[i].mRectTrans.gameObject);
        }
        outCardEntityList.Clear();

    }

    private Vector3[] lineTargetPos = new Vector3[5]
{
        new Vector3(-647,-178,-1),
        new Vector3(694,114,-1),
        new Vector3(306,138,-1),
        new Vector3(-285,138,-1),
        new Vector3(-708,84,-1)
};

    //指定目标
    public void SpecifyTarget(int curIndex, int targetIndex)
    {

        int sendPos = GetLocalIndex(curIndex);

        int targetPos = GetLocalIndex(targetIndex);


        //StartCoroutine(SetTrail(sendPos, targetPos));
        SetTrail(sendPos, targetPos);
    }


    //设置线条
    public void SetTrail(int curIndex, int targetIndex)
    {




        Vector3 targetPos = lineTargetPos[targetIndex];//目标位置
        Vector3 curPos = lineTargetPos[curIndex];//起始位置

        trail.localPosition = curPos;

        SetActive(trail);


        trail.DOLocalMove(targetPos, 1);




        Invoke("SetTrailFalse", 2);

    }

    public void SetTrailFalse()
    {
        SetActive(trail, false);

    }

    /*
        //设置线条
        IEnumerator SetLine(int curIndex,int targetIndex)
        {

            SetActive(Line);
            SetActive(pos);


            Vector3 targetPos = lineTargetPos[targetIndex];//目标位置
            Vector3 curPos = lineTargetPos[curIndex];//起始位置

            pos.rectTransform.localPosition = targetPos;

            Line.rectTransform.localPosition = curPos;
            Line.rectTransform.sizeDelta = new Vector2(8, Vector3.Distance(targetPos, curPos));

                    //设置角度
            double angle = Math.Atan2(targetPos.y - curPos.y, targetPos.x - curPos.x) * 180 / Math.PI;
            Line.rectTransform.rotation = Quaternion.Euler(0, 0, (float)angle + 270);

            yield return new WaitForSeconds(Constants.lineWaitTime);

            SetActive(Line, false);
            SetActive(pos, false);

        }*/


    public void ShowMessageInfo(List<Card> messageList)
    {
        SetActive(messageRegionTrans);

        for(int i = 0; i < messageList.Count; i++)
        {
            GameObject go = Instantiate(cardObj);
            RectTransform rectTrans = go.GetComponent<RectTransform>();
            rectTrans.SetParent(messageRegionTrans);
            rectTrans.sizeDelta = new Vector2(170, 240);
            rectTrans.localScale = Vector3.one;

            if (isMessageClick)
            {
                OnClickUp(rectTrans.gameObject, OnMessageClicked);

            }


            go.name = "msg_" + i;


            CardEntity cardEntity = new CardEntity(rectTrans, -1);

            cardEntity.SetEntityData(messageList[i], false);
            cardEntity.SetRectPos(new Vector3(-414 + i * Constants.cardDistance, 22, 0));

            showCardEntityList.Add(cardEntity);
        }
    }

    public void RoundEnd()
    {
        isMyTurn = false;
        isLocking = false;
        isTransMsg = false;
        isMyDisCard = false;
        isMessageClick = false;
        isTigerMountain = false;
        for (int i = 0; i < Constants.fivePersonFieldCount; i++)
        {
            SetActive(playerState[i], false);
        }
        messagePositionIndex = -1;
    }

    #region Card Function

    public void Probing(int index,int action)
    {
        int targetIndex = GetLocalIndex(index);
        switch (action)
        {
            case 0:
                StartCoroutine(SetProbingInfo(targetIndex, "弃牌"));
                break;
            case 1:
                StartCoroutine(SetProbingInfo(targetIndex, "抽牌"));
                break;
            case 2:
                StartCoroutine(SetProbingInfo(targetIndex, "我是个好人"));
                break;
            default:
                break;
        }
    }

    IEnumerator SetProbingInfo(int index,string info)
    {
        probingTrans.localPosition = acceptPos[index];
        SetText(probingTxt, info);

        SetActive(probingTrans);

        yield return new WaitForSeconds(Constants.probingWaitTime);

        SetActive(probingTrans, false);

    }

    public void Gambling(int index,Card card)
    {
        GameObject go = Instantiate(cardObj);
        RectTransform rectTrans = go.GetComponent<RectTransform>();
        rectTrans.SetParent(ExhibitioAreaTrans);
        rectTrans.sizeDelta = new Vector2(170, 240);
        rectTrans.localScale = Vector3.one;
        rectTrans.localPosition = new Vector3(874, 418, 0);
        go.name = "balanceCard";

        CardEntity cardEntity = new CardEntity(rectTrans, -1);

        cardEntity.SetEntityData(card);
        int targetIndex = GetLocalIndex(index);
        
        StartCoroutine(GamblingAni(targetIndex, cardEntity));



    }

    IEnumerator GamblingAni(int index, CardEntity card)
    {

        card.MoveTargetPosInTime(Constants.messageMoveTime, messageTransPos[index]);
        yield return new WaitForSeconds(Constants.acceptMessageTime + Constants.messageMoveTime);

        card.MoveTargetPosInTime(Constants.messageMoveTime, acceptPos[index], null, Constants.acceptMessageSize);
        yield return new WaitForSeconds(Constants.messageMoveTime);

        Destroy(card.mRectTrans.gameObject);

    }

    public void RealOrFalse(List<Card> cardList, int index)
    {
        StartCoroutine(RealOrFalseAni(cardList, index));

    }

    IEnumerator RealOrFalseAni(List<Card> cardList,int index)
    {
        int localIndex = GetLocalIndex(index);
        for(int i = 0; i < Constants.fivePersonFieldCount; i++)
        {
            GameObject go = Instantiate(cardObj);
            RectTransform rectTrans = go.GetComponent<RectTransform>();
            rectTrans.SetParent(ExhibitioAreaTrans);
            rectTrans.sizeDelta = new Vector2(170, 240);
            rectTrans.localScale = Vector3.one;
            rectTrans.localPosition = new Vector3(874, 418, 0);
            go.name = "realOrFalseCard";

            CardEntity cardEntity = new CardEntity(rectTrans, -1);

            cardEntity.SetEntityData(cardList[i]);
            StartCoroutine(GamblingAni(localIndex, cardEntity));
            yield return new WaitForSeconds(Constants.messageMoveTime*4+Constants.acceptMessageTime);

            localIndex++;
            if (localIndex > 4)
            {
                localIndex -= 5;
            }
        }
        GameMsg refreshMsg = new GameMsg
        {
            cmd = CMD.RequestRefreshMessage
        };
        netSvc.SendMsg(refreshMsg);
        if (selfIndex == index)
        {
            GameMsg msg = new GameMsg
            {
                cmd = CMD.RequestEndRealOrFalse
            };
            netSvc.SendMsg(msg);
        }
    }

    public void Locking(int index)
    {
        if(index == selfIndex)
        {
            isLocking = true;
        }
        int localIndex = GetLocalIndex(index);
        SetText(playerState[localIndex], "锁");
        SetActive(playerState[localIndex]);
    }

    public void TigerMountain(int index)
    {

        if (index == selfIndex)
        {
            isTigerMountain = true;
        }
        int localIndex = GetLocalIndex(index);
        SetText(playerState[localIndex], "调");
        SetActive(playerState[localIndex]);
    }

    public void Swap(Card card)
    {
        transEntity.SetEntityData(card);
    }
    #endregion

    #region Set And Get
    public void SetMessagePositionIndex(int index)
    {
        messagePositionIndex = index;
    }

    public void SetIsMyDisCard(bool flag)
    {
        isMyDisCard = flag;
    }

    public void SetIsMyTurn(bool flag)
    {
        isMyTurn = flag;
    }

    public bool GetIsMyTurn()
    {
        return isMyTurn;
    }
    #endregion


    #region TOOL

    public int GetLocalIndex(int serverIndex)
    {
        int localIndex = 0 - (selfIndex - serverIndex);
        if (localIndex >= 5)
        {
            localIndex -= 5;
        }
        else if (localIndex < 0)
        {
            localIndex += 5;
        }
        return localIndex;
    }

    public int GetServerIndex(int localIndex)
    {
        int serverIndex = selfIndex + localIndex;
        if (serverIndex >= 5)
        {
            serverIndex -= 5;
        }
        else if (serverIndex < 0)
        {
            serverIndex += 5;
        }
        return serverIndex;
    }
    #endregion

}
