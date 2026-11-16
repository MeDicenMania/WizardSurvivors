using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

/// <summary>
/// PATRÓN FACTORY + PATRÓN OBJECT POOL (UnityEngine.Pool):
/// Combina el Dictionary del Factory con un ObjectPool<EnemyStats> para cada tipo
/// de enemigo, pre-instanciando los enemigos desactivados al inicio y activándolos/desactivándolos.
/// </summary>
public class EnemyFactory : MonoBehaviour
{
    [Header("Prefabs de Enemigos (Factory)")]
    [SerializeField] private EnemyStats[] enemies;

    [Header("Configuración del ObjectPool")]
    [SerializeField] private bool collectionCheck = false;
    [SerializeField] private int defaultCapacity = 10;
    [SerializeField] private int maxSize = 100;

    private Dictionary<string, EnemyStats> idToEnemy = new Dictionary<string, EnemyStats>();
    private Dictionary<string, ObjectPool<EnemyStats>> enemyPools = new Dictionary<string, ObjectPool<EnemyStats>>();

    private void Awake()
    {
        InitializeFactoryAndPools();
    }

    private void InitializeFactoryAndPools()
    {
        if (enemies == null) return;

        foreach (var enemyPrefab in enemies)
        {
            if (enemyPrefab != null)
            {
                RegisterEnemyPrefab(enemyPrefab.Id, enemyPrefab);
            }
        }
    }

    public void RegisterEnemyPrefab(string id, EnemyStats enemyPrefab)
    {
        if (string.IsNullOrEmpty(id) || enemyPrefab == null) return;
        if (enemyPools.ContainsKey(id)) return;

        idToEnemy[id] = enemyPrefab;

        EnemyStats capturedPrefab = enemyPrefab;
        string capturedId = id;

        ObjectPool<EnemyStats> pool = new ObjectPool<EnemyStats>(
            () => CreatePoolItem(capturedId, capturedPrefab),
            OnTakeFromPool,
            OnReturnedToPool,
            OnDestroyPoolObject,
            collectionCheck,
            defaultCapacity,
            maxSize
        );

        enemyPools.Add(id, pool);


        List<EnemyStats> prewarmList = new List<EnemyStats>(defaultCapacity);
        for (int i = 0; i < defaultCapacity; i++)
        {
            prewarmList.Add(pool.Get());
        }
        for (int i = 0; i < prewarmList.Count; i++)
        {
            pool.Release(prewarmList[i]);
        }
    }

    private EnemyStats CreatePoolItem(string poolId, EnemyStats prefab)
    {
        EnemyStats enemy = Instantiate(prefab);
        enemy.gameObject.SetActive(false);

        if (enemyPools.TryGetValue(poolId, out ObjectPool<EnemyStats> pool))
        {
            enemy.SetPool(pool);
        }

        return enemy;
    }

    private void OnTakeFromPool(EnemyStats enemy)
    {
        enemy.gameObject.SetActive(true);
    }

    private void OnReturnedToPool(EnemyStats enemy)
    {
        enemy.gameObject.SetActive(false);
    }

    private void OnDestroyPoolObject(EnemyStats enemy)
    {
        Destroy(enemy.gameObject);
    }


    public IEnemy Create(string id, Vector3 spawnPosition)
    {
        if (!enemyPools.TryGetValue(id, out ObjectPool<EnemyStats> pool))
        {
            Debug.LogWarning($"[EnemyFactory] No se encontró ningún enemigo/pool con el ID '{id}'.");
            return null;
        }

        EnemyStats enemyInstance = pool.Get();
        enemyInstance.SetPool(pool);
        enemyInstance.Initialize(spawnPosition);

        return enemyInstance;
    }

    public IEnemy Create(string id)
    {
        return Create(id, Vector3.zero);
    }
}
