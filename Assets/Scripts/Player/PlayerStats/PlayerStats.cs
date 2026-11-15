using NUnit.Framework;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
public class PlayerStats : MonoBehaviour
{
    WizardsScriptableObject wizardData;
    public List<GameObject> spawnedSpeells;

    [Header("Facade")]
    [SerializeField] private GameFlowFacade gameFlowFacade;

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
        if (gameFlowFacade == null)
        {
            gameFlowFacade = FindAnyObjectByType<GameFlowFacade>();
        }

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
                if(level >= range.startLevel && level <= range.endLevel)
                {
                    experienceCapIncrease = range.experienceCapIncrease;
                        break;
                }
            }

            experienceCap += experienceCapIncrease;

            if (gameFlowFacade != null)
            {
                gameFlowFacade.NotifyLevelUp(level);
            }
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
        if (gameFlowFacade != null)
        {
            gameFlowFacade.TriggerGameOver();
        }
        else
        {
            Debug.Log("Player Murió (GameFlowFacade no encontrado en la escena).");
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

    // ==========================================
    // PATRÓN MEMENTO (Originador / Originator)
    // ==========================================

    // Crea un memento con el estado actual del jugador
    public PlayerMemento SaveState()
    {
        return new PlayerMemento(
            transform.position,
            currentMaxHealth,
            experience,
            level,
            experienceCap
        );
    }

    // Restaura el estado del jugador desde un memento
    public void RestoreState(PlayerMemento memento)
    {
        if (memento == null) return;

        transform.position = memento.Position;
        this.currentMaxHealth = memento.Health;
        this.experience = memento.Experience;
        this.level = memento.Level;
        this.experienceCap = memento.ExperienceCap;

        if (onPlayerHealthChange != null)
        {
            onPlayerHealthChange.Raise(this, currentMaxHealth);
        }
    }

}
