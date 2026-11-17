using UnityEngine;
using ED262C;

public class GraphTest : MonoBehaviour
{
    void Start()
    {
        Graph<string> graph = new Graph<string>();

        graph.AddEdge("Damage", "Critical", 10);
        graph.AddEdge("Damage", "Health", 5);
        graph.AddEdge("Health", "Critical", 2);
        graph.AddEdge("Critical", "Projectiles", 3);
        graph.AddEdge("Health", "Projectiles", 20);

        DijkstraSolver<string> dijkstra =
            new DijkstraSolver<string>();

        PathResult<string> result =
            dijkstra.FindPath(graph, "Damage", "Projectiles");

        Debug.Log("¿Encontró camino?: " + result.Found);
        Debug.Log("Costo total: " + result.TotalCost);

        for (int i = 0; i < result.Path.Count; i++)
        {
            Debug.Log(result.Path[i]);
        }
    }
}