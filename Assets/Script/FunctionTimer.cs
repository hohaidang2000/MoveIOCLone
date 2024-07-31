using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.CompilerServices;

public class FunctionTimer
{
    private static List<FunctionTimer> activeTimerList;
    private static GameObject initGameObject;
    private static void InitIfNeeded()
    {
        if (initGameObject == null)
        {
            initGameObject = new GameObject("FunctionTimer_InitGameObject");
            activeTimerList = new List<FunctionTimer> ();
        }

    }
    public static FunctionTimer Create(Action action, float timer, string timerName = null)
    {
        InitIfNeeded();
       
        GameObject gameObject = new GameObject("FunctionTimer", typeof(MonoBehaviourHook));
        
        FunctionTimer functionTimer = new FunctionTimer(action, timer, timerName, gameObject);
        
        gameObject.GetComponent<MonoBehaviourHook>().onUpdate = functionTimer.Update;

        activeTimerList.Add(functionTimer); 
        return functionTimer;
    }
    public static void RemoveTimer(FunctionTimer functionTimer)
    {
        InitIfNeeded();
        activeTimerList.Remove(functionTimer); 
    }

    public static void StopTimer(string timerName)
    {
        for(int i = 0;i<activeTimerList.Count; i++)
        {
            if (activeTimerList[i].timerName == timerName)
            {
                //stop timer
                activeTimerList[i].DestroySelf();
                i--;
            }
        }
    }

    public class MonoBehaviourHook:MonoBehaviour {
        public Action onUpdate;
        private void Update()
        {
            if (onUpdate != null) onUpdate();
        }
    }
    // Start is called before the first frame update
    private Action action;
    private float timer;
    private string timerName;
    public bool cancelTimer = false;
    public bool doneTimer = false;
    private GameObject gameObject;
    private bool isDestroyed;
    public FunctionTimer(Action action, float timer,string timerName, GameObject gameObject)
    {
        this.action = action;
        this.timer = timer;
        isDestroyed = false;
        this.gameObject = gameObject;
        this.timerName = timerName;
    }
    public void Update()
    {
        if (!isDestroyed)
        {
            if (!cancelTimer)
            {
                timer -= Time.deltaTime;
                if (timer < 0)
                {
                    bool doneTimer = true;
                    action();
                    DestroySelf();
                }
            }
        }
        
    }
    private void DestroySelf()
    {
        isDestroyed = true;
        UnityEngine.Object.Destroy(gameObject);
        RemoveTimer(this);
    }
}
