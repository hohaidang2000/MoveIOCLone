using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    public bool start = false;
    public bool stop = false;
    // Start is called before the first frame update

    [SerializeField] GameObject[] spawnPoint;
    public int killed;
    public int currentSpawn=0;
    public int hasSpawn=0;
    [SerializeField] public int maxSpawn = 4;
    [SerializeField] public int allSpawn = 20;
    public bool reachAllSpawn = false;
    public Wave[] waves;

    private int previousI;

    public float timeBetweenSpawn = 1f;
    private float timeBetweenSpawnClamp;
    void Start()
    {
        //currentSpawn = 0;
        //hasSpawn = 0;
        killed = 0;
        timeBetweenSpawnClamp = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
 
        if(killed >= allSpawn)
        {
            reachAllSpawn = true;
        }
        else if (start && killed + currentSpawn < allSpawn && Time.time > timeBetweenSpawnClamp + timeBetweenSpawn)
        {
            if (currentSpawn < maxSpawn)
            {
                int i = Random.Range(0, spawnPoint.Length);
                if (previousI != i)
                {
                    previousI = i;

                    SpawnWave();
                }
            }
            
        }
        
    }
    private void SpawnWave()
    {
        
            timeBetweenSpawnClamp = Time.time;
            currentSpawn++;
            hasSpawn++;
            
            Instantiate(waves[0].enemies[0], spawnPoint[previousI].transform);
        

    }

    [System.Serializable]
    public class Wave
    {
        public Enemy[] enemies;
    }
}
