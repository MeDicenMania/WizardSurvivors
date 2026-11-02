using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    public EnemyScriptableOjbects enemyData;
    EnemyPool pool;
    public float currentMoveSpeed;
    public float currentHealth;
    public float currentDamage;

    void Awake()
    {
        currentDamage = enemyData.Damage;
        currentHealth = enemyData.MaxHealth;
        currentMoveSpeed = enemyData.MoveSpeed;
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
        gameObject.SetActive(false);
    }

    private void OnCollisionStay2D(Collision2D col)
    {

        if (col.gameObject.CompareTag("Player"))
        {
      
            PlayerStats player = col.gameObject.GetComponent<PlayerStats>();
            player.TakeDamage(currentDamage);
            

        }
    }


}
