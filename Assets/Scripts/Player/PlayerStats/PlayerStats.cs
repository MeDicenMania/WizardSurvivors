using NUnit.Framework;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class PlayerStats : MonoBehaviour
{
    public WizardsScriptableObject wizardData;


    float currentMaxHealth;
    float currentDamage;
    float currentMoveSpeed;
    float currentRegen;
    float currentProjectileSpeed;

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

    void awake()
    {
        currentMaxHealth = wizardData.MaxHealth;
        currentDamage = wizardData.Damage;
        currentMoveSpeed = wizardData.MoveSpeed;
        currentRegen = wizardData.Regen;
        currentProjectileSpeed = wizardData.ProjectileSpeed;
    }

     void Start()
    {
        experienceCap = levelRanges[0].experienceCapIncrease;
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

    void Update()
    {
        
    }
}
