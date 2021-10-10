//登录窗口

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class LoginWindow : WindowRoot
{

    public InputField iptAcct;
    public InputField iptPass;

    private AudioSvc audioSvc;
    private LoginSys loginSys;

    public LineRenderer line;

    public RectTransform pos;

    public override void  InitWindow(){
        base.InitWindow();
        audioSvc = AudioSvc.Instance;
        loginSys = LoginSys.Instance;
    }

    public void ClickLoginBtn(){

        pos.localPosition = new Vector3(50, 50, -1);
        pos.DOLocalMove(new Vector3(200, 200, -1), 1);


        line.SetPosition(0, new Vector3(0, 0, 0));
        line.SetPosition(1, new Vector3(50, 50, 0));

        audioSvc.PlayUIAudio(Constants.NormalClick);

        if(iptAcct.text.Length > 0 && iptPass.text.Length > 0){
            TipsWindow.AddTips("开始登录");
            loginSys.ReqLogin(iptAcct.text,iptPass.text);

        }else{
            TipsWindow.AddTips("账号或密码为空");
        }
    }



}
