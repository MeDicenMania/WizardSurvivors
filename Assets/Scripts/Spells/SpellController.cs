using UnityEngine;

public class SpellController : SpellSystem
{
    [SerializeField] bool shootClosestFirst = true;

    protected override void Start()
    {
        base.Start();
    }

    protected override void Cast()
    {
        base.Cast();

        Vector3 shootDir = GetTargetDirection();
        GameObject iceSpawned = Instantiate(spellData.Prefab);
        iceSpawned.transform.position = transform.position;
        iceSpawned.GetComponent<ProjectileSpells>().DirectionCheck(shootDir);
    }

    // Trackea y ordena enemigos por distancia
    Vector3 GetTargetDirection()
    {
        EnemyStats[] enemies = FindObjectsByType<EnemyStats>(FindObjectsSortMode.None);

        if (enemies.Length == 0)
        {
            return pc != null ? (Vector3)pc.lastMovedVector : Vector3.right;
        }

        Vector3 origin = transform.position;
        CollectionSort.SortArray(enemies, (a, b) =>
        {
            float distA = (a.transform.position - origin).sqrMagnitude;
            float distB = (b.transform.position - origin).sqrMagnitude;
            return distA.CompareTo(distB);
        }, shootClosestFirst);

        Vector3 dir = enemies[0].transform.position - origin;
        if (dir.sqrMagnitude > 0f)
        {
            return dir.normalized;
        }

        return pc != null ? (Vector3)pc.lastMovedVector : Vector3.right;
    }
}
