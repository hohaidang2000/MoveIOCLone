using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameplaySystem : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] MainCanvasButton mainCanvas;
    [SerializeField] public WaveSpawner wave;
    [SerializeField] PlayerTouchMovement playerTouchMovement;
    [SerializeField] Camera _camera;
    [SerializeField] Transform mainCameraTransform;
    [SerializeField] Transform spanInPosition;
    [SerializeField] Button ResetButton;
    [SerializeField] public Text text;

    public int firstSpawn = 1;
    public bool isDeath = false;
    public bool isPausing = false;
    

    private void Start()
    {
        //_camera.transform.position = new Vector3();
        playerTouchMovement.enabled = false;
        isPausing = true;
        wave.currentSpawn = firstSpawn;
        wave.hasSpawn = firstSpawn;
        _camera = Camera.main;
        mainCameraTransform = GameObject.FindWithTag("SpanOutPosition").transform;
        spanInPosition = GameObject.FindWithTag("SpanInPosition").transform;
        _camera.transform.position = spanInPosition.position;
        _camera.transform.rotation = spanInPosition.rotation;
    }
    private void Awake()
    {
        
    }
    // Update is called once per frame
    void Update()
    {
        isDeath = playerTouchMovement.isDeath;
        if (isDeath && isPausing == false)
        {
            playerTouchMovement.enabled = false;
            isPausing = true;
            PLayerDeath();
        }else if (wave.reachAllSpawn && isPausing == false)
        {
           
            isPausing = true;
            PlayerWin();
        }
        text.text = (wave.allSpawn - wave.killed).ToString();
    }

    private void PlayerWin()
    {
        _camera.transform.position = spanInPosition.position;
        _camera.transform.rotation = spanInPosition.rotation;
        playerTouchMovement.Win();
        playerTouchMovement.enabled = false;

        mainCanvas.Win();
    }

    private void PLayerDeath()
    {
        _camera.transform.position = spanInPosition.position;
        _camera.transform.rotation = spanInPosition.rotation;

        mainCanvas.Death();

    }

    public void StartLevel()
    {
        isPausing = false;
        playerTouchMovement.enabled = true;
        wave.start = true;
        _camera.transform.position = mainCameraTransform.position;
        _camera.transform.rotation = mainCameraTransform.rotation;
        mainCanvas.Started();
    }

    public void ResetLevel()
    {
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
    }
    
}
