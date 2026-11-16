using UnityEngine;
using System.Collections.Generic;
public class Experience : Pickups, ICollectible
{
    public int experienceGranted;
    public void Collect()
    {
        PlayerStats player = FindAnyObjectByType<PlayerStats>();
        player.IncreaseExperience (experienceGranted);
       
    }

   

}
