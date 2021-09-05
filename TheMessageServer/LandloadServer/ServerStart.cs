//服务器入口
using PEUtils;
using System;
using System.Threading;

class ServerStart
    {
        static void Main(string[] args)
        {
        PELog.InitSettings();
        PELog.ColorLog(LogColor.Green,"Landlord Server Start ...");
        ServerRoot.Instance.Init();
        while (true)
        {
            Thread.Sleep(50);
            ServerRoot.Instance.Update();
        }
        }
    }

