//ս������


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
    RoundStart,//�غϿ�ʼ�׶�
    DrawPhase,//���ƽ׶�
    PlayStage,//���ƽ׶�
    MessageTransfer,//�鱨���ݽ׶�
    TransferSection,//����С��
    ArriveSection,//����С��
    AcceptSection,//����С��
    RoundEnd //�غϽ����׶�
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

    public Transform selfCardTrans;//����
    public Transform btnGroupTrans;//��ť��
    public Transform messageTrans;//�鱨������
    public Transform ExhibitioAreaTrans;//����չʾ��

    public Material outLineMaterial;

    public Text txtTimer;
    public Text cardLibrary;

    private AudioSvc audioSvc;
    private NetSvc netSvc;

    public int selfIndex { private set; get; }
    private int charIndex;
    private int[] charList;

    private List<CardEntity> selfCardEntityList = new List<CardEntity>();
    private CardEntity prepareEntity = null;//׼��ʹ�õĿ���ʵ��
    private GameObject cardObj = null;

    private CardEntity transEntity = null;//�����е��鱨ʵ��
    public bool isMyTurn = false;
    private bool isTransMsg = false;

    private int handCardNum;

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
        //��������
        for(int i = 0; i < 5; i++)
        {
            SetText(cards[i], "4");
            SetText(redNum[i], "0");
            SetText(blueNum[i], "0");
            SetText(blackNum[i], "0");
        }

        SetText(cardLibrary, "93");



        //�����Ͼ����µĲ�����
        selfCardEntityList.Clear();

        for (int i = 0; i < selfCardTrans.childCount; i++)
        {
            Destroy(selfCardTrans.GetChild(i).gameObject);
        }

        //������Դ�е�UI
        cardObj = Resources.Load("UIWindow/CardItem") as GameObject;
        selfCardTrans.localPosition = new Vector3(-580, -380, 0);


        //�����µ���
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
    }

    public void SetMessageStage(MessageStage state)
    {
        messageStage = state;
        SetActive(btnGroupTrans);
        switch (messageStage)
        {
            case MessageStage.SelectChar:
                SetActive(btn1bg.transform, false);
                SetActive(btn2bg.transform, false);
                break;
            case MessageStage.PlayStage:
                if (isMyTurn)
                {
                    SetActive(btn1bg.transform);
                    SetBtnInfo(btn1bg, btn1txt, "ʹ��", 1, new Vector2(-175, -50));


                    SetActive(btn2bg.transform);
                    SetBtnInfo(btn2bg, btn2txt, "����", 2, new Vector2(200, -50));


                    btnGroupTrans.localPosition = new Vector3(0, -75, 0);
                }
                else
                {
                    SetActive(btn1bg.transform, false);
                    SetActive(btn2bg.transform, false);
                }
                SetClockCallBack(Constants.playStageCounter, ClickBtn2);
                break;
            case MessageStage.MessageTransfer:
                isTransMsg = false;
                if (isMyTurn)
                {
                    SetActive(btn1bg.transform);
                    SetBtnInfo(btn1bg, btn1txt, "�����鱨", 1, new Vector2(-50, -50));

                    SetActive(btn2bg.transform, false);
                }
                else
                {
                    SetActive(btn1bg.transform, false);
                    SetActive(btn2bg.transform, false);
                }

                //SetClockCallBack(Constants.messageTransferCounter, ClickBtn1);
                break;
            case MessageStage.AcceptSection:
                TipsWindow.AddTips("��ѡ���Ƿ�����鱨");
                SetActive(btn1bg.transform);
                SetBtnInfo(btn1bg, btn1txt, "Ҫ", 1, new Vector2(-175, -50));

                SetActive(btn2bg.transform);
                SetBtnInfo(btn2bg, btn2txt, "��Ҫ", 2, new Vector2(200, -50));
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

                if (PECommon.PlayStageUsability(prepareEntity.cardData))
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
                    TipsWindow.AddTips("��ʱ�޷�ʹ�ø���");
                }


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

                            TipsWindow.AddTips("��������ͷ����ѡ��Ŀ��");

                            AddPlayerClickEvent();


                            SetActive(btnGroupTrans, false);

                        }
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
            TipsWindow.AddTips("��ѡ������");
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
            TipsWindow.AddTips("��ȴ��������ѡ��");
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

    public void SetAddMessageInfo(int posIndex,int addCards,int addRedNum,int addBlueNum,int addBlackNum)
    {
        int flag =0-(selfIndex - posIndex);
        if (flag >= 5)
        {
            flag -= 5;
        }else if (flag < 0)
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

    public void endPlay()
    {
        StopClock(txtTimer.gameObject);
        SetMessageStage(MessageStage.MessageTransfer);
    }

    //�ܵ��鱨���ݶ���
    public void MessageTransferAni(Card message,int posIndex,int targetIndex = -1)
    {
        GameObject go = Instantiate(cardObj);
        RectTransform rectTrans = go.GetComponent<RectTransform>();
        rectTrans.SetParent(messageTrans);
        rectTrans.sizeDelta = new Vector2(170, 240);
        rectTrans.localScale = Vector3.one;
        go.name = "transMsg";




        CardEntity cardEntity = new CardEntity(rectTrans, -1);
        cardEntity.SetEntityData(message,true);
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


        StartCoroutine(MessageTransferAni(transEntity, actualPos,targetPos));


    }
    //�ܵ��ı��鱨�����ж���
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
    //ֱ���鱨�����ж���
    public void NonstopMessageTransAni(int sendIndex,int targetIndex)
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

    //չʾ����
    public void ShowSelfCard(List<Card> cardList)
    {
        SetActive(selfCardTrans);

        for (int i = 0; i < cardList.Count; i++)
        {
            selfCardEntityList[i].SetEntityData(cardList[i]);
        }
    }

    //����ʱ����
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

    //ֹͣ��ʱ
    public void StopClock(GameObject gameObject)
    {

        PETimer pt = (PETimer)GetOrAddComponent<PETimer>(gameObject);
        pt.isRun = false;
        SetText(txtTimer, "0");

    }

    //���ƶ���
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

    //�鿨����
    IEnumerator DrawMoveCard(CardEntity cardEntity)
    {

        if (handCardNum < 8)
        {
            SetActive(cardEntity.mRectTrans);
            cardEntity.MoveLocalPosInTime(Constants.moveTime, new Vector3(Constants.cardDistance * handCardNum, 0, 0));
            yield return new WaitForSeconds(Constants.moveTime);
        }




    }
    //���ƶ���
    public void OutCardAni()
    {
        handCardNum--;
        int flag = -1;
        for (int i = 0; i < selfCardEntityList.Count; i++)
        {
            if (flag >= 0)
            {
                selfCardEntityList[i].MoveLocalPosInTime(Constants.outCardMoveTime, new Vector3(-Constants.cardDistance,0,0));
            }

            if (selfCardEntityList[i].mState == CardState.Prepare)
            {
                selfCardEntityList[i].MoveLocalPosInTime(Constants.outCardMoveTime, new Vector3(0, Constants.upDistance,0));

                flag = i;
            }

        }

        if (flag >= 0)
        {
            selfCardEntityList.RemoveAt(flag);

        }

        for(int i = 0; i < selfCardEntityList.Count; i++)
        {
            selfCardEntityList[i].mRectTrans.name = "msg_" + i;
            selfCardEntityList[i].index = i;
        }

        Destroy(prepareEntity.mRectTrans.gameObject);
        prepareEntity = null;

    }
    private Vector3[] messageTransPos = new Vector3[5]
{
        new Vector3(-567,-178,0),
        new Vector3(636,0,0),
        new Vector3(514,396,0),
        new Vector3(-79,396,0),
        new Vector3(-633,209,0)
};

    //�鱨���ݶ���
    IEnumerator MessageTransferAni(CardEntity cardEntity,int actualPos,int targetPos)
    {

        cardEntity.SetRectPos(messageTransPos[actualPos]);
        cardEntity.MoveTargetPosInTime(Constants.messageMoveTime, messageTransPos[targetPos]);

        yield return new WaitForSeconds(Constants.messageMoveTime);


    }

    //���Ʊ�ѡ��ʱ�ĺ���
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
    //�ı����Ʊ�ѡ��ʱ��״̬
    void ChangeCardItemState(int index)
    {
        CardEntity ce = selfCardEntityList[index];
        if(ce.mState == CardState.Normal)
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

    //��������ص���¼�
    public void AddPlayerClickEvent()
    {
        for(int i = 0; i < charImg.Length; i++)
        {
            OnClickUp(player[i].gameObject,OnPlayerSelected);

        }
    }

    //���ﱻѡ��ʱ�ĺ���
    public void OnPlayerSelected(GameObject go)
    {
        this.Log("click!!!!!!!!!!!");
        int index = -1;
        string numstr = go.name.Substring(6, go.name.Length - 6);
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
            DestroyPlayerClickEvent();
            OutCardAni();


        }
    }

    //�����������¼�
    public void DestroyPlayerClickEvent()
    {
        for (int i = 0; i < charImg.Length; i++)
        {
            PEListener listener = (PEListener)GetOrAddComponent<PEListener>(charImg[i].gameObject);
            Destroy(listener,0);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
