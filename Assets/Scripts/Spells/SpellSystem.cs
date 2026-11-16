using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.WSA;

public class SpellSystem : MonoBehaviour
{
    [Header("Spell Stats")]

    public SpellsScriptableObjects spellData;
    
    public float currentCooldown;
    protected SpellSkillTree skillTree;
    protected float bonusDamage, bonusSpeed, bonusCooldown, bonusPierce;

    protected PlayerController pc;

  protected virtual void Start()
    {
        pc = FindAnyObjectByType<PlayerController>();
        skillTree = GetComponent<SpellSkillTree>();
        RefreshSkillBonuses();
        currentCooldown = GetEffectiveCooldown();
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
        currentCooldown = GetEffectiveCooldown();
    }

    protected void RefreshSkillBonuses()
    {
        if (skillTree != null)
            skillTree.GetBonuses(out bonusDamage, out bonusSpeed, out bonusCooldown, out bonusPierce);
    }

    public void RefreshBonusesAfterUpgrade()
    {
        RefreshSkillBonuses();
    }

    private float GetEffectiveCooldown()
    {
        return Mathf.Max(0.05f, spellData.Cooldown - bonusCooldown);
    }
}
