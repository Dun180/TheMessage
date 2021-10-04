//网络服务
using UnityEngine;
using PENet;
using PEProtocol;
using PEUtils;
using System.Collections.Generic;
public class NetSvc : MonoBehaviour
{

    public static NetSvc Instance;


    private void Awake(){
        Instance = this;
    }

    IOCPNet<ClientToken,GameMsg> net = null;
    private Queue<GameMsg> msgQue;
    public void Init()
    {
        msgQue = new Queue<GameMsg>();

        IOCPTool.LogFunc = this.Log;
        IOCPTool.WarnFunc = this.Warn;
        IOCPTool.ErrorFunc = this.Error;
        IOCPTool.ColorLogFunc = (color, msg) =>
        {
            this.ColorLog((LogColor)color, msg);
        };
        net = new IOCPNet<ClientToken,GameMsg>();
        net.StartAsClient("127.0.0.1",17666);
    }

    public void AddMsgQue(GameMsg msg){
        lock(msgQue){
            msgQue.Enqueue(msg);
        }
    }

    public void Update(){
        while(msgQue.Count > 0){
            lock(msgQue){
                GameMsg msg = msgQue.Dequeue();
                HandleMsg(msg);
            }
        }
        
    }

    private void HandleMsg(GameMsg msg){
        switch(msg.cmd){
            case CMD.RspLogin:
                LoginSys.Instance.RspLogin(msg);
                break;
/*
            case CMD.PshMatch:
                FightSys.Instance.PshMatch(msg.pshMatch);
                break;*/
            case CMD.ResponseAddRoom:
                LobbySys.Instance.ResponseAddRoom(msg);
                break;
            case CMD.ResponseRoomMsg:
                LobbySys.Instance.RefreshRoom(msg);
                break;
            case CMD.PushJoinRoomMsg:
                LobbySys.Instance.PushJoinRoom(msg);
                break;
            case CMD.PushReady:
                LobbySys.Instance.PushReady(msg);
                break;
            case CMD.PushExitRoom:
                LobbySys.Instance.PushExitRoom(msg);
                break;
            case CMD.ResponseExitRoom:
                LobbySys.Instance.ResponseExitRoom();
                break;
            case CMD.PushGameStart:
                LobbySys.Instance.PushGameStart(msg);
                break;
            case CMD.ResponseRefreshMessage:
                FightSys.Instance.ResponseRefreshMessage(msg);
                break;
            case CMD.PushChar:
                FightSys.Instance.PushChar(msg);
                break;
            case CMD.PushSelectChar:
                FightSys.Instance.PushSelectChar(msg);
                break;
            case CMD.PushIdentityInfo:
                FightSys.Instance.PushIdentityInfo(msg);
                break;
            case CMD.PushCard:
                FightSys.Instance.PushCard(msg);
                break;
            case CMD.PushRoundStart:
                FightSys.Instance.PushRoundStart(msg);
                break;
            case CMD.PushDrawCard:
                FightSys.Instance.PushDrawCard(msg);
                break;
            case CMD.PushEndPlay:
                FightSys.Instance.PushEndPlay(msg);
                break;
            case CMD.PushMessageTransfer:
                FightSys.Instance.PushMessageTransfer(msg);
                break;
            case CMD.PushAcceptSection:
                FightSys.Instance.PushAcceptSection(msg);
                break;
            case CMD.PushMessageTransfering:
                FightSys.Instance.PushMessageTransfering(msg);
                break;
            case CMD.None:
                break;
            default:
                break;
        }
    }
    public void SendMsg(GameMsg msg){
        if(net != null&&net.token!=null){
            net.token.SendMsg(msg);
        }else{
            TipsWindow.AddTips("服务器未连接");
        }
    }

    public void UnInit(){
        net.CloseClient();
    }
}
