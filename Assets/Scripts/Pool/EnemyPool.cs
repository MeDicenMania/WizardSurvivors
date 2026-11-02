using UnityEngine;
using System.Collections.Generic;
public class EnemyPool : MonoBehaviour
{
    
    [SerializeField]  private GameObject enemies;
    Queue<GameObject> pool = new Queue<GameObject>();

    [SerializeField] public int tamaño;
    EnemySpawner spawner;


    void Start()
    {
        for (int i = 0; i < tamaño; i++)
        {
            GameObject newEnemy = Instantiate(enemies);
            enemies.SetActive(false);
            pool.Enqueue(newEnemy);
            newEnemy.GetComponent<EnemyMovement>().SetPool(this);


        }
    }
    public GameObject GetEnemies(Vector3 posicion)
    {
        GameObject newEnemy;

        if (pool.Count > 0)
        {
            newEnemy = pool.Dequeue();
        }
        else
        {
            newEnemy = Instantiate(enemies);
            newEnemy.GetComponent<EnemyMovement>().SetPool(this);
        }

        // Primero lo ubicamos donde queremos.
        newEnemy.transform.position = posicion;

        // Después lo activamos.
        newEnemy.SetActive(true);

        return newEnemy;
    }

    public void RecicleEnemy(GameObject enemy)
    {
        pool.Enqueue(enemy);
        enemy.SetActive(false);

    }


}
