using UnityEngine;

public class IceKnife : ProjectileSpells
{
   

  protected  override void Start()
    {
        base.Start();
        
        

    }

    // Update is called once per frame
    void Update()
    {
       
        transform.position += direction * currentSpeed * Time.deltaTime;
    }
}
