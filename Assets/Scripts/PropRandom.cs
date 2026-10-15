using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class PropRandomizer : MonoBehaviour
{

    public List <GameObject>  propSpawnPoints;
    public List <GameObject> propsPrefabs;
    void Start()
    {
        SpawnProps();
    }

   
    void Update()
    {
        
    }
    void SpawnProps()
    {
        foreach( GameObject sp in propSpawnPoints)
        {
            int rand = Random.Range(0, propsPrefabs.Count);
           GameObject prop =  Instantiate(propsPrefabs[rand], sp.transform.position, Quaternion.identity);
            prop.transform.parent = sp.transform;
        }
    }

}
