using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance { get; private set; }
    EnemyPool pool;
    private AVL<int> enemies = new AVL<int>();

    private Dictionary<int, GameObject> enemyObjects =
        new Dictionary<int, GameObject>();

    private Dictionary<GameObject, int> enemyIds =
        new Dictionary<GameObject, int>();

    private int nextId = 1;

    public int ActiveEnemyCount => enemyObjects.Count;

    private void Awake()
    {
        pool = FindAnyObjectByType<EnemyPool>();
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

      
        Instance = this;
    }

    public int RegisterEnemy(GameObject enemy)
    {
        if (enemy == null)
            return -1;

        // Evita registrar dos veces el mismo objeto.
        if (enemyIds.TryGetValue(enemy, out int existingId))
            return existingId;

        int id = nextId++;

      

        // Guardamos la relación entre ID y GameObject.
        enemyObjects.Add(id, enemy);
        enemyIds.Add(enemy, id);


        enemies.Insert(id);
        return id;
    }

    public void UnregisterEnemy(GameObject enemy)
    {
        if (enemy == null)
            return;

        if (!enemyIds.TryGetValue(enemy, out int id))
            return;

    
        enemies.Remove(id);

      
        enemyIds.Remove(enemy);
        enemyObjects.Remove(id);
        pool.RecicleEnemy(enemy);

        Debug.Log("Enemigo eliminado: " + id);
    }

    public bool IsRegistered(int id)
    {
        return enemies.Contains(id);
    }

    public GameObject GetEnemy(int id)
    {
        if (enemyObjects.TryGetValue(id, out GameObject enemy))
            return enemy;

        return null;
    }
}