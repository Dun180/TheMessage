//核心对战逻辑

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PEProtocol;

public class FightSys : MonoBehaviour
{
    public static FightSys Instance;

    public MessageWindow messageWindow;


    private NetSvc netSvc;
    private LobbySys lobbySys;

    private void Awake()
    {
        Instance = this;
    }

    public void Init()
    {
        netSvc = NetSvc.Instance;
        lobbySys = LobbySys.Instance;
    }

    public void EnterMessage()
    {
        messageWindow.InitWindow();
        messageWindow.SetWindowState();
    }

    //请求刷新房间信息
    public void RequestRefreshMessage()
    {
        netSvc.SendMsg(new GameMsg { cmd = CMD.RequestRefreshMessage });
    }

    //刷新房间信息
    public void ResponseRefreshMessage(GameMsg msg)
    {
        messageWindow.RefreshMessage(msg.responseRefreshMessage.selfPosIndex, msg.responseRefreshMessage.playerArr);
    }

    public void PushChar(GameMsg msg)
    {
        messageWindow.SelectChar(msg.pushChar);
        messageWindow.SetMessageStage(MessageStage.SelectChar);
    }

    public void PushSelectChar(GameMsg msg)
    {
        messageWindow.RefreshMessage(messageWindow.selfIndex, msg.pushSelectChar.playerArr);
    }

    public void PushIdentityInfo(GameMsg msg)
    {

        
        int identity = msg.pushIdentityInfo.identity;

        switch (identity)
        {
            case 1:
                TipsWindow.AddTips("您的身份为：潜伏");
                messageWindow.SetIdentity("潜");
                break;
            case 2:
                TipsWindow.AddTips("您的身份为：军情");
                messageWindow.SetIdentity("军");
                break;
            case 3:
                TipsWindow.AddTips("您的身份为：酱油");
                messageWindow.SetIdentity("酱");
                break;
            default:
                break;

        }
    }

    public void PushCard(GameMsg msg)
    {
        messageWindow.ShowSelfCard(msg.pushCard.cardList);
        netSvc.SendMsg(new GameMsg { cmd = CMD.RequestRefreshMessage });
    }

    public void PushRoundStart(GameMsg msg)
    {
        messageWindow.SetTurnIdentification(msg.pushRoundStart.index);
    }


    public void PushDrawCard(GameMsg msg)
    {
        messageWindow.SetMessageInfo(msg.pushDrawCard.index, msg.pushDrawCard.cardList.Count, 0, 0, 0);
        if (msg.pushDrawCard.index == messageWindow.selfIndex)
        {
            messageWindow.isMyTurn = true;
            for(int i = 0; i < msg.pushDrawCard.cardList.Count; i++)
            {
                messageWindow.AddCard(msg.pushDrawCard.cardList[i]);
            }
        }
        messageWindow.SetMessageStage(MessageStage.PlayStage);
    }

    public void PushEndPlay(GameMsg msg)
    {
        messageWindow.endPlay();
    }

    public void PushMessageTransfer(GameMsg msg)
    {
        messageWindow.MessageTransferAni(msg.pushMessageTransfer.message, msg.pushMessageTransfer.transferIndex);
        
        
        

    }

    public void PushAcceptSection(GameMsg msg)
    {
        messageWindow.SetMessageStage(MessageStage.AcceptSection);
    }

    public void PushMessageTransfering(GameMsg msg)
    {
        messageWindow.MessageTransferingAni(msg.pushMessageTransfering.transferIndex);

        int targetPos = msg.pushMessageTransfering.transferIndex + 1;
        if (targetPos >= 5)
        {
            targetPos -= 5;
        }
        else if (targetPos < 0)
        {
            targetPos += 5;
        }
        if (msg.pushMessageTransfering.transferIndex == messageWindow.selfIndex)//如果目标的人是自己
        {
            messageWindow.SetMessageStage(MessageStage.AcceptSection);
        }
    }
}
