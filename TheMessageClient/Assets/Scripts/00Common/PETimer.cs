//定时工具

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class PETimerTask
{
    public float delayTime;//延迟开启时间
    public float endTime;//结束时间
    public float rateTime;//间隔调用时间，每过一秒时钟跳动一次
    public Action delayCB;//延迟回调函数
    public Action rateCB;//间隔时间回调函数
    public Action endCB;//结束回调函数

}

public class PETimer : MonoBehaviour
{
    //计时器状态定义
    enum CounterState
    {
        None,
        Delay,//延时状态
        Normal//常规状态
    }

    public bool isRun = false;
    private PETimerTask mTask = null;
    public float delayCount;
    public float endCount;
    public float rateCount;
    private CounterState counterState = CounterState.None;

    public void AddTimerTask(PETimerTask task)
    {
        mTask = task;
        if (task.delayTime > 0)
        {
            counterState = CounterState.Delay;
        }
        else
        {
            counterState = CounterState.Normal;
        }
        isRun = true;
    }

    private void Update()
    {
        if (isRun)
        {
            float delta = Time.deltaTime;
            if (counterState == CounterState.Delay)
            {
                DelayCounter(delta);
            }
            else
            {
                NormalCounter(delta);
            }
        }
    }

    void DelayCounter(float delta)
    {
        delayCount += delta;
        float delayOffset = delayCount - mTask.delayTime;
        if (delayOffset >= 0)
        {
            rateCount = delayOffset;
            endCount = delayOffset;
            if (mTask.delayCB != null)
            {
                mTask.delayCB();
                mTask.delayCB = null;
            }
            counterState = CounterState.Normal;
            NormalCounter(0);
        }
    }

    void NormalCounter(float delta)
    {
        if (mTask.rateTime > 0)
        {
            rateCount += delta;
            float rateOffset = rateCount - mTask.rateTime;
            if (rateOffset >= 0)
            {
                rateCount = rateOffset;
                mTask.rateCB?.Invoke();
            }
        }

        if (mTask.endTime != -1)
        {
            endCount += delta;
            float endOffset = endCount - mTask.endTime;
            if (endOffset >= 0)
            {
                mTask.endCB?.Invoke();
                OnDisable();
            }
        }
    }

    private void OnDisable()
    {
        isRun = false;
        mTask = null;
        counterState = CounterState.None;
        delayCount = 0;
        rateCount = 0;
        endCount = 0;
    }
}

