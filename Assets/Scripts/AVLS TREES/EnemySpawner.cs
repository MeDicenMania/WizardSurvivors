
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private Transform player;

    [Header("Generación")]
    [SerializeField] private float spawnInterval = 2f;
    [SerializeField] private float spawnDistance = 10f;
    [SerializeField] private int maxEnemies = 100;

    private float timer;

    private void Update()
    {
        if (enemyPrefab == null || player == null)
            return;

        if (EnemyManager.Instance == null)
            return;

        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            timer -= spawnInterval;

            if (EnemyManager.Instance.ActiveEnemyCount < maxEnemies)
                SpawnEnemy();
        }
    }

    private void SpawnEnemy()
    {
        Vector2 direction = Random.insideUnitCircle.normalized;

        // Evita que la dirección sea exactamente cero.
        if (direction.sqrMagnitude < 0.01f)
            direction = Vector2.right;

        Vector3 spawnPosition = player.position +
            new Vector3(direction.x, direction.y, 0f) * spawnDistance;

        GameObject enemy = Instantiate(
            enemyPrefab,
            spawnPosition,
            Quaternion.identity
        );

        EnemyManager.Instance.RegisterEnemy(enemy);
    }
}