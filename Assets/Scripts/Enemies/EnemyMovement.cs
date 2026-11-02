
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    private EnemyStats enemy;
    private Transform player;
    private EnemyPool enemyPool;
    void Start()
    {
        enemy = GetComponent<EnemyStats>();
        BuscarJugador();
    }

    void BuscarJugador()
    {
        PlayerController playerController =
            FindAnyObjectByType<PlayerController>();

        if (playerController != null)
        {
            player = playerController.transform;
        }
    }

    void Update()
    {
        // Si el jugador fue destruido o no existe, lo buscamos de nuevo.
        if (player == null)
        {
            BuscarJugador();
            return;
        }

        // Comprobamos que el enemigo tenga sus estadísticas.
        if (enemy == null)
        {
            enemy = GetComponent<EnemyStats>();

            if (enemy == null)
                return;
        }

        // Movimiento original del enemigo hacia el jugador.
        transform.position = Vector2.MoveTowards(
            transform.position,
            player.position,
            enemy.currentMoveSpeed * Time.deltaTime
        );
    }
    public void SetPool(EnemyPool pool)
    {
        enemyPool = pool;
    }
}
