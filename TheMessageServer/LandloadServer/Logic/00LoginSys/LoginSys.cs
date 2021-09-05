//登陆系统
using System;
using System.Collections.Generic;
using System.Text;
using PEProtocol;

public class LoginSys
{
    private static LoginSys instance = null;
    public static LoginSys Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new LoginSys();

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

    public void ReqLogin(MsgPack pack)
    {
        ReqLogin data = pack.msg.reqLogin;

        GameMsg msg = new GameMsg
        {
            cmd = CMD.RspLogin,
        };

        if (cacheSvc.IsAcctOnline(data.acct))
        {
            msg.err = ErrorCode.AcctIsOnline;
        }
        else {
            PlayerData playerData = cacheSvc.GetPlayerData(data.acct, data.pass);
            if(playerData == null)
            {
                msg.err = ErrorCode.WrongPass;
            }
            else
            {
                msg.rspLogin = new RspLogin
                {
                    playerData = playerData
                };
            }

            cacheSvc.AcctOnline(data.acct, pack.token, playerData);

        }

        pack.token.SendMsg(msg);
    }

    public void ClearOfflineData(ServerToken token)
    {
        cacheSvc.AcctOffline(token);
    }
}


