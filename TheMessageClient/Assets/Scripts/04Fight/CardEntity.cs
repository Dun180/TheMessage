//牌的实体类

using PEProtocol;
using System;
using UnityEngine;
using UnityEngine.UI;

public enum CardState
{
    Normal,
    Prepare
}

public class CardEntity
{
    public int index = -1;
    public RectTransform mRectTrans;
    public Card cardData;
    public CardState mState = CardState.Normal;

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
        Sprite sp = GetRes<Sprite>("ResImages/Message/" + spName);
        img.sprite = sp;
    }

    protected T GetRes<T>(string path) where T : UnityEngine.Object
    {
        return Resources.Load(path, typeof(T)) as T;
    }

    //相对移动动画
    public void MoveLocalPosInTime(float time, Vector3 offset, Action cb = null)
    {
        RectPosTween rpt = (RectPosTween)GetOrAddComponent<RectPosTween>(mRectTrans.gameObject);
        rpt.MoveLocalPosInTime(time, offset, cb);
    }

    //绝对移动动画
    public void MoveTargetPosInTime(float time, Vector3 target, Action cb = null)
    {
        RectPosTween rpt = (RectPosTween)GetOrAddComponent<RectPosTween>(mRectTrans.gameObject);
        rpt.MoveTargetPosInTime(time, target, cb);
    }

    //状态变化
    public void SetCardEntityState(CardState state,bool move = true, bool ani = true)
    {
        if(mState == state)
        {
            return;
        }

        mState = state;

        if (move)
        {
            Vector3 pos = mRectTrans.localPosition;//记录牌当前位置
            if(mState == CardState.Prepare)
            {
                if (ani)
                {
                    MoveTargetPosInTime(Constants.moveTime / 2, new Vector3(index * Constants.cardDistance, Constants.upDistance, 0));
                }
                else
                {
                    mRectTrans.localPosition = new Vector3(index * Constants.cardDistance, Constants.upDistance, 0);

                }

            }
            else
            {
                if (ani)
                {
                    MoveTargetPosInTime(Constants.moveTime / 2, new Vector3(index * Constants.cardDistance,0, 0));
                }
                else
                {
                    mRectTrans.localPosition = new Vector3(index * Constants.cardDistance, 0, 0);

                }
            }
        }
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

