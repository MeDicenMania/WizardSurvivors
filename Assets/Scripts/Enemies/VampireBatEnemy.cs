using UnityEngine;

public class VampireBatEnemy : EnemyStats
{
    [Header("Vampire Bat Specials")]
    [SerializeField] private float lifeStealAmount = 1f;

    public override void Attack(PlayerStats targetPlayer)
    {
        base.Attack(targetPlayer);

        // Comportamiento único de esta subclase: roba vida al atacar al jugador
        if (enemyData != null && currentHealth < enemyData.MaxHealth)
        {
            currentHealth = Mathf.Min(currentHealth + lifeStealAmount, enemyData.MaxHealth);
        }
    }
}
