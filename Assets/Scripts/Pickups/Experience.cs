using UnityEngine;
using System.Collections.Generic;
using UnityEngine.WSA;
using UnityEngine.Rendering;
public class Experience : MonoBehaviour,ICollectible
{
    public int experienceGranted;
    public void Collect()
    {
        PlayerStats player = FindAnyObjectByType<PlayerStats>();
        player.IncreaseExperience (experienceGranted);
        Destroy(gameObject);
    }

    
}
