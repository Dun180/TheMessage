//登录系统

using UnityEngine;
using PEProtocol;
public class LoginSys : MonoBehaviour
{

    public static LoginSys Instance;

    public LoginWindow loginWindow;
    public TipsWindow tipsWindow;
    private NetSvc netSvc;
    private LobbySys lobbySys;
    private void Awake(){
        Instance = this;
    }
    public void Init()
    {
        netSvc =NetSvc.Instance;
        lobbySys = LobbySys.Instance;
    }

    public void StartGame(){
        loginWindow.InitWindow();
        loginWindow.SetWindowState();
        tipsWindow.InitWindow();
        tipsWindow.SetWindowState();
    }

    public void ReqLogin(string acct,string pass){
        GameMsg msg = new GameMsg{
            cmd = CMD.ReqLogin,
            reqLogin = new ReqLogin { acct = acct, pass = pass}

        };
        netSvc.SendMsg(msg);
    }

    public void RspLogin(GameMsg msg){
        if(msg.err == ErrorCode.AcctIsOnline){
            TipsWindow.AddTips("当前账号已经上线");
        }else if(msg.err == ErrorCode.WrongPass){
            TipsWindow.AddTips("密码错误");
        }else{
            TipsWindow.AddTips("登录成功");
            lobbySys.SetPlayerData(msg.rspLogin);
            lobbySys.EnterLobby();
            loginWindow.SetWindowState(false);
        }
    }
}
