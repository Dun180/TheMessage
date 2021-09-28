//战斗界面


using PEProtocol;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessageWindow : WindowRoot
{

    public Image[] charImg;

    public Text[] charName;
    public Text[] playerName;
    public Text[] cards;
    public Text[] redNum;
    public Text[] blueNum;
    public Text[] blackNum;

    public Text selfIdentity;

    public Image selectionBox;
    public Image char_1;
    public Image char_2;
    public Image char_3;

    public Transform selfCardTrans;


    public Material outLineMaterial;

    public Text txtTimer;
    public Text cardLibrary;

    private AudioSvc audioSvc;
    private NetSvc netSvc;

    public int selfIndex { private set; get; }
    private int charIndex;
    private int[] charList;

    private List<CardEntity> selfCardEntityList = new List<CardEntity>();
    private CardEntity prepareEntity = null;
    private GameObject cardObj = null;

    private int handCardNum;

    public override void InitWindow()
    {
        base.InitWindow();
        audioSvc = AudioSvc.Instance;
        netSvc = NetSvc.Instance;

        SetActive(selectionBox, false);

        handCardNum = 0;
        selfIndex = -1;
        charIndex = -1;
        charList = new int[3];
        SetText(selfIdentity, "");
        //重置数据
        for(int i = 0; i < 5; i++)
        {
            SetText(cards[i], "4");
            SetText(redNum[i], "0");
            SetText(blueNum[i], "0");
            SetText(blackNum[i], "0");
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
        }
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

    public void SetMessageInfo(int posIndex,int addCards,int addRedNum,int addBlueNum,int addBlackNum)
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
        pt.isRun = false;
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
        if(ce.mState == CardState.Normal)
        {
            ce.SetCardEntityState(CardState.Prepare);
            prepareEntity = ce;
        }
        else
        {
            ce.SetCardEntityState(CardState.Normal);
            prepareEntity = null;
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
