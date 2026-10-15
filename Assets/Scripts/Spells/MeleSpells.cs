using UnityEngine;

public class MeleSpells : MonoBehaviour
{
    public SpellsScriptableObjects SpellData;
    public float destroyDelay;



    protected float currentDamage;
    protected float currentPierce;
    protected float currentSpeed;
    protected float currentCooldown;


    private void Awake()
    {
        currentCooldown = SpellData.Cooldown;
        currentDamage = SpellData.Damage;
        currentPierce = SpellData.Pierce;
        currentSpeed = SpellData.Speed;
    }

    protected virtual void Start()
    {
        Destroy(gameObject, destroyDelay);
    }


    protected virtual void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Enemy"))
        {
            EnemyStats enemy = col.GetComponent<EnemyStats>();
            enemy.TakeDamage(currentDamage);

        }
    }
}
