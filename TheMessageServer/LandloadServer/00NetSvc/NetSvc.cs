//网络服务

using PENet;
using PEProtocol;
using PEUtils;
using System;
using System.Collections.Generic;
using System.Text;


public class MsgPack
{
    public ServerToken token;
    public GameMsg msg;
    public MsgPack(ServerToken token, GameMsg msg)
    {
        this.token = token;
        this.msg = msg;
    }
}

public class NetSvc
{
    private static NetSvc instance = null;
    public static NetSvc Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new NetSvc();

            }
            return instance;
        }
    }
    private Queue<MsgPack> msgPackQue = null;
    public void Init()
    {
        msgPackQue = new Queue<MsgPack>();

        IOCPTool.LogFunc = this.Log;
        IOCPTool.WarnFunc = this.Warn;
        IOCPTool.ErrorFunc = this.Error;
        IOCPTool.ColorLogFunc = (color, msg) =>
        {
            this.ColorLog((LogColor)color, msg);
        };

        IOCPNet<ServerToken, GameMsg> net = new IOCPNet<ServerToken, GameMsg>();
        net.StartAsServer("127.0.0.1",17666, 1000);
    }
    public void AddMsgQue(ServerToken token,GameMsg msg)
    {
        lock(msgPackQue){
            msgPackQue.Enqueue(new MsgPack(token, msg));

        }
    }
    public void Update()
    {
        while (msgPackQue.Count > 0)
        {
            lock (msgPackQue)
            {
                MsgPack pack = msgPackQue.Dequeue();
                HandleOutMsg(pack);
            }
        }
    }

    private void HandleOutMsg(MsgPack pack)
    {
        switch (pack.msg.cmd)
        {
            case CMD.OnConnected:
                this.Log("Assign TokenID:{0}", pack.token.tokenID);
                break;
            case CMD.OnDisConnected:
                this.ColorLog(PEUtils.LogColor.Yellow, "Token:{0} client offline.", pack.token.tokenID);
                MatchSys.Instance.RequestExitRoom(pack);
                LoginSys.Instance.ClearOfflineData(pack.token);
                
                break;
            //登录请求
            case CMD.ReqLogin:
                LoginSys.Instance.ReqLogin(pack);
                break;
            //匹配请求
            case CMD.RequestAddRoom:
                MatchSys.Instance.RequestAddRoom(pack);//添加房间请求
                break;
            case CMD.RequestRoomMsg:
                MatchSys.Instance.RequestRoomMsg(pack);
                break;
            case CMD.RequestJoinRoomMsg:
                MatchSys.Instance.RequestJoinRoom(pack);
                break;
            case CMD.RequestReady:
                MatchSys.Instance.RequestReady(pack);
                break;
            case CMD.RequestUnReady:
                MatchSys.Instance.RequestUnReady(pack);
                break;
            case CMD.RequestExitRoom:
                MatchSys.Instance.RequestExitRoom(pack);
                break;
            case CMD.RequestGameStart:
                MatchSys.Instance.RequestGameStart(pack);
                break;
            //游戏请求
            case CMD.RequestRefreshMessage:
                FightSys.Instance.RequestRefreshMessage(pack);
                break;
            case CMD.RequestMessageInfo:
                FightSys.Instance.RequestMessageInfo(pack);
                break;
            case CMD.RequestSelectChar:
                FightSys.Instance.RequestSelectChar(pack);
                break;
            case CMD.RequestOutCard:
                FightSys.Instance.RequestOutCard(pack);
                break;
            case CMD.RequestEndResponseStage:
                FightSys.Instance.RequestEndResponseStage(pack);
                break;
            case CMD.RequestDisCard:
                FightSys.Instance.RequestDisCard(pack);
                break;
            case CMD.RequestEndPlay:
                FightSys.Instance.RequestEndPlay(pack);
                break;
            case CMD.RequestBalanceInfo:
                FightSys.Instance.RequestBalanceInfo(pack);
                break;
            case CMD.RequestMessageTransfer:
                FightSys.Instance.RequestMessageTransfer(pack);
                break;
            case CMD.RequestAcceptMessage:
                FightSys.Instance.RequestAcceptMessage(pack);
                break;
            case CMD.None:
                break;
            default:
                break;
        }
    }
}
