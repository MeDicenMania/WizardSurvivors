using System.Collections.Generic;
using UnityEngine;

// Casts the projectile toward the nearest active enemy, selected with
// the Selection Sort structure shown in ED 07 - Sorting.

public class NearestEnemySpellController : SpellSystem
{
    protected override void Cast()
    {
        base.Cast();

        if (pc == null || spellData == null || spellData.Prefab == null)
            return;

        Vector2 aimDirection = GetNearestEnemyDirection();
        if (aimDirection.sqrMagnitude <= Mathf.Epsilon)
            return;

        GameObject projectile = Instantiate(spellData.Prefab, transform.position, Quaternion.identity);
        IceKnife iceKnife = projectile.GetComponent<IceKnife>();
        if (iceKnife != null)
            iceKnife.DirectionCheck(aimDirection);
    }

    private Vector2 GetNearestEnemyDirection()
    {
        List<EnemyStats> enemies = new List<EnemyStats>(FindObjectsByType<EnemyStats>(FindObjectsSortMode.None));

        // Selection Sort: put the enemy with the smallest squared distance at each index.
        for (int i = 0; i < enemies.Count - 1; i++)
        {
            int nearestIndex = i;
            float nearestDistance = GetDistanceSquared(enemies[i]);

            for (int j = i + 1; j < enemies.Count; j++)
            {
                float candidateDistance = GetDistanceSquared(enemies[j]);
                if (candidateDistance < nearestDistance)
                {
                    nearestIndex = j;
                    nearestDistance = candidateDistance;
                }
            }

            Swap(enemies, i, nearestIndex);
        }

        // Skip dead or inactive entries in case one remains in the scene briefly.
        for (int i = 0; i < enemies.Count; i++)
        {
            EnemyStats enemy = enemies[i];
            if (enemy == null || !enemy.gameObject.activeInHierarchy || enemy.currentHealth <= 0f)
                continue;

            return ((Vector2)enemy.transform.position - (Vector2)pc.transform.position).normalized;
        }

        return Vector2.zero;
    }

    private float GetDistanceSquared(EnemyStats enemy)
    {
        if (enemy == null)
            return float.PositiveInfinity;

        return ((Vector2)enemy.transform.position - (Vector2)pc.transform.position).sqrMagnitude;
    }

    private void Swap(List<EnemyStats> enemies, int a, int b)
    {
        if (a == b)
            return;

        EnemyStats temp = enemies[a];
        enemies[a] = enemies[b];
        enemies[b] = temp;
    }
}
