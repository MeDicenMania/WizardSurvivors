using NUnit.Framework;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class PlayerStats : MonoBehaviour
{
    WizardsScriptableObject wizardData;
    public List<GameObject> spawnedSpeells = new List<GameObject>();

    [Header("Facade")]
    [SerializeField] private GameFlowFacade gameFlowFacade;

    public GameEvent onPlayerHealthChange;

    public float currentMaxHealth;
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
            if (currentDamage != value)
            {
                currentDamage = value;

                Debug.Log("CAMBIO EL DAÑO");

                if (GameManager.instance != null)
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

    [Header("Experience")]
    public int experience = 0;
    public int level = 1;
    public int experienceCap;
    [SerializeField] private ExperienceCurveAVL experienceCurve;

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
        if (gameFlowFacade == null)
        {
            gameFlowFacade = FindAnyObjectByType<GameFlowFacade>();
        }

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
       // SpawnItem(item2);
    }

    void Start()
    {
        experienceCap = GetExperienceRequired(level);

        if (GameManager.instance != null)
        {
            GameManager.instance.currentDamageUI.text = "Damage: " + currentDamage;
            GameManager.instance.currentMoveSpeedUI.text = "MoveSpeed: " + currentMoveSpeed;
            GameManager.instance.currentRegenUI.text = "Life Regen: " + currentRegen;
            GameManager.instance.currentProjectileSpeedUI.text = "Projectile Speed: " + currentProjectileSpeed;
            GameManager.instance.currentMagnetUI.text = "Magnet: " + currentMagnet;

            GameManager.instance.AssingWizardUI(wizardData);
        }
    }

    void Update()
    {
        if (invicibilityTimer > 0)
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
        while (experience >= experienceCap && experienceCap > 0)
        {
            level++;
            experience -= experienceCap;
            experienceCap = GetExperienceRequired(level);

            GameManager.instance.StartLevelUp();

            if (gameFlowFacade != null)
            {
                gameFlowFacade.NotifyLevelUp(level);
            }
        }
    }

    private int GetExperienceRequired(int targetLevel)
    {
        int fallback = levelRanges != null && levelRanges.Count > 0 ? Mathf.Max(1, levelRanges[0].experienceCapIncrease) : 1;
        // Preserve the prior range-based curve when no AVL asset is assigned.
        if (levelRanges != null)
            for (int current = 2; current <= targetLevel; current++)
                foreach (LevelRange range in levelRanges)
                    if (range != null && current >= range.startLevel && current <= range.endLevel)
                    { fallback = Mathf.Max(1, fallback + range.experienceCapIncrease); break; }
        return experienceCurve != null ? experienceCurve.GetRequiredExperience(targetLevel, fallback) : fallback;
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
        if (GameManager.instance != null && !GameManager.instance.isGameOver)
        {
            GameManager.instance.AssingLevelReachedUI(level);
            GameManager.instance.GameOver();
        }

        if (gameFlowFacade != null)
        {
            gameFlowFacade.TriggerGameOver();
        }
    }

    public void RestoreHealth(float amount)
    {
        if (currentMaxHealth < wizardData.MaxHealth)
        {
            currentMaxHealth += amount;

            if (currentMaxHealth > wizardData.MaxHealth)
            {
                currentMaxHealth = wizardData.MaxHealth;
            }
        }
    }

    void Recover()
    {
        if (currentMaxHealth < wizardData.MaxHealth)
        {
            currentMaxHealth += CurrentRegen * Time.deltaTime;

            if (currentMaxHealth > wizardData.MaxHealth)
            {
                currentMaxHealth = wizardData.MaxHealth;
            }
        }
    }

    public void SpawnSpell(GameObject spell)
    {
        if (inventory != null && SpellIndex >= inventory.spellSlots.Count)
        {
            Debug.LogError("spelllimitreached");
            return;
        }

        GameObject spawnSpell = Instantiate(spell, transform.position, Quaternion.identity);
        spawnSpell.transform.SetParent(transform);
        spawnedSpeells.Add(spawnSpell);

        if (inventory != null)
        {
            inventory.AddSpell(SpellIndex, spawnSpell.GetComponent<SpellSystem>());
        }

        SpellIndex++;
    }

    public void SpawnItem(GameObject passiveItem)
    {
        if (passiveItem == null) return;

        if (inventory != null && ItemIndex >= inventory.passiveItemSlots.Count)
        {
            Debug.LogError("spelllimitreached");
            return;
        }

        GameObject SpawnedPassiveItem = Instantiate(passiveItem, transform.position, Quaternion.identity);
        SpawnedPassiveItem.transform.SetParent(transform);

        if (inventory != null)
        {
            inventory.AddPassiveItem(ItemIndex, SpawnedPassiveItem.GetComponent<PassiveItem>());
        }

        ItemIndex++;
    }


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
