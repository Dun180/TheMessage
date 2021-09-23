using PEProtocol;
using System;
using UnityEngine;
using UnityEngine.UI;

public class CardEntity
{
    public int index = -1;
    public RectTransform mRectTrans;
    public Card cardData;

    public CardEntity(RectTransform rectTrans, int index)
    {
        mRectTrans = rectTrans;
        this.index = index;
    }

    public void SetRectPos(Vector3 pos)
    {
        mRectTrans.localPosition = pos;
    }

    public void SetEntityData(Card card)
    {
        cardData = card;
        Image img = mRectTrans.GetComponent<Image>();
        string spName = Enum.GetName(typeof(CardColor), card.color) + Enum.GetName(typeof(CardType), card.type) + Enum.GetName(typeof(CardFunction), card.function);
        this.Log("spName:{0}", spName);
        Sprite sp = GetRes<Sprite>("ResImages/Message/" + spName);
        img.sprite = sp;
    }

    protected T GetRes<T>(string path) where T : UnityEngine.Object
    {
        return Resources.Load(path, typeof(T)) as T;
    }

    //移动动画
    public void MoveLocalPosInTime(float time, Vector3 offset, Action cb = null)
    {
        RectPosTween rpt = (RectPosTween)GetOrAddComponent<RectPosTween>(mRectTrans.gameObject);
        rpt.MoveLocalPosInTime(time, offset, cb);
    }

    //把组件挂载到对应牌的物体上
    Component GetOrAddComponent<T>(GameObject go) where T : Component
    {
        T cpt = go.GetComponent<T>();
        if (cpt == null)
        {
            cpt = go.AddComponent<T>();
        }
        return cpt;
    }
}

