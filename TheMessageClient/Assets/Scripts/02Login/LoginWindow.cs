//登录窗口

using UnityEngine;
using UnityEngine.UI;
public class LoginWindow : WindowRoot
{

    public InputField iptAcct;
    public InputField iptPass;

    private AudioSvc audioSvc;
    private LoginSys loginSys;


    public override void  InitWindow(){
        base.InitWindow();
        audioSvc = AudioSvc.Instance;
        loginSys = LoginSys.Instance;
    }

    public void ClickLoginBtn(){
        audioSvc.PlayUIAudio(Constants.NormalClick);

        if(iptAcct.text.Length > 0 && iptPass.text.Length > 0){
            TipsWindow.AddTips("开始登录");
            loginSys.ReqLogin(iptAcct.text,iptPass.text);

        }else{
            TipsWindow.AddTips("账号或密码为空");
        }
    }
}
