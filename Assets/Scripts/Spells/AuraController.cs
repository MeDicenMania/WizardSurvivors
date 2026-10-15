using UnityEngine;

public class AuraController : SpellSystem
{
   
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    protected override void Cast()
    {
        base.Cast();
        GameObject spawnedAura = Instantiate(spellData.Prefab);
        spawnedAura.transform.position = transform.position;
        spawnedAura.transform.parent = transform;
    }
}
