using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class ExperienceDrop : MonoBehaviour
{
    [System.Serializable]
    public class Drops
    {
        public string name;
        public GameObject prefab;
        public float dropRate;
    }

    public List<Drops> drops;
    private bool droppedInCurrentLife = false;

    private void OnEnable()
    {
        droppedInCurrentLife = false;
    }

    public void SpawnDrop()
    {
        if (droppedInCurrentLife) return;
        droppedInCurrentLife = true;

        float randomNum = UnityEngine.Random.Range(0f, 100f);
        List<Drops> possibleDrops = new List<Drops>();
        foreach (Drops rate in drops)
        {
            if (randomNum <= rate.dropRate)
            {
                possibleDrops.Add(rate);
            }
        }
        if (possibleDrops.Count > 0)
        {
            Drops selectedDrop = possibleDrops[UnityEngine.Random.Range(0, possibleDrops.Count)];
            if (selectedDrop.prefab != null)
            {
                Instantiate(selectedDrop.prefab, transform.position, Quaternion.identity);
            }
        }
    }

    void OnDestroy()
    {
        if (!gameObject.scene.isLoaded)
        {
            return;
        }

        SpawnDrop();
    }
}
