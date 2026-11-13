using UnityEngine;

public class PassiveItemN1 : PassiveItem
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void ApplyModifier()
    {
        player.currentMoveSpeed *= 1 + passiveItemData.Multiplier / 100f;
    }
}
