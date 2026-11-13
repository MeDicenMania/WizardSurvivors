using NUnit.Framework;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
public class PlayerStats : MonoBehaviour
{
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
        experience += amount;
    }

    void LevelUpCheker()
    {
        if(experience >= experienceCap)
        {
            level++;
            experience -= experienceCap;

            int experienceCapIncrease = 0;

            foreach(LevelRange range in levelRanges)
            {
                if(level >= range.startLevel && level >= range.endLevel)
                {
                    experienceCapIncrease = range.experienceCapIncrease;
                        break;
                }
            }

            experienceCap += experienceCapIncrease;

        }
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
        GameObject spawnSpell = Instantiate(spell, transform.position, Quaternion.identity);
        spawnSpell.transform.SetParent(transform);
        spawnedSpeells.Add(spawnSpell);
    }


}
