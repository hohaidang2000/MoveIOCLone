using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SelfYeet : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] float time = 3f;
    [SerializeField] GameObject gameObject;
    [SerializeField] WaveSpawner wave;
    public PlayerTouchMovement playerThrower = null;
    public EnemyAI AIThrower = null;
    private float speed = 1000f;
    private void Awake()
    {
        //gameObject = GetComponent<GameObject>();

        
    }
    private void Start()
    {
        wave = GameObject.FindGameObjectWithTag("WaveSpawner").GetComponent<WaveSpawner>();
    }

    void SelfYeetTime()
    {
        Destroy(gameObject);
    }
    // Update is called once per frame
    void Update()
    {
        time -= Time.deltaTime;
        if(time <= 0)
        {
            SelfYeetTime();
        }
        transform.Rotate(0, -Time.deltaTime * speed, 0, Space.World);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "EnemyModel" && other.gameObject.GetComponentInParent<EnemyAI>().isDeath == false)
        {
            other.gameObject.GetComponentInParent<EnemyAI>().Death();
            GetBig();
            time = 0;
        }
        if(other.tag == "PlayerModel" && other.gameObject.GetComponentInParent<PlayerTouchMovement>().isDeath == false)
        {
            other.gameObject.GetComponentInParent<PlayerTouchMovement>().Death();
            GetBig();
            time = 0;
        }
    }
    private void GetBig()
    {
        wave.currentSpawn -= 1;
        wave.killed += 1;
        if (playerThrower != null)
        {
            playerThrower.GetBig();
        }
        if(AIThrower != null)
        {
            AIThrower.GetBig();
        }
    }
}
