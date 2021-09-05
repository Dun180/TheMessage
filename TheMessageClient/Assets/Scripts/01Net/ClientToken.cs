//客户端会话连接token

using PENet;
using PEProtocol;
public class ClientToken : IOCPToken<GameMsg>
{
    protected override void OnConnected(){
        this.ColorLog(PEUtils.LogColor.Green,"Connect to Server Success.");
        TipsWindow.AddTips("连接服务器成功");
    }
    protected override void OnDisConnected(){
        this.ColorLog(PEUtils.LogColor.Yellow,"Disconnect to Server.");
        TipsWindow.AddTips("服务器连接断开");

    }
    protected override void OnReciveMsg(GameMsg msg){
        this.Log("RcvPack CMD:{0}",msg.cmd.ToString());
        NetSvc.Instance.AddMsgQue(msg);
    }
}
