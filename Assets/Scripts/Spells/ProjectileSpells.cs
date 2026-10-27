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

    // Update is called once per frame
    public void DirectionCheck(Vector3 dir)
    {
        direction = dir;

        float dirx = direction.x;
        float diry = direction.y;
     
        Vector3 scale =transform.localScale;

        Vector3 rotation = transform.rotation.eulerAngles;

        if(dirx <0 && diry == 0)     //izquierda soy down
        {
            scale.x = scale.x * -1;
            scale.y = scale.y * -1;
        }

        else if (dirx == 0 && diry < 0)  //Abajo soy down
        {

           
            rotation.z = -90f;
        }
        else if(dirx == 0 && diry > 0)  //Arriba soy down
        {
            scale.y = scale.y * -1;
           
            rotation.z = 90f;
        }
        else if (dirx > 0 && diry > 0) //derecha arriba
        {
            rotation.z = 35f;
           
        }
        else if (dirx > 0 && diry < 0) //derecha abajo
        {
            
            rotation.z = -35f;
        }
        else if (dirx < 0 && diry > 0) //izquierda arriba
        {
          
            rotation.z = 135f;
        }
        else if (dirx < 0 && diry < 0) //izquiera abajo que poronga ios
        {
           
            rotation.z = -135f;
        }

        transform.localScale = scale;  
        transform.rotation = Quaternion.Euler(rotation);
    }

    protected virtual void OnTriggerEnter2D(Collider2D col)
    {
        if(col.CompareTag("Enemy"))
        {
            EnemyStats enemy = col.GetComponent<EnemyStats>();
            enemy.TakeDamage(currentDamage);
            Pierce();
        } 
        else if (col.CompareTag("Prop"))
        {

            if(col.gameObject.TryGetComponent(out BreakableProps breakable))
            {
                breakable.TakeDamage(currentDamage);
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
