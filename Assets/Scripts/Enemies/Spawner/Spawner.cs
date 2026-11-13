using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class Spawner : MonoBehaviour
{


    SimpleLinkedPriorityQueue<EnemyGroup> enemyQueue;
    bool isDifficultWave;

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
        player = FindAnyObjectByType<PlayerStats>().transform;
        enemyQueue = new SimpleLinkedPriorityQueue<EnemyGroup>();

        CalculateWaveQuota();
        CheckDifficultWave();


        Debug.Log("Elementos en cola al iniciar: " + enemyQueue.Count);


    }
    [System.Serializable]
    public class EnemyGroup
    {
        public string enemyName;
        public int enemyCount; //numero de enemigos a spawnear
        public int spawnCount;  //enemigos ya spawneados spawneados
        public GameObject enemyPrefab;


        public int priority; //PRIORIDAD A LA HORA DE SPAWNEAR
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
    void CheckDifficultWave()
    {
        isDifficultWave = Random.Range(1,1) == 1;

        if (isDifficultWave)
        {
                                                   //  Debug.Log("oleada rapida ns");

            CalculateEnemyPriorities();
            FillEnemyQueue();
        }
        else
        {
                                                   //Debug.Log("Oleada normal");
        }
    }


    void CalculateEnemyPriorities()
    {
        foreach (var enemyGroup in waves[currentWaveCount].enemyGroups)
        {
            EnemyStats enemyStats = enemyGroup.enemyPrefab.GetComponent<EnemyStats>();

            float speed = enemyStats.enemyData.MoveSpeed;

                                                                                       //busca el mas rapido
            enemyGroup.priority = Mathf.RoundToInt(100 - speed);

           
        }
    }

    void FillEnemyQueue()
    {
        enemyQueue.Clear();

        foreach (var enemyGroup in waves[currentWaveCount].enemyGroups)
        {
            if (enemyGroup.spawnCount < enemyGroup.enemyCount)
            {
                Debug.Log("Agregando a cola: " + enemyGroup.enemyName);
                enemyQueue.Enqueue(enemyGroup, enemyGroup.priority);
            }
        }
        Debug.Log("Elementos en cola: " + enemyQueue.Count);
    }




    IEnumerator BeginNextWave()
    {
        
        yield return new WaitForSeconds(waveInterval);

        // si hay mas oleadas se mueve a la siguente digamos
        if(currentWaveCount < waves.Count -1)
        {
            currentWaveCount++;
            CalculateWaveQuota();
            CheckDifficultWave();

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
        if (waves[currentWaveCount].spawnCount >= waves[currentWaveCount].waveQuota)
            return;

        if (enemiesAlive >= maxEnemiesAllowed)
        {
            maxEnemiesReached = true;
            return;
        }

        EnemyGroup enemyGroup = null;

        if (isDifficultWave)
        {
            if (enemyQueue.IsEmpty)
                return;

            enemyGroup = enemyQueue.Dequeue();
        }
        else
        {
                                                                               // Si es Oleada normal sigue  el orden de la lista comun 
            foreach (var group in waves[currentWaveCount].enemyGroups)
            {
                if (group.spawnCount < group.enemyCount)
                {
                    enemyGroup = group;
                    break;
                }
            }
        }

        if (enemyGroup == null)
            return;

        Instantiate(
            enemyGroup.enemyPrefab,
            player.position + relativeSpawnPoints[Random.Range(0, relativeSpawnPoints.Count)].position,
            Quaternion.identity
        );

        enemyGroup.spawnCount++;
        waves[currentWaveCount].spawnCount++;
        enemiesAlive++;

        if (isDifficultWave && enemyGroup.spawnCount < enemyGroup.enemyCount)
        {
            enemyQueue.Enqueue(enemyGroup, enemyGroup.priority);
        }

        maxEnemiesReached = enemiesAlive >= maxEnemiesAllowed;
    }

    public void OnEnemyKill()
    {
        enemiesAlive--;
    }



}

