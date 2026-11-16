using NUnit.Framework;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.Events;
public class PlayerStats : MonoBehaviour
{
    public class SpellUpgradeOption
    {
        public GameObject spellInstance;
        public SpellSkillTree.SkillNode upgrade;
    }
     WizardsScriptableObject wizardData;
    public List<GameObject> spawnedSpeells;

    public GameEvent onPlayerHealthChange;

    public float currentMaxHealth;
    [HideInInspector]
    public float currentDamage;
   
    public float currentMoveSpeed;
    [HideInInspector]
    public float currentRegen;
    [HideInInspector]
    public float currentProjectileSpeed;
    [HideInInspector]
    public float currentMagnet;

    [Header("Experience")]
    public int experience = 0;
    public int level = 1;
    public int experienceCap;
    [SerializeField] private ExperienceRequirementAVL experienceRequirements;
    [SerializeField] private int spellCurrency;
    [SerializeField] private UnityEvent<int> onPlayerLevelUp = new UnityEvent<int>();
    public int SpellCurrency => spellCurrency;
    public UnityEvent<int> OnPlayerLevelUp => onPlayerLevelUp;
    private readonly HashSet<string> ownedSpellIds = new HashSet<string>();


    [System.Serializable] 
    public class LevelRange
    {
        public int startLevel;
        public int endLevel;
        public int experienceCapIncrease;
    }

    public List<LevelRange> levelRanges;


    [Header("IFRAMES")]

    public float incibilityDuration;
    float invicibilityTimer;
    bool isInvicible;

    void Awake()
    {

        wizardData = CharacterSelector.GetData();
        CharacterSelector.instance.DestroySingleton();

        currentMaxHealth = wizardData.MaxHealth;
        currentDamage = wizardData.Damage;
        currentMoveSpeed = wizardData.MoveSpeed;
        currentRegen = wizardData.Regen;
        currentProjectileSpeed = wizardData.ProjectileSpeed;
        currentMagnet = wizardData.Magnet;

        SpawnSpell(wizardData.StartingSpell);
    }

     void Start()
    {
        if (experienceRequirements != null && experienceRequirements.TryGetRequirement(level + 1, out int firstRequirement))
            experienceCap = firstRequirement;
        else if (levelRanges != null && levelRanges.Count > 0)
            experienceCap = levelRanges[0].experienceCapIncrease;
    }
    
     void Update()
    {
        if (invicibilityTimer >0)
        {
            invicibilityTimer -= Time.deltaTime;
        }
        else if (isInvicible)
        {
            isInvicible = false;
        }
        Recover();
    }
    public void IncreaseExperience(int amount)
    {
        if (amount <= 0) return;
        experience += amount;
        while (experienceCap > 0 && experience >= experienceCap)
        {
            experience -= experienceCap;
            level++;
            UpdateExperienceCap();
            if (onPlayerLevelUp == null) onPlayerLevelUp = new UnityEvent<int>();
            onPlayerLevelUp?.Invoke(level);
        }
    }

    private void UpdateExperienceCap()
    {
        if (experienceRequirements != null && experienceRequirements.TryGetRequirement(level + 1, out int requirement))
        {
            experienceCap = requirement;
            return;
        }

        if (levelRanges == null) { experienceCap = 0; return; }
        for (int i = 0; i < levelRanges.Count; i++)
        {
            LevelRange range = levelRanges[i];
            if (range != null && level >= range.startLevel && level <= range.endLevel)
            {
                experienceCap += range.experienceCapIncrease;
                return;
            }
        }
        experienceCap = 0;
    }

    public bool HasSpellInInventory(string spellId) => !string.IsNullOrWhiteSpace(spellId) && ownedSpellIds.Contains(spellId);
    public void RegisterSpellInInventory(string spellId)
    {
        if (!string.IsNullOrWhiteSpace(spellId)) ownedSpellIds.Add(spellId);
    }
    public bool TrySpendSpellCurrency(int amount)
    {
        if (amount < 0 || spellCurrency < amount) return false;
        spellCurrency -= amount;
        return true;
    }
    public void AddSpellCurrency(int amount)
    {
        if (amount > 0) spellCurrency += amount;
    }

    public List<SpellUpgradeOption> GetAvailableSpellUpgrades()
    {
        List<SpellUpgradeOption> options = new List<SpellUpgradeOption>();
        if (spawnedSpeells == null) return options;
        for (int i = 0; i < spawnedSpeells.Count; i++)
        {
            GameObject spell = spawnedSpeells[i];
            if (spell == null) continue;
            SpellSkillTree tree = spell.GetComponent<SpellSkillTree>();
            if (tree == null) continue;
            List<SpellSkillTree.SkillNode> available = tree.GetAvailableUpgrades(level);
            for (int n = 0; n < available.Count; n++)
                options.Add(new SpellUpgradeOption { spellInstance = spell, upgrade = available[n] });
        }
        return options;
    }

    public bool TryAcquireSpellUpgrade(GameObject spellInstance, string upgradeId)
    {
        if (spellInstance == null || spawnedSpeells == null || !spawnedSpeells.Contains(spellInstance)) return false;
        SpellSkillTree tree = spellInstance.GetComponent<SpellSkillTree>();
        if (tree == null || !tree.TryAcquireUpgrade(upgradeId, level)) return false;

        SpellSystem controller = spellInstance.GetComponent<SpellSystem>();
        if (controller != null) controller.RefreshBonusesAfterUpgrade();
        return true;
    }


    public void TakeDamage(float dmg)
    {
        if (!isInvicible)
        {
            currentMaxHealth -= dmg;
            onPlayerHealthChange.Raise(this, currentMaxHealth);

            invicibilityTimer = incibilityDuration;
            isInvicible = true;

            if (currentMaxHealth <= 0)
            {
                Kill();
            }
        }
    }

    public void Kill()
    {
        
    }
  public void RestoreHealth(float amount)
    {
        if( currentMaxHealth < wizardData.MaxHealth)
        {
            currentMaxHealth += amount;

            if(currentMaxHealth > wizardData.MaxHealth)
            {
                currentMaxHealth = wizardData .MaxHealth;
            }
        }
      
    }

    void Recover()
    {
        if( currentMaxHealth < wizardData.MaxHealth)
        {
            currentMaxHealth += currentRegen * Time.deltaTime;


            if(currentMaxHealth > wizardData.MaxHealth)
            {
                currentMaxHealth = wizardData.MaxHealth;
            }
        }
    }

    public void SpawnSpell(GameObject spell)
    {
        if (spell == null) return;
        GameObject spawnSpell = Instantiate(spell, transform.position, Quaternion.identity);
        spawnSpell.transform.SetParent(transform);
        if (spawnedSpeells == null) spawnedSpeells = new List<GameObject>();
        spawnedSpeells.Add(spawnSpell);
    }


}
