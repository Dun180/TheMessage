//服务器根节点

    public class ServerRoot
    {
    private static ServerRoot instance = null;
    public static ServerRoot Instance
    {
        get
        {
            if(instance == null)
            {
                instance = new ServerRoot();

            }
            return instance;
        }
    }

    public void Init()
    {
        DBSvc.Instance.Init();
        CacheSvc.Instance.Init();
        NetSvc.Instance.Init();

        LoginSys.Instance.Init();
        MatchSys.Instance.Init();
        FightSys.Instance.Init();
    }

    public void Update()
    {
        DBSvc.Instance.Update();
        CacheSvc.Instance.Update();
        NetSvc.Instance.Update();

        LoginSys.Instance.Update();
        MatchSys.Instance.Update();
        FightSys.Instance.Update();
    }
    }

