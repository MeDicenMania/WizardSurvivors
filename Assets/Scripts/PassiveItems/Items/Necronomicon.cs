using UnityEngine;

public class Necronomicon : PassiveItem
{

    protected override void ApplyModifier()
    {
        player.currentDamage *= 1 + passiveItemData.Multiplier / 100f;
    }
}
