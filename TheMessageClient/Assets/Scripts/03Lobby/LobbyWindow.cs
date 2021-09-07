//大厅界面



using PEProtocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class LobbyWindow : WindowRoot
{
    public Text txtName;
    public Text txtLevel;
    public Text txtCoin;
    public Text txtDmd;
    public Image prgExp;
    public Image headpPortrait;

    public GameObject roomInfo;
    public GameObject[] room;
    public Text[] txtRoomOwner;
    public Text[] txtNumber;
    public Text[] playerName;
    public Image[] playerHeadPortrait;
    public Image[] playerReady;
    public Button btnStart;


    private AudioSvc audioSvc;
    private NetSvc netSvc;
    private LobbySys lobbySys;

    public override void  InitWindow(){
        base.InitWindow();
        audioSvc = AudioSvc.Instance;
        netSvc = NetSvc.Instance;
        lobbySys = LobbySys.Instance;

        for(int i = 0; i < room.Length; i++)
        {
            room[i].SetActive(false);
        }

        roomInfo.SetActive(false);
    }
    public void RefreshWindow(){
        string path = "ResImages/Lobby/icon_";
        var playerData = lobbySys.playerData;
        SetText(txtName, playerData.name);
        SetText(txtLevel, playerData.lv);
        SetText(txtCoin, playerData.coin);
        SetText(txtDmd, playerData.diamond);
        SetSprite(headpPortrait, path + playerData.iconIndex);
        prgExp.fillAmount = playerData.exp*1.0f/PECommon.GetLevelUpExpCount(playerData.lv);

    }

    public void ClickScoreRankBtn(){
        audioSvc.PlayUIAudio(Constants.NormalClick);
        TipsWindow.AddTips("正在开发中...");
    }
    public void ClickArenaRankBtn(){
        audioSvc.PlayUIAudio(Constants.NormalClick);
        TipsWindow.AddTips("正在开发中...");
    }
    public void ClickMatchBtn(){
        audioSvc.PlayUIAudio(Constants.NormalClick);
        lobbySys.ReqMatch();
    }
    public void ClickSetBtn(){
        audioSvc.PlayUIAudio(Constants.NormalClick);
        TipsWindow.AddTips("正在开发中...");
    }
    public void ClickAddCoinBtn(){
        audioSvc.PlayUIAudio(Constants.NormalClick);
        TipsWindow.AddTips("正在开发中...");
    }
    public void ClickAddDmdBtn(){
        audioSvc.PlayUIAudio(Constants.NormalClick);
        TipsWindow.AddTips("正在开发中...");
    }
    public void ClickAddRoomBtn()
    {
        audioSvc.PlayUIAudio(Constants.NormalClick);
        lobbySys.RequestAddRoom();
    }
    public void ClickRefreshBtn()
    {
        audioSvc.PlayUIAudio(Constants.NormalClick);
        netSvc.SendMsg(new GameMsg { cmd = CMD.RequestRoomMsg });
        TipsWindow.AddTips("刷新成功");
    }

    public void ClickJoinRoomBtn0()
    {
        audioSvc.PlayUIAudio(Constants.NormalClick);
        netSvc.SendMsg(new GameMsg { cmd = CMD.RequestJoinRoomMsg,roomID = 0 });
        
    }

    public void ClickJoinRoomBtn1()
    {
        audioSvc.PlayUIAudio(Constants.NormalClick);
        netSvc.SendMsg(new GameMsg { cmd = CMD.RequestJoinRoomMsg, roomID = 1 });

    }

    public void ClickJoinRoomBtn2()
    {
        audioSvc.PlayUIAudio(Constants.NormalClick);
        netSvc.SendMsg(new GameMsg { cmd = CMD.RequestJoinRoomMsg, roomID = 2 });

    }

    public void ClickJoinRoomBtn3()
    {
        audioSvc.PlayUIAudio(Constants.NormalClick);
        netSvc.SendMsg(new GameMsg { cmd = CMD.RequestJoinRoomMsg, roomID = 3 });

    }

    public void ClickJoinRoomBtn4()
    {
        audioSvc.PlayUIAudio(Constants.NormalClick);
        netSvc.SendMsg(new GameMsg { cmd = CMD.RequestJoinRoomMsg, roomID = 4 });

    }

    public void ClickReady()
    {
        audioSvc.PlayUIAudio(Constants.NormalClick);
        netSvc.SendMsg(new GameMsg { cmd = CMD.RequestReady });
    }

    public void ClickGameStart()
    {
        audioSvc.PlayUIAudio(Constants.NormalClick);
        netSvc.SendMsg(new GameMsg { cmd = CMD.RequestGameStart});
    }

    public void SetRoomName(int roomID,string name)
    {
        txtRoomOwner[roomID].text = "房主：" + name;
    }

    public void SetRoomNumber(int roomID,int number)
    {
        txtNumber[roomID].text = number.ToString() + "/5";
    }

    public void SetRoomActive(int roomID,bool active)
    {
        room[roomID].SetActive(active);
    }

    public void ShowRoomDetails(DetailRoomMsg msg,bool isOwner)
    {
        string path = "ResImages/Lobby/icon_";
        roomInfo.SetActive(true);
        if (!isOwner)
        {
            btnStart.gameObject.SetActive(false);
        }
        for(int i = 0; i < msg.playerArr.Length; i++)
        {
            if (msg.playerArr[i] != null)
            {

                SetText(playerName[i], msg.playerArr[i].name);
                playerName[i].gameObject.SetActive(true);
                SetSprite(playerHeadPortrait[i], path + msg.playerArr[i].iconIndex);
                playerReady[i].gameObject.SetActive(false);

            }
            else
            {
                playerName[i].gameObject.SetActive(false);
                playerReady[i].gameObject.SetActive(false);
            }
        }
    }

    public void ShowReady(int posIndex)
    {
        playerReady[posIndex].gameObject.SetActive(true);
    }
}
