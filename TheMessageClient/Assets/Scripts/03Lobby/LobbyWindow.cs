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
    public Button btnExit;

    public Text txtReady;
    public Text txtExit;

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
        switch (lobbySys.playerState)
        {
            case PlayerState.RoomOwner:
                audioSvc.PlayUIAudio(Constants.NormalClick);
                netSvc.SendMsg(new GameMsg { cmd = CMD.RequestGameStart });
                break;
            case PlayerState.Ready:
                audioSvc.PlayUIAudio(Constants.NormalClick);
                netSvc.SendMsg(new GameMsg { cmd = CMD.RequestUnReady });
                lobbySys.playerState = PlayerState.UnReady;
                break;
            case PlayerState.UnReady:
                audioSvc.PlayUIAudio(Constants.NormalClick);
                netSvc.SendMsg(new GameMsg { cmd = CMD.RequestReady });
                lobbySys.playerState = PlayerState.Ready;
                break;
            case PlayerState.None:
                break;
            default:
                break;
        }

    }

    public void ClickExitRoom()
    {
        audioSvc.PlayUIAudio(Constants.NormalClick);
        netSvc.SendMsg(new GameMsg { cmd = CMD.RequestExitRoom });
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

    public void ShowRoomDetails(DetailRoomMsg msg)
    {
        string path = "ResImages/Lobby/icon_";
        roomInfo.SetActive(true);
        if (lobbySys.playerState == PlayerState.RoomOwner)
        {
            SetText(txtReady, "开始");
        }
        else if(lobbySys.playerState == PlayerState.UnReady)
        {
            SetText(txtReady, "准备");
        }else if(lobbySys.playerState == PlayerState.Ready)
        {
            SetText(txtReady, "取消准备");
        }
        for(int i = 0; i < msg.playerArr.Length; i++)
        {
            if (msg.playerArr[i] != null)
            {

                SetText(playerName[i], msg.playerArr[i].name);
                playerName[i].gameObject.SetActive(true);
                SetSprite(playerHeadPortrait[i], path + msg.playerArr[i].iconIndex);
                playerReady[i].gameObject.SetActive(msg.playerArr[i].isReady);

            }
            else
            {
                playerName[i].gameObject.SetActive(false);
                playerReady[i].gameObject.SetActive(false);
                SetSprite(playerHeadPortrait[i], path + "-1");
            }
        }
    }

    public void ShowReady(int posIndex,bool isReady)
    {
        playerReady[posIndex].gameObject.SetActive(isReady);
        if (lobbySys.playerState == PlayerState.Ready)
        {
            SetText(txtReady, "取消准备");
        }else if(lobbySys.playerState == PlayerState.UnReady)
        {
            SetText(txtReady, "准备");
        }
    }

    public void ExitRoom()
    {

        roomInfo.SetActive(false);
    }
}
