using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainCanvasButton : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] GameObject startButton;
    [SerializeField] GameObject restartButton;
    [SerializeField] GameObject winButton;
    private void Awake()
    {
        
        restartButton = GameObject.Find("Restart Button");
        winButton = GameObject.Find("Win Button");
        startButton = GameObject.Find("Start Button");
        
    }
    void Start()
    {
        restartButton.SetActive(false);
        winButton.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Started()
    {
        startButton.SetActive(false);
    }
    public void Death()
    {
        startButton.SetActive(false);
        restartButton.SetActive(true);
    }
    public void Win()
    {
        winButton.SetActive(true);
    }
}
