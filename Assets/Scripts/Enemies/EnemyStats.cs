using UnityEngine;
using UnityEngine.Pool;

public class EnemyStats : MonoBehaviour, IEnemy
{
    [Header("Factory ID")]
    [SerializeField] protected string id;
    public string Id => string.IsNullOrEmpty(id) ? gameObject.name : id;
    public GameObject GameObject => gameObject;

    // Referencia al ObjectPool de UnityEngine.Pool (Patrón Pool del PDF)
    private IObjectPool<EnemyStats> objectPool;
    private bool isReleasedToPool = false;

    [Header("Stats (Flyweight)")]
    public EnemyScriptableOjbects enemyData;
    EnemyPool pool;
    public float currentMoveSpeed;
    public float currentHealth;
    public float currentDamage;

    public float despawnDistance = 40f;
    protected Transform player;

    protected virtual void Awake()
    {
        pool = FindAnyObjectByType<EnemyPool>();
        ResetStats();
    }

    protected virtual void Start()
    {
        PlayerStats playerStats = FindAnyObjectByType<PlayerStats>();
        if (playerStats != null)
        {
            player = playerStats.transform;
        }
    }

    // Método de IEnemy para asignar el pool al crearse
    public void SetPool(IObjectPool<EnemyStats> poolRef)
    {
        objectPool = poolRef;
    }

    // Método de IEnemy para resetear los valores originales al salir del Pool
    public virtual void Initialize(Vector3 spawnPosition)
    {
        isReleasedToPool = false;
        transform.position = spawnPosition;
        ResetStats();
    }

    protected void ResetStats()
    {
        if (enemyData != null)
        {
            currentDamage = enemyData.Damage;
            currentHealth = enemyData.MaxHealth;
            currentMoveSpeed = enemyData.MoveSpeed;
        }
    }

    protected virtual void Update()
    {
        if (player != null && Vector2.Distance(transform.position, player.position) >= despawnDistance)
        {
            ReturnEnemy();
        }
    }

    public virtual void TakeDamage(float damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Kill();
        }
    }

    public virtual void Attack(PlayerStats targetPlayer)
    {
        if (targetPlayer != null)
        {
            targetPlayer.TakeDamage(currentDamage);
        }
    }

    protected virtual void Kill()
    {
        if (isReleasedToPool) return;

        Spawner sp = FindAnyObjectByType<Spawner>();
        if (sp != null)
        {
            sp.OnEnemyKill();
        }

        // Soltar gema de experiencia antes de desactivar y devolver al Pool
        ExperienceDrop expDrop = GetComponent<ExperienceDrop>();
        if (expDrop != null)
        {
            expDrop.SpawnDrop();
        }

        Deactivate();
    }

    // Devuelve el enemigo al ObjectPool (como en la diapositiva 9 del PDF de Pool)
    public virtual void Deactivate()
    {
        if (objectPool != null)
        {
            if (!isReleasedToPool)
            {
                isReleasedToPool = true;
                objectPool.Release(this);
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    protected virtual void OnCollisionStay2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            PlayerStats targetPlayer = col.gameObject.GetComponent<PlayerStats>();
            Attack(targetPlayer);
        }
    }

    protected void ReturnEnemy()
    {
        Spawner sp = FindAnyObjectByType<Spawner>();
        if (sp != null && sp.relativeSpawnPoints.Count > 0)
        {
            transform.position = player.position + sp.relativeSpawnPoints[Random.Range(0, sp.relativeSpawnPoints.Count)].position;
        }
    }
}
