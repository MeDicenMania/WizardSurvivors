using UnityEngine;

public class TokenCollect : Pickups, ICollectible
{
    public int tokensGiven;
    public void Collect()
    {
        PlayerStats player = FindAnyObjectByType<PlayerStats>();
        player.IncreaseTokens(tokensGiven);
    }
}
