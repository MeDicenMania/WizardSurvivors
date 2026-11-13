using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class Spawner : MonoBehaviour
{
    [System.Serializable]
    public class Wave
    {
        public string waveName;
        public List<EnemyGroup> enemyGroups;// una lista del grupo de enemigos a spawnear 
        public int waveQuota;         //total de enemigos en la oleada
        public float spawnInterval;   // tiempo entre spawns
        public int spawnCount;         //enemigos spawneados en la wave
      
    }
    private void Start()
    {
        CalculateWaveQuota();
        player = FindAnyObjectByType<PlayerStats>().transform;
      
       
    }
    [System.Serializable]
    public class EnemyGroup
    {
        public string enemyName;
        public int enemyCount; //numero de enemigos a spawnear
        public int spawnCount;  //enemigos ya spawneados spawneados
        public GameObject enemyPrefab;
    }

    public List<Wave> waves;     //lista de todas las oleadas
    public int currentWaveCount; //indice de la oleada actual

    [Header("Spawn specs")]
    float spawnTimer; //tiempo de cuando spawnear el proximo enemigo
    public float waveInterval;    //intervalo de tiempo entre cada oleada sin matarlas 
    public int enemiesAlive;
    public int maxEnemiesAllowed;
    public bool maxEnemiesReached;


    [Header("Spawn Positions")]
    public List<Transform> relativeSpawnPoints;

    Transform player;
     void Update()
    {

        if( currentWaveCount < waves.Count && waves[currentWaveCount].spawnCount == 0)
        {
            StartCoroutine(BeginNextWave());
        }

        spawnTimer += Time.deltaTime;
        if( spawnTimer >= waves[currentWaveCount].spawnInterval)
        {
            spawnTimer = 0;
            SpawnEnemies();
        }
    }

    IEnumerator BeginNextWave()
    {
        //
        yield return new WaitForSeconds(waveInterval);

        // si hay mas oleadas se mueve a la siguente digamos
        if(currentWaveCount < waves.Count -1)
        {
            currentWaveCount++;
            CalculateWaveQuota();
        }
    }

    public void CalculateWaveQuota()
    {

        int currentWaveQuota = 0;
 
        foreach (var enemyGroup in waves[currentWaveCount].enemyGroups)
        {
            currentWaveQuota += enemyGroup.enemyCount;
        }

        waves[currentWaveCount].waveQuota = currentWaveQuota;

        Debug.LogWarning("TOTAL: " + currentWaveQuota);
    }


    void SpawnEnemies()
    {
        if (waves[currentWaveCount].spawnCount < waves[currentWaveCount].waveQuota && !maxEnemiesReached)
        {
            //spawnea cada enemigo hasta que se llene la quota  
            foreach(var enemyGroup in waves[currentWaveCount].enemyGroups)
            {
                //chequea si el minimo de enemigos de un tipo ya fue spawneado
                if( enemyGroup.spawnCount < enemyGroup.enemyCount)
                {
                   if(enemiesAlive >= maxEnemiesAllowed)
                    {
                        maxEnemiesReached = true;
                        return;
                    }


                    Instantiate(enemyGroup.enemyPrefab, player.position + relativeSpawnPoints[Random.Range(0, relativeSpawnPoints.Count)].position, Quaternion.identity);

                    Vector2 spawnPositions = new Vector2(player.transform.position.x + Random.Range(-10f,10f),player.transform.position.y + Random.Range(-10f,10f));
                   

                    enemyGroup.spawnCount++;
                    waves[currentWaveCount].spawnCount++;
                    enemiesAlive++;
                }
            }
        }

        if( enemiesAlive < maxEnemiesAllowed)
        {
            maxEnemiesReached = false; 
        }
    }


    public void OnEnemyKill()
    {
        enemiesAlive--;
    }

}
