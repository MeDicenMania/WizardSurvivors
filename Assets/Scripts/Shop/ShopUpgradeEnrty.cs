using System;
using UnityEngine;

[Serializable]
public class ShopUpgradeEntry : IComparable<ShopUpgradeEntry>
{
    public int requiredPlayerLevel;
    public GameObject prefab;

    public int UpgradeID
    {
        get
        {
            if (prefab == null)
                return -1;

            PassiveItem item = prefab.GetComponent<PassiveItem>();

            if (item == null || item.passiveItemData == null)
                return -1;

            return item.passiveItemData.UpgradeID;
        }
    }

    public int CompareTo(ShopUpgradeEntry other)
    {
        if (other == null)
            return 1;

        int comparison = requiredPlayerLevel.CompareTo(
            other.requiredPlayerLevel);

        if (comparison != 0)
            return comparison;

        return UpgradeID.CompareTo(other.UpgradeID);
    }
}