//启动入口

using PEUtils;
using UnityEngine;



public class GameRoot : MonoBehaviour
{
    public Transform transCanvas;


    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < transCanvas.childCount; i++){
            transCanvas.GetChild(i).gameObject.SetActive(false);
        }



        LogConfig cfg = new LogConfig
        {
            enableThreadID = true,
            enableTrace = false,
            enableSave = true,
            enableCover = true,
            savePath = Application.persistentDataPath+"/PELog/",
            saveName = "LandlordClientPELog.txt",
            loggerEnum = LoggerType.Unity
        };
        PELog.InitSettings(cfg);
        PELog.Log("GameStart...");

#if UNITY_EDITOR||UNITY_STANDALONE_WIN
        Screen.SetResolution(667,375,false);
#endif

        //services
        NetSvc.Instance.Init();

        //systems
        LoginSys.Instance.Init();
        LobbySys.Instance.Init();
        FightSys.Instance.Init();

        LoginSys.Instance.StartGame();
    }

    private void OnApplicationQuit() {
        NetSvc.Instance.UnInit();
    }
}
