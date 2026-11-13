using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    public EnemyScriptableOjbects enemyData;
    EnemyPool pool;
    public float currentMoveSpeed;
    public float currentHealth;
    public float currentDamage;

    public float despawnDistance = 40f;
    Transform player;
    void Awake()
    {
        pool = FindAnyObjectByType<EnemyPool>();
        currentDamage = enemyData.Damage;
        currentHealth = enemyData.MaxHealth;
        currentMoveSpeed = enemyData.MoveSpeed;
    }

     void Start()
    {
        player = FindAnyObjectByType<PlayerStats>().transform;
    }

    private void Update()
    {
        if(Vector2.Distance(transform.position,player.position ) >= despawnDistance)
        {
            ReturnEnemy();
        }
    }   
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Kill();
        }
    } 

    void Kill()
    {
        Spawner sp = FindAnyObjectByType<Spawner>();
        sp.OnEnemyKill();
        Destroy(gameObject);
    }

    private void OnCollisionStay2D(Collision2D col)
    {

        if (col.gameObject.CompareTag("Player"))
        {
      
            PlayerStats player = col.gameObject.GetComponent<PlayerStats>();
            player.TakeDamage(currentDamage);
            

        }
    }
    void ReturnEnemy()
    {
        Spawner sp = FindAnyObjectByType<Spawner>();
        transform.position = player.position + sp.relativeSpawnPoints[Random.Range(0, sp.relativeSpawnPoints.Count)].position;
    }

}
