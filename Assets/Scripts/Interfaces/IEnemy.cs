using UnityEngine;
using UnityEngine.Pool;

public interface IEnemy
{
    string Id { get; }
    GameObject GameObject { get; }
    void Initialize(Vector3 spawnPosition);
    void SetPool(IObjectPool<EnemyStats> pool);
    void TakeDamage(float damage);
    void Attack(PlayerStats player);
    void Deactivate();
}
