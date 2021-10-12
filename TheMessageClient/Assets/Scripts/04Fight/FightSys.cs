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

    public void ResponseMessageInfo(GameMsg msg)
    {
        messageWindow.ShowMessageInfo(msg.responseMessageInfo.messageList);
    }

    public void PushRoundStart(GameMsg msg)
    {
        if(msg.pushRoundStart.index == messageWindow.selfIndex)
        {
            messageWindow.SetIsMyTurn(true);
        }
        messageWindow.SetTurnIdentification(msg.pushRoundStart.index);
    }


    public void PushDrawCard(GameMsg msg)
    {
        messageWindow.SetAddMessageInfo(msg.pushDrawCard.index, msg.pushDrawCard.cardList.Count, 0, 0, 0);
        if (msg.pushDrawCard.index == messageWindow.selfIndex)
        {
            for(int i = 0; i < msg.pushDrawCard.cardList.Count; i++)
            {
                messageWindow.AddCard(msg.pushDrawCard.cardList[i]);
            }
        }
        messageWindow.SetCardLibrary(msg.pushDrawCard.cardLibraryCount);
        
    }

    public void PushOutCard(GameMsg msg)
    {
        messageWindow.UseCard(msg.pushOutCard.card, msg.pushOutCard.sendIndex);
        if (msg.pushOutCard.hasTarget)
        {
            messageWindow.SpecifyTarget(msg.pushOutCard.sendIndex, msg.pushOutCard.targetIndex);
        }
        if(msg.pushOutCard.card.function == CardFunction.Burn)
        {
            messageWindow.UseCard(msg.pushOutCard.burnCard, msg.pushOutCard.targetIndex,true);

        }
        if (messageWindow.selfIndex == msg.pushOutCard.sendIndex)
        {
            messageWindow.SetMessageStage(MessageStage.ResponseWaitStage);
        }
        else
        {
            messageWindow.SetMessageStage(MessageStage.ResponseStage);
        }
        //TODO
    }

    public void PushDisCard(GameMsg msg)
    {
        if(messageWindow.selfIndex == msg.pushDisCard.targetIndex)
        {
            messageWindow.SetIsMyDisCard(true);
        }
        messageWindow.SetMessageStage(MessageStage.DisCardStage);

    }

    public void PushProbingInfo(GameMsg msg)
    {
        //TODO
        messageWindow.Probing(msg.pushProbingInfo.targetIndex, msg.pushProbingInfo.responseAction);
    }

    public void PushGamblingCard(GameMsg msg)
    {
        messageWindow.Gambling(msg.pushGamblingCard.index, msg.pushGamblingCard.card);
    }
    public void PushPlayStage(GameMsg msg)
    {
        messageWindow.DestroyOutCard();
        messageWindow.SetMessageStage(MessageStage.PlayStage);
    }

    public void PushEndPlay(GameMsg msg)
    {
        messageWindow.endPlay();
    }

    public void PushMessageTransfer(GameMsg msg)
    {
        if(msg.pushMessageTransfer.message.type == CardType.RestrictedMessage || msg.pushMessageTransfer.message.type == CardType.TextMessage)
        messageWindow.MessageTransferAni(msg.pushMessageTransfer.message, msg.pushMessageTransfer.transferIndex);
        else
        messageWindow.MessageTransferAni(msg.pushMessageTransfer.message, msg.pushMessageTransfer.transferIndex,msg.pushMessageTransfer.targetIndex);
        
    }

    public void PushAcceptSection(GameMsg msg)
    {
        messageWindow.SetMessageStage(MessageStage.AcceptSection);
    }

    public void PushMessageTransfering(GameMsg msg)
    {
        if(msg.pushMessageTransfering.message.type == CardType.NonstopMessage)
        {
            messageWindow.NonstopMessageTransAni(msg.pushMessageTransfering.transferIndex, msg.pushMessageTransfering.targetIndex);
        }
        else
        {
            messageWindow.MessageTransferingAni(msg.pushMessageTransfering.targetIndex);

        }


        if (msg.pushMessageTransfering.targetIndex == messageWindow.selfIndex)//如果目标的人是自己
        {
            if (messageWindow.GetIsMyTurn())
            {
                GameMsg acceptMsg = new GameMsg
                {
                    cmd = CMD.RequestAcceptMessage,
                    requestAcceptMessage = new RequestAcceptMessage
                    {
                        isAccept = true
                    }
                };
                netSvc.SendMsg(acceptMsg);
            }
            else
            {
                messageWindow.SetMessageStage(MessageStage.AcceptSection);
            }
        }
    }

    public void PushSinglePlayerMessageUpdate(GameMsg msg)
    {
        messageWindow.SetMessageInfo(
            msg.pushSinglePlayerMessageUpdate.posIndex, 
            msg.pushSinglePlayerMessageUpdate.cards, 
            msg.pushSinglePlayerMessageUpdate.redNum, 
            msg.pushSinglePlayerMessageUpdate.blueNum, 
            msg.pushSinglePlayerMessageUpdate.blackNum);

        messageWindow.SetCardLibrary(msg.pushSinglePlayerMessageUpdate.cardLibraryCount);
    }

    public void PushConfirmAcceptMessage(GameMsg msg)
    {
        messageWindow.ConfirmAcceptMessage(msg.pushConfirmAcceptMessage.index);
    }


    public void PushRoundEnd(GameMsg msg)
    {
        if (messageWindow.GetIsMyTurn()) messageWindow.SetIsMyTurn(false);
    }
}
