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
            case CMD.RspMatch:
                LobbySys.Instance.RspMatch(msg);
                break;
            case CMD.PshMatch:
                FightSys.Instance.PshMatch(msg.pshMatch);
                break;
            case CMD.ResponseAddRoom:
                LobbySys.Instance.ResponseAddRoom(msg);
                break;
            case CMD.ResponseRoomMsg:
                LobbySys.Instance.RefreshRoom(msg);
                break;
            case CMD.ResponseJoinRoomMsg:
                LobbySys.Instance.ResponseJoinRoomMsg(msg);
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
