using NUnit.Framework;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class PlayerStats : MonoBehaviour
{
    public WizardsScriptableObject wizardData;

    
    public float currentMaxHealth;
    [HideInInspector]
    public float currentDamage;
    [HideInInspector]
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
        currentMaxHealth = wizardData.MaxHealth;
        currentDamage = wizardData.Damage;
        currentMoveSpeed = wizardData.MoveSpeed;
        currentRegen = wizardData.Regen;
        currentProjectileSpeed = wizardData.ProjectileSpeed;
        currentMagnet = wizardData.Magnet;
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
        Destroy(gameObject);
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




}
