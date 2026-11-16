using Unity.VisualScripting;
using UnityEngine;

public class ProjectileSpells : MonoBehaviour
{

    protected Vector3 direction;
    public float destroyDelay;
    public SpellsScriptableObjects spellData;

    protected float currentDamage;
    protected float currentSpeed;
    protected float currentCooldown;
    protected float currentPierce;


     void Awake()
    {
        currentCooldown = spellData.Cooldown;
        currentSpeed = spellData.Speed;
        currentDamage = spellData.Damage;
    }

    protected virtual void Start()
    {
        Destroy(gameObject, destroyDelay);
    }

    
    public float getCurrentDamage()
    {
        return currentDamage *= FindAnyObjectByType<PlayerStats>().CurrentDamage;
    }
    // Apunta el proyectil en 360 grados
    public void DirectionCheck(Vector3 dir)
    {
        direction = dir.sqrMagnitude > 0f ? dir.normalized : Vector3.right;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    protected virtual void OnTriggerEnter2D(Collider2D col)
    {
        if(col.CompareTag("Enemy"))
        {
            EnemyStats enemy = col.GetComponent<EnemyStats>();
            enemy.TakeDamage(getCurrentDamage());
            Pierce();
        } 
        else if (col.CompareTag("Prop"))
        {

            if(col.gameObject.TryGetComponent(out BreakableProps breakable))
            {
                breakable.TakeDamage(getCurrentDamage());
                Pierce();
            }

        }
    }
    void Pierce()
    {
        currentPierce--;
        if( currentPierce < 0)
        {
            Destroy(gameObject);
        }
    }


}
