using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
public class IceAura : MeleSpells
{
    List <GameObject> markedEnemies;
   protected override void Start()
    {
        base.Start();

        markedEnemies = new List <GameObject>();
    }
    protected override void OnTriggerEnter2D(Collider2D col)               //nerfeo esta cagada
    {
        if(col.CompareTag("Enemy") && !markedEnemies.Contains(col.gameObject))
        { 
            EnemyStats enemy = col.GetComponent<EnemyStats>();
            enemy.TakeDamage(currentDamage);
            markedEnemies.Add(col.gameObject);
        }
        else if (col.CompareTag("Prop"))
        {

            if (col.gameObject.TryGetComponent(out BreakableProps breakable))
            {
                breakable.TakeDamage(currentDamage);

                markedEnemies.Add(col.gameObject);
            }

        }
    }
}
