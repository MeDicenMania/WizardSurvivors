using UnityEngine;

public class Necronomicon : PassiveItem
{

    protected override void ApplyModifier()
    {
        player.CurrentDamage *= 1 + passiveItemData.Multiplier / 100f;
        player.RegisterUpgrade(passiveItemData.Name, passiveItemData.Description);
    }
}
