using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Testing : MonoBehaviour
{
    // Start is called before the first frame update
    //private FunctionTimer timer;
    private void Start()
    {
        //timer = new FunctionTimer(TestingAction, 2f);
        FunctionTimer.Create(TestingAction, 3f,"Timer");
        FunctionTimer.Create(TestingAction_2, 4f,"Timer2");
        
    }
    private void Update()
    {
        //timer.Update();
    }
    private void TestingAction()
    {
        Debug.Log("Testing");
        //FunctionTimer.StopTimer("Timer2");

    }
    private void TestingAction_2()
    {
        
        Debug.Log("Testing 2");
    }
}
