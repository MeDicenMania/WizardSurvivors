using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.WSA;

public class SpellSystem : MonoBehaviour
{
    [Header("Spell Stats")]

    public SpellsScriptableObjects spellData;
    
    public float currentCooldown;
   

    protected PlayerController pc;

  protected virtual void Start()
    {
        pc = FindAnyObjectByType<PlayerController>();
        if (pc == null)
        {
            Debug.Log("NO ENCONTRE AL PLAYER");
        }
        else
        {
            Debug.Log("Player encontrado: " + pc.gameObject.name);
        }

        currentCooldown = spellData.Cooldown;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        currentCooldown -= Time.deltaTime;


        if(currentCooldown <= 0f)
        {
            Cast();
        }
    }



    protected virtual void Cast()
    {
        currentCooldown = spellData.Cooldown;
    }
}
