
namespace ED262C
{
    public class DijkstraSolver<T> : IPathfindingSolver<T>
    {
        public PathResult<T> FindPath(
            Graph<T> graph, T start, T destination)
        {
            if (graph == null ||
                !graph.ContainsVertex(start) ||
                !graph.ContainsVertex(destination))
            {
                return new PathResult<T>(
                    false, new SimpleArrayList<T>(), int.MaxValue);
            }

            SimpleDictionary<T, int> distances =
                new SimpleDictionary<T, int>();

            SimpleDictionary<T, T> previous =
                new SimpleDictionary<T, T>();

            SimpleArrayList<T> unvisited =
                new SimpleArrayList<T>();

            // Inicializa las distancias.
            ISimpleList<T> vertices = graph.GetVertices();

            for (int i = 0; i < vertices.Count; i++)
            {
                T vertex = vertices[i];

                distances.Add(vertex, int.MaxValue);
                unvisited.Add(vertex);
            }

            distances[start] = 0;

            while (unvisited.Count > 0)
            {
                T current = default(T);
                int smallestDistance = int.MaxValue;
                bool foundCurrent = false;

                // Busca el nodo no visitado con menor distancia.
                for (int i = 0; i < unvisited.Count; i++)
                {
                    T vertex = unvisited[i];

                    if (distances[vertex] < smallestDistance)
                    {
                        smallestDistance = distances[vertex];
                        current = vertex;
                        foundCurrent = true;
                    }
                }

                if (!foundCurrent)
                    break;

                if (System.Collections.Generic.EqualityComparer<T>.Default.Equals(
                    current, destination))
                    break;

                unvisited.Remove(current);

                // Revisa las conexiones del nodo actual.
                ISimpleList<Edge<T>> edges = graph.GetEdges(current);

                for (int i = 0; i < edges.Count; i++)
                {
                    Edge<T> edge = edges[i];
                    T neighbor = edge.To;

                    if (!unvisited.Contains(neighbor))
                        continue;

                    if (smallestDistance > int.MaxValue - edge.Cost)
                        continue;

                    int newDistance = smallestDistance + edge.Cost;

                    if (newDistance < distances[neighbor])
                    {
                        distances[neighbor] = newDistance;

                        if (previous.ContainsKey(neighbor))
                            previous[neighbor] = current;
                        else
                            previous.Add(neighbor, current);
                    }
                }
            }

            // Si no se encontró un camino.
            if (distances[destination] == int.MaxValue)
            {
                return new PathResult<T>(
                    false, new SimpleArrayList<T>(), int.MaxValue);
            }

            // Reconstruye el camino.
            SimpleArrayList<T> path = new SimpleArrayList<T>();
            T step = destination;

            path.Add(step);

            while (!System.Collections.Generic.EqualityComparer<T>.Default.Equals(
                step, start))
            {
                if (!previous.ContainsKey(step))
                {
                    return new PathResult<T>(
                        false, new SimpleArrayList<T>(), int.MaxValue);
                }

                step = previous[step];
                path.Add(step);
            }

            path.Reverse();

            return new PathResult<T>(
                true, path, distances[destination]);
        }
    }
}
