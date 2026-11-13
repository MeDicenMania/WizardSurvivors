using UnityEngine;
using System.Collections.Generic;
using UnityEngine.WSA;
using UnityEngine.Rendering;
public class Experience : Pickups, ICollectible
{
    public int experienceGranted;
    public void Collect()
    {
        PlayerStats player = FindAnyObjectByType<PlayerStats>();
        player.IncreaseExperience (experienceGranted);
       
    }

   

}
