//’Ω∂∑ΩÁ√Ê


using PEProtocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessageWindow : WindowRoot
{

    public Image[] charImg;

    public Text[] charName;
    public Text[] playerName;
    public Text[] cards;
    public Text[] redNum;
    public Text[] blueNum;
    public Text[] blackNum;


    private AudioSvc audioSvc;
    private NetSvc netSvc;

    private int selfIndex;


    public override void InitWindow()
    {
        base.InitWindow();
        audioSvc = AudioSvc.Instance;
        netSvc = NetSvc.Instance;

        selfIndex = -1;

    }

    public void RefreshMessage(int index, MatchPlayerData[] matchplayer)
    {
        string path = "ResImages/Char/Char_";
        if (selfIndex < 0)
        {
            selfIndex = index;
        }

        for (int i = 0; i < matchplayer.Length; i++)
        {
            int flag = i+selfIndex;
            if (flag >= matchplayer.Length)
            {
                flag -= matchplayer.Length;
            }
            SetSprite(charImg[i], path + matchplayer[flag].charIndex);
            SetText(playerName[i], matchplayer[flag].name);
            SetText(cards[i], matchplayer[flag].cards);
            SetText(redNum[i], matchplayer[flag].redNum);
            SetText(blueNum[i], matchplayer[flag].blueNum);
            SetText(blackNum[i], matchplayer[flag].blackNum);
        }

    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
