using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using Unity.Mathematics;
using UnityEngine.UI;
public class InventoryManager : MonoBehaviour
{
    public List<SpellSystem> spellSlots = new List<SpellSystem>(4);
    public List<PassiveItem> passiveItemSlots = new List<PassiveItem>(6);

    public List<Image> spellIconsUI = new List<Image>(4);
    public List<Image> passiveitemconsUI = new List<Image>(6);

    public int[] spellLevels = new int [6];
    public int[] passiveItemsLevels = new int [6];


    public void AddSpell(int slot,SpellSystem newSpell)     //añade un nuevo hechizo a tu inventario/arsenal qsy
    {
        spellSlots[slot] = newSpell;
        spellLevels[slot] = newSpell.spellData.Level;
        spellIconsUI[slot].enabled = true;
        spellIconsUI[slot].sprite = newSpell.spellData.Icon;

      
    }

    public void AddPassiveItem(int slot,PassiveItem newItem)
    {
        passiveItemSlots[slot] = newItem;
        passiveItemsLevels[slot] = newItem.passiveItemData.Level;
        passiveitemconsUI[slot].enabled = true;
        passiveitemconsUI[slot].sprite = newItem.passiveItemData.Icon;
    }

    public void levelUpSpell(int slot)
    {

        if( spellSlots.Count > slot)
        {
            SpellSystem spell = spellSlots[slot];

            if (!spell.spellData.NextLevelPrefab)
            {
                Debug.LogError("nomorelevelslilbro");
                return;
            }

            GameObject upgradeSpell = Instantiate(spell.spellData.NextLevelPrefab,transform.position,Quaternion.identity);
            upgradeSpell.transform.SetParent(transform);
            AddSpell(slot, upgradeSpell.GetComponent<SpellSystem>());
            Destroy(spell.gameObject);
            spellLevels[slot] = upgradeSpell.GetComponent<SpellSystem>().spellData.Level;
        }
    }
    public void levelUpItem(int slot)
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
        }
         



    }
}
