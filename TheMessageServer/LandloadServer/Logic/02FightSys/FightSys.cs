//战斗系统
using PEProtocol;
using System;
using System.Collections.Generic;
using System.Text;


public class FightSys
{
    private static FightSys instance = null;
    public static FightSys Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new FightSys();

            }
            return instance;
        }
    }
    private CacheSvc cacheSvc;
    public void Init()
    {
        cacheSvc = CacheSvc.Instance;
    }

    public void Update()
    {

    }

    public void GameStart(MessageRoom messageRoom)
    {
        messageRoom.GameStart();//战斗房间初始化
        DistributionRole(messageRoom);
    }

    //刷新游戏房间数据,需要传回房间中所有人的游戏公开信息
    public void RequestRefreshMessage(MsgPack pack)
    {
        MessageRoom messageRoom = cacheSvc.GetMessageRoomByToken(pack.token);
        PlayerData playerData = cacheSvc.GetPlayerDataByToken(pack.token);
        if (messageRoom == null)
        {
            return;
        }
        messageRoom.UpdateMatchData();
        GameMsg msg = new GameMsg
        {
            cmd = CMD.ResponseRefreshMessage,
            responseRefreshMessage = new ResponseRefreshMessage
            {
                selfPosIndex = messageRoom.GetIndex(playerData.id),
                playerArr = messageRoom.matchPlayerArr
            }
        };
        pack.token.SendMsg(msg);

    }


    public void RequestMessageInfo(MsgPack pack)
    {
        MessageRoom messageRoom = cacheSvc.GetMessageRoomByToken(pack.token);
        GameMsg msg = new GameMsg
        {
            cmd = CMD.ResponseMessageInfo,
            responseMessageInfo = new ResponseMessageInfo
            {
                messageList = messageRoom.playerArr[pack.msg.requestMessageInfo.index].messageList
            }
        };
        pack.token.SendMsg(msg);
    }


    //分发角色牌
    public void DistributionRole(MessageRoom messageRoom)
    {
        //生成随机数组
        List<int> list = new List<int>();
        Random rand = new Random();
        int k = 0;
        do
        {
            k = rand.Next(1, 26);
            if (!list.Contains(k))
                list.Add(k);
        }
        while (list.Count < 20);

        //分发角色牌索引
        MessagePlayer[] playerArr = messageRoom.playerArr;

        for (int i = 0; i < playerArr.Length; i++)
        {
            GameMsg msg = new GameMsg
            {
                cmd = CMD.PushChar,
                pushChar = new PushChar
                {
                    char_1 = list[i * 3],
                    char_2 = list[i * 3 + 1],
                    char_3 = list[i * 3 + 2]
                }
            };
            playerArr[i].token.SendMsg(msg);
        }



    }

    public void RequestSelectChar(MsgPack pack)
    {
        MessageRoom messageRoom = cacheSvc.GetMessageRoomByToken(pack.token);
        PlayerData playerData = cacheSvc.GetPlayerDataByToken(pack.token);
        if (messageRoom == null)
        {
            return;
        }
        string name = cacheSvc.GetCharNameByIndex(pack.msg.requestSelectChar.charIndex);
        bool flag = messageRoom.SetPlayerChar(playerData.id, pack.msg.requestSelectChar.charIndex, name);


        if (flag)
        {

            //推送玩家选择的角色信息
            messageRoom.UpdateMatchData();
            GameMsg msg = new GameMsg
            {
                cmd = CMD.PushSelectChar,
                pushSelectChar = new PushSelectChar
                {
                    playerArr = messageRoom.matchPlayerArr
                }
            };

            cacheSvc.SendMsgAll(messageRoom, msg);

            //随机发放身份
            PushIdentityAndCardInfo(messageRoom);


        }


    }
    //随机发放身份
    public void PushIdentityAndCardInfo(MessageRoom messageRoom)
    {
        messageRoom.RandomIdentity();
        for (int i = 0; i < messageRoom.playerArr.Length; i++)
        {
            GameMsg msg = new GameMsg
            {
                cmd = CMD.PushIdentityInfo,
                pushIdentityInfo = new PushIdentityInfo { identity = (int)messageRoom.playerArr[i].playerIdentity }
            };

            messageRoom.playerArr[i].token.SendMsg(msg);
        }

        //创建牌并分发牌
        messageRoom.CreatRandomCard();
        messageRoom.DispenseCardToPlayer();
        messageRoom.RoundStart();//回合开始
    }

    //处理出牌请求
    public void RequestOutCard(MsgPack pack)
    {
        //TODO
        MessageRoom messageRoom = cacheSvc.GetMessageRoomByToken(pack.token);
        PlayerData playerData = cacheSvc.GetPlayerDataByToken(pack.token);
        int selfindex = messageRoom.GetIndexById(playerData.id);
        messageRoom.RemoveCard(selfindex, pack.msg.requestOutCard.card);

        if (pack.msg.requestOutCard.card.function == CardFunction.Penetrate)
        {
            messageRoom.Penetrate();

            GameMsg msg = new GameMsg
            {
                cmd = CMD.PushOutCard,
                pushOutCard = new PushOutCard
                {
                    card = pack.msg.requestOutCard.card,
                    sendIndex = selfindex
                }
            };

            CacheSvc.Instance.SendMsgAll(messageRoom, msg);
        }
        else if (pack.msg.requestOutCard.card.function == CardFunction.Burn)
        {
            messageRoom.SetWaitSettlementCard(pack.msg.requestOutCard.card, pack.msg.requestOutCard.targetIndex);
            messageRoom.SetWaitBurnCard(pack.msg.requestOutCard.burnCard);
            GameMsg msg = new GameMsg
            {
                cmd = CMD.PushOutCard,
                pushOutCard = new PushOutCard
                {
                    card = pack.msg.requestOutCard.card,
                    sendIndex = selfindex,
                    targetIndex = pack.msg.requestOutCard.targetIndex,
                    hasTarget = true,
                    burnCard = pack.msg.requestOutCard.burnCard
                }
            };

            CacheSvc.Instance.SendMsgAll(messageRoom, msg);
        }
        else
        {

            if (pack.msg.requestOutCard.hasTarget)
            {
                messageRoom.SetWaitSettlementCard(pack.msg.requestOutCard.card, pack.msg.requestOutCard.targetIndex);

                GameMsg msg = new GameMsg
                {
                    cmd = CMD.PushOutCard,
                    pushOutCard = new PushOutCard
                    {
                        card = pack.msg.requestOutCard.card,
                        sendIndex = selfindex,
                        targetIndex = pack.msg.requestOutCard.targetIndex,
                        hasTarget = true
                    }
                };

                CacheSvc.Instance.SendMsgAll(messageRoom, msg);
            }
            else
            {
                messageRoom.SetWaitSettlementCard(pack.msg.requestOutCard.card);

                GameMsg msg = new GameMsg
                {
                    cmd = CMD.PushOutCard,
                    pushOutCard = new PushOutCard
                    {
                        card = pack.msg.requestOutCard.card,
                        sendIndex = selfindex
                    }
                };

                CacheSvc.Instance.SendMsgAll(messageRoom, msg);
            }


        }




    }



    public void RequestEndResponseStage(MsgPack pack)
    {
        //卡牌结算
        //TODO

        MessageRoom messageRoom = cacheSvc.GetMessageRoomByToken(pack.token);

        messageRoom.CardSettlement();

    }
    public void RequestEndPlay(MsgPack pack)
    {
        MessageRoom messageRoom = cacheSvc.GetMessageRoomByToken(pack.token);
        messageRoom.MessageTransfer();

        GameMsg msg = new GameMsg
        {
            cmd = CMD.PushEndPlay
        };
        CacheSvc.Instance.SendMsgAll(messageRoom, msg);

    }


    public void RequestDisCard(MsgPack pack)
    {
        MessageRoom messageRoom = cacheSvc.GetMessageRoomByToken(pack.token);
        PlayerData playerData = cacheSvc.GetPlayerDataByToken(pack.token);
        int index = messageRoom.GetIndexById(playerData.id);
        messageRoom.RemoveCard(index, pack.msg.requestDisCard.disCard);

        GameMsg msg = new GameMsg
        {
            cmd = CMD.PushPlayStage
        };

        CacheSvc.Instance.SendMsgAll(messageRoom, msg);
    }

    public void RequestMessageTransfer(MsgPack pack)
    {
        MessageRoom messageRoom = cacheSvc.GetMessageRoomByToken(pack.token);
        PlayerData playerData = cacheSvc.GetPlayerDataByToken(pack.token);

        if (pack.msg.requestMessageTransfer.message.type == CardType.RestrictedMessage || pack.msg.requestMessageTransfer.message.type == CardType.TextMessage)
        {

            int index = messageRoom.GetIndexById(playerData.id);
            messageRoom.SetTransferMessage(index+1, pack.msg.requestMessageTransfer.message);
            messageRoom.RemoveCard(index, pack.msg.requestMessageTransfer.message);
            GameMsg msg = new GameMsg
            {
                cmd = CMD.PushMessageTransfer,
                pushMessageTransfer = new PushMessageTransfer
                {
                    message = pack.msg.requestMessageTransfer.message,
                    transferIndex = index
                }
            };
            CacheSvc.Instance.SendMsgAll(messageRoom, msg);

            //Test 直接进入接收小节
            messageRoom.AcceptSection();
        }
        else
        {
            int index = messageRoom.GetIndexById(playerData.id);
            messageRoom.SetTransferMessage(pack.msg.requestMessageTransfer.targetIndex, pack.msg.requestMessageTransfer.message);

            messageRoom.RemoveCard(index, pack.msg.requestMessageTransfer.message);

            GameMsg msg = new GameMsg
            {
                cmd = CMD.PushMessageTransfer,
                pushMessageTransfer = new PushMessageTransfer
                {
                    message = pack.msg.requestMessageTransfer.message,
                    transferIndex = index,
                    targetIndex = pack.msg.requestMessageTransfer.targetIndex
                }
            };
            CacheSvc.Instance.SendMsgAll(messageRoom, msg);


            //Test 直接进入接收小节
            messageRoom.AcceptSection();

        }



    }

    //处理是否接收情报的请求
    public void RequestAcceptMessage(MsgPack pack)
    {
        PlayerData playerData = cacheSvc.GetPlayerDataByToken(pack.token);
        MessageRoom messageRoom = cacheSvc.GetMessageRoomByToken(pack.token);

        if (pack.msg.requestAcceptMessage.isAccept)
        {
            messageRoom.AcceptMessage();
        }
        else
        {
            if(messageRoom.transferingMessage.type == CardType.NonstopMessage)
            {
                //TODO
                int index = messageRoom.GetIndexById(playerData.id);

                int transIndex = -1;
                int tgIndex = -1;

                if(index == messageRoom.transferingMessageIndex)
                {
                    transIndex = messageRoom.transferingMessageIndex;
                    tgIndex = messageRoom.roundPlayerIndex;
                    messageRoom.SetTransferMessage(tgIndex);

                }


                if (tgIndex >= 0)
                {
                    GameMsg msg = new GameMsg
                    {
                        cmd = CMD.PushMessageTransfering,
                        pushMessageTransfering = new PushMessageTransfering
                        {
                            message = messageRoom.transferingMessage,
                            transferIndex = transIndex,
                            targetIndex = tgIndex

                        }
                    };
                    CacheSvc.Instance.SendMsgAll(messageRoom, msg);
                }
                else
                {
                    //TODO

                }

            }
            else
            {

                messageRoom.SetTransferMessage(messageRoom.transferingMessageIndex + 1);
                int index = messageRoom.GetIndexById(playerData.id);

                GameMsg msg = new GameMsg
                {
                    cmd = CMD.PushMessageTransfering,
                    pushMessageTransfering = new PushMessageTransfering
                    {
                        message = messageRoom.transferingMessage,
                        targetIndex = messageRoom.transferingMessageIndex
                    }
                };
                CacheSvc.Instance.SendMsgAll(messageRoom, msg);
            }
            

        }
    }
}

