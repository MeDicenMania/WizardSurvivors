using UnityEngine;

public class PassiveItemN1 : PassiveItem
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void ApplyModifier()
    {
        player.CurrentMoveSpeed *= 1 + passiveItemData.Multiplier / 100f;
        player.RegisterUpgrade(passiveItemData.Name, passiveItemData.Description);
    }
}
