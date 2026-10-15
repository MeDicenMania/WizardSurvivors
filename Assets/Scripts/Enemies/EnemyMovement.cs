using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public EnemyScriptableOjbects enemyData;
    Transform player;
   
    void Start()
    {
        player = FindAnyObjectByType<PlayerController>().transform;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector2.MoveTowards(transform.position, player.transform.position, enemyData.MoveSpeed * Time.deltaTime);
    }
}
