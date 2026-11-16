using System.Collections.Generic;
using UnityEngine;
using System.Collections;

using UnityEngine.UI;
using TMPro;

public class InventoryManager : MonoBehaviour
{
    public List<SpellSystem> spellSlots = new List<SpellSystem>(4);
    public List<PassiveItem> passiveItemSlots = new List<PassiveItem>(6);

    public List<Image> spellIconsUI = new List<Image>(4);
    public List<Image> passiveitemconsUI = new List<Image>(6);

    public int[] spellLevels = new int [6];
    public int[] passiveItemsLevels = new int [6];

    [System.Serializable]
    public class SpellUpgrade
    {
        public int spellUpgradeIndex;

        public GameObject initialWeapon;
        public SpellsScriptableObjects spellsData;
    }
    [System.Serializable]
    public class PasiveItemUpgrade
    {
        public int passiveItemIndex;

        public GameObject initialItem;
        public PassiveItemScriptableObject passiveItemsData;

    }
    [System.Serializable]
    public class UpgradeUI
    {
        public TextMeshProUGUI upgradeTextUI;
        public TextMeshProUGUI upgradeDescriptionUI;
        public Image upgradeIcon;
        public Button upgradeButton;

    }
    [Header("Level Up Options")]
    public List<SpellUpgrade> spellUpgradeOptions = new List<SpellUpgrade>();
    public List<PasiveItemUpgrade> passiveItemsUpgradeOptions = new List<PasiveItemUpgrade>();
    public List<UpgradeUI> upgradeOptions = new List<UpgradeUI>();
    [Header("Spell progression data")]
    [SerializeField] private SpellSkillTree spellSkillTree;
    [SerializeField] private SpellPurchaseGraph spellPurchaseGraph;

    PlayerStats stats;

    //Upgrade history 
    [Header("Upgrade history")]
    [SerializeField] private TextMeshProUGUI upgradeNameText;
    [SerializeField] private TextMeshProUGUI upgradeDescriptionText;


    private void Awake()
    {
        stats = GetComponent<PlayerStats>();
        EnsureSlots(spellSlots, 4);
        EnsureSlots(passiveItemSlots, 6);
        EnsureSlots(spellIconsUI, 4);
        EnsureSlots(passiveitemconsUI, 6);
    }

    private static void EnsureSlots<T>(List<T> slots, int count) where T : class
    { while (slots.Count < count) slots.Add(null); }

    /// <summary>Validates the skill tree and cheapest graph route, then buys the spell for XP.</summary>
    public bool TryPurchaseSpell(SpellsScriptableObjects target)
    {
        if (target == null || spellSkillTree == null || spellPurchaseGraph == null || stats == null) return false;
        HashSet<SpellsScriptableObjects> owned = new HashSet<SpellsScriptableObjects>();
        foreach (SpellSystem spell in spellSlots)
            if (spell != null && spell.spellData != null) owned.Add(spell.spellData);
        if (owned.Contains(target) || !spellSkillTree.Contains(target)) return false;
        if (!spellSkillTree.GetUnlockableSpells(owned).Contains(target)) return false;

        SpellPurchaseGraph.PurchasePath cheapest = null;
        foreach (SpellsScriptableObjects source in owned)
            if (spellPurchaseGraph.TryGetCheapestPath(source, target, out SpellPurchaseGraph.PurchasePath candidate) &&
                (cheapest == null || candidate.totalCost < cheapest.totalCost)) cheapest = candidate;
        if (cheapest == null || stats.experience < cheapest.totalCost || stats.SpellIndex >= spellSlots.Count) return false;

        GameObject prefab = FindSpellPrefab(target);
        if (prefab == null) return false;
        stats.experience -= cheapest.totalCost;
        stats.SpawnSpell(prefab);
        return true;
    }

    private GameObject FindSpellPrefab(SpellsScriptableObjects data)
    {
        foreach (SpellUpgrade option in spellUpgradeOptions)
            if (option != null && option.spellsData == data) return option.initialWeapon;
        return null;
    }
    public void AddSpell(int slot, SpellSystem newSpell)     //añade un nuevo hechizo a tu inventario/arsenal qsy
    {
        EnsureSlots(spellSlots, 4);
        EnsureSlots(spellIconsUI, 4);
        if (slot < 0 || slot >= spellSlots.Count) return;
        spellSlots[slot] = newSpell;
        spellLevels[slot] = newSpell.spellData.Level;
        spellIconsUI[slot].enabled = true;
        spellIconsUI[slot].sprite = newSpell.spellData.Icon;

        if (GameManager.instance != null && GameManager.instance.ischoosingUpgrades)
        {
            GameManager.instance.EndLevelUp();
        }
    }
    
    public void AddPassiveItem(int slot,PassiveItem newItem)
    {
        EnsureSlots(passiveItemSlots, 6);
        EnsureSlots(passiveitemconsUI, 6);
        if (slot < 0 || slot >= passiveItemSlots.Count) return;
        passiveItemSlots[slot] = newItem;
        passiveItemsLevels[slot] = newItem.passiveItemData.Level;
        passiveitemconsUI[slot].enabled = true;
        passiveitemconsUI[slot].sprite = newItem.passiveItemData.Icon;

        if (GameManager.instance != null && GameManager.instance.ischoosingUpgrades)
        {
            GameManager.instance.EndLevelUp();
        }
    }

    public void levelUpSpell(int slot, int upgradeIndex)
    {

        if( spellSlots.Count > slot)
        {
            SpellSystem spell = spellSlots[slot];

            if (!spell.spellData.NextLevelPrefab)
            {
                Debug.LogError("nomorelevelslilbro");
                return;
            }

            GameObject upgradedSpell = Instantiate(spell.spellData.NextLevelPrefab,transform.position,Quaternion.identity);
            upgradedSpell.transform.SetParent(transform);
            AddSpell(slot, upgradedSpell.GetComponent<SpellSystem>());
            Destroy(spell.gameObject);
            spellLevels[slot] = upgradedSpell.GetComponent<SpellSystem>().spellData.Level;

            spellUpgradeOptions[upgradeIndex].spellsData = upgradedSpell.GetComponent<SpellSystem>().spellData;


            if (GameManager.instance != null && GameManager.instance.ischoosingUpgrades)
            {
                GameManager.instance.EndLevelUp();
            }
        }
    }
    public void levelUpItem(int slot, int upgradeIndex)
    {
        if (passiveItemSlots.Count > slot)
        {
            PassiveItem passiveItem = passiveItemSlots[slot];
            if (!passiveItem.passiveItemData.NextLevelPrefab)
            {
                Debug.LogError("nomorelevelslilbro");
                return;
            }

            GameObject upgradePassiveItem = Instantiate(passiveItem.passiveItemData.NextLevelPrefab, transform.position, Quaternion.identity);
            upgradePassiveItem.transform.SetParent(transform);
            AddPassiveItem(slot, upgradePassiveItem.GetComponent<PassiveItem>());
            Destroy(passiveItem.gameObject);
            passiveItemsLevels[slot] = upgradePassiveItem.GetComponent<PassiveItem>().passiveItemData.Level;

            passiveItemsUpgradeOptions[upgradeIndex].passiveItemsData = upgradePassiveItem.GetComponent<PassiveItem>().passiveItemData;


            if (GameManager.instance != null && GameManager.instance.ischoosingUpgrades)
            {
                GameManager.instance.EndLevelUp();
            }
        }
    }
    void ApllyUpgradeOptions()
    {

        List<SpellUpgrade> avaliableSpellUpgrades = new List<SpellUpgrade>(spellUpgradeOptions);
        List <PasiveItemUpgrade> avaliablePassiveItemsUpgrades = new List<PasiveItemUpgrade>(passiveItemsUpgradeOptions);


        foreach (var upgradeOption in upgradeOptions)
        {
            if(avaliableSpellUpgrades.Count == 0 && avaliablePassiveItemsUpgrades.Count == 0)
            {
                return;
            }

            int upgradeType;

            if(avaliableSpellUpgrades.Count == 0)
            {
                upgradeType = 2;
            }
            else if( avaliablePassiveItemsUpgrades.Count == 0)
            {
                upgradeType = 1;
            }
            else
            {
                upgradeType = Random.Range(1, 3);
            }
               

            if( upgradeType == 1)            //si la mejora es un arma 
            {
                SpellUpgrade choosenSpellUpgrade = avaliableSpellUpgrades[Random.Range(0, avaliableSpellUpgrades.Count)];

                avaliableSpellUpgrades.Remove(choosenSpellUpgrade);

                if(choosenSpellUpgrade != null)
                {
                    bool newSpell = false;

                    for(int i = 0; i < spellSlots.Count; i++)
                    {
                        if (spellSlots[i] != null && spellSlots[i].spellData == choosenSpellUpgrade.spellsData)
                        {
                            newSpell = false;
                            
                            if(!newSpell)
                            {
                                if (!choosenSpellUpgrade.spellsData.NextLevelPrefab)
                                {
                                    break;
                                }

                                upgradeOption.upgradeButton.onClick.AddListener(() => levelUpSpell(i,choosenSpellUpgrade.spellUpgradeIndex));


                                //ponemos descripcion y nombre
                                upgradeOption.upgradeDescriptionUI.text = choosenSpellUpgrade.spellsData.NextLevelPrefab.GetComponent<SpellController>().spellData.Description;
                                upgradeOption.upgradeTextUI.text = choosenSpellUpgrade.spellsData.NextLevelPrefab.GetComponent<SpellController>().spellData.Name;
                            }
                            break;

                        }
                        else
                        {
                            newSpell = true;
                        }
                    }
                    if (newSpell)
                    {
                        upgradeOption.upgradeButton.onClick.AddListener(() => stats.SpawnSpell(choosenSpellUpgrade.initialWeapon));
                        //aplicar descripcion y nombre iniciales
                        upgradeOption.upgradeDescriptionUI.text = choosenSpellUpgrade.spellsData.Description;
                        upgradeOption.upgradeTextUI.text = choosenSpellUpgrade.spellsData.Name;
                    }
                    upgradeOption.upgradeIcon.sprite = choosenSpellUpgrade.spellsData.Icon;
                }

            }
            else if (upgradeType == 2)
            {
                PasiveItemUpgrade choosePasiveItemUpgrade = avaliablePassiveItemsUpgrades[Random.Range(0, avaliablePassiveItemsUpgrades.Count)];

                avaliablePassiveItemsUpgrades.Remove(choosePasiveItemUpgrade);

                if(choosePasiveItemUpgrade != null)
                {
                    bool newPassiveItem = false;
                    for (int i = 0; i < passiveItemSlots.Count; i++)
                    {
                        if (passiveItemSlots[i] != null && passiveItemSlots[i].passiveItemData == choosePasiveItemUpgrade.passiveItemsData) 
                        {
                            newPassiveItem = false;
                            

                            if (!newPassiveItem)
                            {
                                if (!choosePasiveItemUpgrade.passiveItemsData.NextLevelPrefab)
                                {
                                    break;
                                }

                                upgradeOption.upgradeButton.onClick.AddListener(() => levelUpItem(i,choosePasiveItemUpgrade.passiveItemIndex));
                                upgradeOption.upgradeDescriptionUI.text = choosePasiveItemUpgrade.passiveItemsData.NextLevelPrefab.GetComponent<PassiveItem>().passiveItemData.Description;
                                upgradeOption.upgradeTextUI.text = choosePasiveItemUpgrade.passiveItemsData.NextLevelPrefab.GetComponent<PassiveItem>().passiveItemData.Name;
                            }
                            break;
                        }
                        else 
                        {

                            newPassiveItem = true;
                        }
                    }
                    if(newPassiveItem == true)
                    {
                        upgradeOption.upgradeButton.onClick.AddListener(() => stats.SpawnItem(choosePasiveItemUpgrade.initialItem));

                        upgradeOption.upgradeDescriptionUI.text = choosePasiveItemUpgrade.passiveItemsData.Description;
                        upgradeOption.upgradeTextUI.text = choosePasiveItemUpgrade.passiveItemsData.Name;

                    }
                    upgradeOption.upgradeIcon.sprite = choosePasiveItemUpgrade.passiveItemsData.Icon;
                }

            }
        }
    }


    void RemoveUpgradeOptions()
    {
        foreach(var upgradeOption in upgradeOptions)
        {
            upgradeOption.upgradeButton.onClick.RemoveAllListeners ();
        }
    }

    public void RemoveandApllyUI()
    {
        RemoveUpgradeOptions();
        ApllyUpgradeOptions();
    }

  
    public void ShowLastUpgrade()
    {
        if (stats == null || stats.UpgradeCount == 0)
        {
            upgradeNameText.text = "Sin mejoras";
            upgradeDescriptionText.text = "";
            return;
        }

        PlayerUpgradesList lastUpgrade =
            stats.GetUpgrade(stats.UpgradeCount - 1);

        upgradeNameText.text = lastUpgrade.upgradeName;
        upgradeDescriptionText.text = lastUpgrade.description;
    }


}

