using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

/// <summary>
/// PATRÓN FACTORY + PATRÓN OBJECT POOL (UnityEngine.Pool):
/// Combina el Dictionary del Factory con un ObjectPool<EnemyStats> para cada tipo
/// de enemigo, devolviendo la interfaz IEnemy y reciclando instancias sin usar Destroy().
/// </summary>
public class EnemyFactory : MonoBehaviour
{
    [Header("Prefabs de Enemigos (Factory)")]
    [SerializeField] private EnemyStats[] enemies;

    [Header("Configuración del ObjectPool")]
    [SerializeField] private bool collectionCheck = false;
    [SerializeField] private int defaultCapacity = 10;
    [SerializeField] private int maxSize = 100;

    private Dictionary<string, EnemyStats> idToEnemy;
    private Dictionary<string, ObjectPool<EnemyStats>> enemyPools;

    private void Awake()
    {
        InitializeFactoryAndPools();
    }

    private void InitializeFactoryAndPools()
    {
        idToEnemy = new Dictionary<string, EnemyStats>();
        enemyPools = new Dictionary<string, ObjectPool<EnemyStats>>();

        if (enemies == null) return;

        foreach (var enemyPrefab in enemies)
        {
            if (enemyPrefab != null && !idToEnemy.ContainsKey(enemyPrefab.Id))
            {
                string id = enemyPrefab.Id;
                idToEnemy.Add(id, enemyPrefab);

                // Creamos un ObjectPool<EnemyStats> para este tipo de enemigo (como en el PDF de Pool)
                EnemyStats capturedPrefab = enemyPrefab;
                ObjectPool<EnemyStats> pool = new ObjectPool<EnemyStats>(
                    () => CreatePoolItem(capturedPrefab),
                    OnTakeFromPool,
                    OnReturnedToPool,
                    OnDestroyPoolObject,
                    collectionCheck,
                    defaultCapacity,
                    maxSize
                );

                enemyPools.Add(id, pool);
            }
        }
    }

    // =========================================================
    // 4 FUNCIONES DEL PATRÓN POOL (según diapositiva 8 del PDF)
    // =========================================================

    private EnemyStats CreatePoolItem(EnemyStats prefab)
    {
        EnemyStats enemy = Instantiate(prefab);
        enemy.gameObject.SetActive(false);

        if (enemyPools.TryGetValue(prefab.Id, out ObjectPool<EnemyStats> pool))
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

    // =========================================================
    // MÉTODO CREATE DEL FACTORY (usa pool.Get() y devuelve IEnemy)
    // =========================================================

    public IEnemy Create(string id, Vector3 spawnPosition)
    {
        if (idToEnemy == null || enemyPools == null)
        {
            InitializeFactoryAndPools();
        }

        if (!enemyPools.TryGetValue(id, out ObjectPool<EnemyStats> pool))
        {
            Debug.LogWarning($"[EnemyFactory] No se encontró ningún enemigo/pool con el ID '{id}'.");
            return null;
        }

        // Tomamos una instancia reciclada del pool en lugar de hacer Instantiate siempre
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
