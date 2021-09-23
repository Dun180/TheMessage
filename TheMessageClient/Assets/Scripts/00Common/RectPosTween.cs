//位移动画组件


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class RectPosTween : MonoBehaviour
{
    private RectTransform rectTrans = null;

    private bool isRun = false;

    //运动参数
    private float moveTime = 0f;//运动时间
    private Vector3 targetPos = Vector3.zero;//目标位置
    private Vector3 moveVel = Vector3.zero;//运动速度
    private float countTime = 0;//运动时间计数
    private Action callback = null;//运动完成后回调


    private Vector3 startPos = Vector3.zero;//运动开始位置
    private void Awake()
    {
        rectTrans = GetComponent<RectTransform>();
    }

    //运动参数设置
    public void MoveLocalPosInTime(float time, Vector3 offset, Action cb = null)
    {
        startPos = rectTrans.localPosition;
        moveTime = time;
        targetPos = startPos + offset;
        moveVel = (targetPos - startPos) / moveTime;
        countTime = 0;
        callback = cb;
        isRun = true;
    }
    private void Update()
    {
        if (isRun)
        {
            float delt = Time.deltaTime;
            rectTrans.localPosition += moveVel * delt;
            countTime += delt;
            if (countTime >= moveTime)
            {
                rectTrans.localPosition = targetPos;

                isRun = false;
                moveTime = 0;
                targetPos = Vector3.zero;
                moveVel = Vector3.zero;
                countTime = 0;
                startPos = Vector3.zero;

                callback?.Invoke();
            }
        }
    }

    private void OnDisable()
    {
        isRun = false;
        moveTime = 0;
        targetPos = Vector3.zero;
        moveVel = Vector3.zero;
        countTime = 0;
        startPos = Vector3.zero;
        callback = null;
    }
}

