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

        float total = 0f;
        foreach (Drops d in drops) total += d.dropRate;
        if (total <= 0f) return;

        float roll = Random.Range(0f, total);
        float acumulado = 0f;

        foreach (Drops d in drops)
        {
            acumulado += d.dropRate;
            if (roll <= acumulado)
            {
                if (d.prefab != null)
                    Instantiate(d.prefab, transform.position, Quaternion.identity);
                return;
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
