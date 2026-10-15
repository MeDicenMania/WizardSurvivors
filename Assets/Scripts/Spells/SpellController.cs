using UnityEngine;

public class SpellController : SpellSystem
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
   protected override void Start()
    {
        base.Start();  
    }

    // Update is called once per frame
    protected override void Cast()
    {
        base.Cast();
        GameObject iceSpawned = Instantiate(spellData.Prefab);
        iceSpawned.transform.position = transform.position;
        iceSpawned.GetComponent<IceKnife>().DirectionCheck(pc.lastMovedVector);
       
    }
}
