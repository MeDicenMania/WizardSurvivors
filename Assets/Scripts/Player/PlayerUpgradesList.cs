using UnityEngine;
[System.Serializable]
public class PlayerUpgradesList 
{

    public string upgradeName;
    public string description;

    public PlayerUpgradesList(string name, string desc)
    {
        upgradeName = name;
        description = desc;
    }
}