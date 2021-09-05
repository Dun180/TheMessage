//服务端会话连接Token

using PENet;
using PEProtocol;
using System;
using System.Collections.Generic;
using System.Text;


public class ServerToken : IOCPToken<GameMsg>
{
    protected override void OnConnected()
    {
        this.Log("Client Online");
        NetSvc.Instance.AddMsgQue(this, new GameMsg { cmd = CMD.OnConnected });

    }

    protected override void OnDisConnected()
    {
        NetSvc.Instance.AddMsgQue(this, new GameMsg { cmd = CMD.OnDisConnected});

    }

    protected override void OnReciveMsg(GameMsg msg)
    {
        this.Log("RcvPack CMD:{0}", msg.cmd.ToString());
        NetSvc.Instance.AddMsgQue(this, msg);
    }
}

