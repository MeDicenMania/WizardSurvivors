using NUnit.Framework;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
public class PlayerStats : MonoBehaviour
{
     WizardsScriptableObject wizardData;
    //public List<GameObject> spawnedSpeells;

    public GameEvent onPlayerHealthChange;

    float currentMaxHealth;

    float currentDamage;
   
    float currentMoveSpeed;
   
    float currentRegen;
    
    float currentProjectileSpeed;
  
    float currentMagnet;





    public float CurrentDamage
    {
        get { return currentDamage; }
        set
        {
            if( currentDamage != value)
            {
                currentDamage = value;

                Debug.Log("CAMBIO EL DAÑO");

                if(GameManager.instance != null)
                {
                    GameManager.instance.currentDamageUI.text = "Damage: " + currentDamage;
                }

            }
        }
    }
    public float CurrentMoveSpeed
    {
        get { return currentMoveSpeed; }
        set
        {
            if (currentMoveSpeed != value)
            {
                currentMoveSpeed = value;

                if (GameManager.instance != null)
                {
                    GameManager.instance.currentMoveSpeedUI.text = "MoveSpeed: " + currentMoveSpeed;
                }


            }
        }
    }
    public float CurrentRegen
    {
        get { return currentRegen; }
        set
        {
            if (currentRegen != value)
            {
                currentRegen = value;
                if (GameManager.instance != null)
                {
                    GameManager.instance.currentRegenUI.text = "Life Regen: " + currentRegen;
                }
            }
        }
    }
    public float CurrentProjectileSpeed
    {
        get { return currentProjectileSpeed; }
        set
        {
            if (currentProjectileSpeed != value)
            {
                currentProjectileSpeed = value;
                if (GameManager.instance != null)
                {
                    GameManager.instance.currentProjectileSpeedUI.text = "Projectile Speed: " + currentProjectileSpeed;
                }
            }
        }
    }
    public float CurrentMagnet
    {
        get { return currentMagnet; }
        set
        {
            if (currentMagnet != value)
            {
                currentMagnet = value;
                if (GameManager.instance != null)
                {
                    GameManager.instance.currentMagnetUI.text = "Magnet: " + currentMagnet;
                }
            }
        }
    }

    [Header("Inventario")]
    InventoryManager inventory;
    public int SpellIndex;
    public int ItemIndex;
    
    
    //public GameObject secondweapontest;

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


    public GameObject item1;
    public GameObject item2;
    void Awake()
    {
        
        wizardData = CharacterSelector.GetData();
        CharacterSelector.instance.DestroySingleton();


        inventory = GetComponent<InventoryManager>();

        currentMaxHealth = wizardData.MaxHealth;
        CurrentDamage = wizardData.Damage;
        CurrentMoveSpeed = wizardData.MoveSpeed;
        CurrentRegen = wizardData.Regen;
        CurrentProjectileSpeed = wizardData.ProjectileSpeed;
        CurrentMagnet = wizardData.Magnet;

        SpawnSpell(wizardData.StartingSpell);
        SpawnItem(item1);
        SpawnItem(item2);
    }

     void Start()
    {

        experienceCap = levelRanges[0].experienceCapIncrease;

        GameManager.instance.currentDamageUI.text = "Damage: " + currentDamage;
        GameManager.instance.currentMoveSpeedUI.text = "MoveSpeed: " + currentMoveSpeed;
        GameManager.instance.currentRegenUI.text = "Life Regen: " + currentRegen;
        GameManager.instance.currentProjectileSpeedUI.text = "Projectile Speed: " + currentProjectileSpeed;
        GameManager.instance.currentMagnetUI.text = "Magnet: " + currentMagnet;

        GameManager.instance.AssingWizardUI(wizardData);
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
        LevelUpCheker();
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
        if (!GameManager.instance.isGameOver)
        {
            GameManager.instance.AssingLevelReachedUI(level);
            GameManager.instance.GameOver();
        }
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
            currentMaxHealth += CurrentRegen * Time.deltaTime;


            if(currentMaxHealth > wizardData.MaxHealth)
            {
                currentMaxHealth = wizardData.MaxHealth;
            }
        }
    }

    public void SpawnSpell(GameObject spell)
    {
        if(SpellIndex >= inventory.spellSlots.Count -1)
        {
            Debug.LogError("spelllimitreached");
            return;

        }


        GameObject spawnSpell = Instantiate(spell, transform.position, Quaternion.identity);
         spawnSpell.transform.SetParent(transform);
         inventory.AddSpell(SpellIndex, spawnSpell.GetComponent<SpellController>());

         SpellIndex++;
         }
    public void SpawnItem(GameObject passiveItem)
    {
        if (ItemIndex >= inventory.passiveItemSlots.Count - 1)
        {
            Debug.LogError("spelllimitreached");
            return;

        }
        GameObject SpawnedPassiveItem = Instantiate(passiveItem, transform.position, Quaternion.identity);
        SpawnedPassiveItem.transform.SetParent(transform);
        inventory.AddPassiveItem(ItemIndex, SpawnedPassiveItem.GetComponent<PassiveItem>());

        ItemIndex++;
    }
}
