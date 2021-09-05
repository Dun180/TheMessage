//Tips弹窗
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class TipsWindow : WindowRoot
{
    public Transform tipsRoot;
    public Transform tipsBg;
    public Animator ani;
    public Text txtTips;
    
    private bool isTipsShow = false;
    private static Queue<string> tipsQue = new Queue<string>();

    public override void InitWindow()
    {
        base.InitWindow();
        SetActive(tipsRoot,false);
    }

    public static void AddTips(string tips){
        lock(tipsQue){
            tipsQue.Enqueue(tips);
        }
    }

    private void Update(){
        if(tipsQue.Count > 0 && isTipsShow == false){
            lock(tipsQue){
                string tips = tipsQue.Dequeue();
                isTipsShow = true;
                SetTips(tips);
            }
        }
    }

    private void SetTips(string tips){
        int len = tips.Length;
        SetText(txtTips,tips);
        tipsBg.GetComponent<RectTransform>().sizeDelta = new Vector2(25*len+50,55);
        SetActive(tipsRoot,true);
        ani.Play("TipsShowAni",0,0);
    }

    public void DisableTips(){
        SetActive(tipsRoot,false);
        isTipsShow = false;
    }
}
