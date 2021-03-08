using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Timer
{
    public int frametime = 30;
    [HideInInspector]
    public int framereset = 30;

    public void Init()
    {
        framereset = frametime;
    }

    public int Elapsed()
    {
        return framereset - frametime;
    }

    public bool TimerReached()
    {
        return frametime <= 0;
    }

    public bool TimerMax()
    {
        return frametime == framereset;
    }

    public void Reset()
    {
        frametime = framereset;
    }

    public void Finish()
    {
        frametime = 0;
    }

    public void Increment()
    {
        ++frametime;
    }

    public void Decrement()
    {
        --frametime;
    }
}
