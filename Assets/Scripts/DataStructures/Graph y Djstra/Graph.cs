using System;
using System.Collections.Generic;

namespace ED262C
{
    public class Graph<T>
    {
        private ISimpleDictionary<T, ISimpleList<Edge<T>>> adjacencyList;

        public int VertexCount => adjacencyList.Count;

        public Graph()
        {
            adjacencyList =
                new SimpleDictionary<T, ISimpleList<Edge<T>>>();
        }

    
        public void AddVertex(T vertex)
        {
            if (adjacencyList.ContainsKey(vertex))
                return;

            adjacencyList.Add(
                vertex,
                new SimpleArrayList<Edge<T>>()
            );
        }

        public bool ContainsVertex(T vertex)
        {
            return adjacencyList.ContainsKey(vertex);
        }

   
        public void AddEdge(
            T from,
            T to,
            int cost,
            bool bidirectional = false)
        {
            if (cost < 0)
                throw new ArgumentException(
                    "Dijkstra no admite costos negativos."
                );

            AddVertex(from);
            AddVertex(to);

            ISimpleList<Edge<T>> edges =
                adjacencyList[from];

            edges.Add(
                new Edge<T>(from, to, cost)
            );

            if (bidirectional)
            {
                ISimpleList<Edge<T>> reverseEdges =
                    adjacencyList[to];

                reverseEdges.Add(
                    new Edge<T>(to, from, cost)
                );
            }
        }

   
        public ISimpleList<Edge<T>> GetEdges(T vertex)
        {
            if (!adjacencyList.ContainsKey(vertex))
                return new SimpleArrayList<Edge<T>>();

            return adjacencyList[vertex];
        }

        public bool HasEdge(T from, T to)
        {
            if (!adjacencyList.ContainsKey(from))
                return false;

            ISimpleList<Edge<T>> edges =
                adjacencyList[from];

            for (int i = 0; i < edges.Count; i++)
            {
                if (EqualityComparer<T>.Default.Equals(
                    edges[i].To,
                    to))
                {
                    return true;
                }
            }

            return false;
        }

     
        public bool RemoveEdge(
            T from,
            T to,
            bool bidirectional = false)
        {
            bool removed =
                RemoveSingleEdge(from, to);

            if (bidirectional)
            {
                RemoveSingleEdge(to, from);
            }

            return removed;
        }

        private bool RemoveSingleEdge(T from, T to)
        {
            if (!adjacencyList.ContainsKey(from))
                return false;

            ISimpleList<Edge<T>> edges =
                adjacencyList[from];

            for (int i = 0; i < edges.Count; i++)
            {
                if (EqualityComparer<T>.Default.Equals(
                    edges[i].To,
                    to))
                {
                    edges.RemoveAt(i);
                    return true;
                }
            }

            return false;
        }

        public bool RemoveVertex(T vertex)
        {
            if (!adjacencyList.ContainsKey(vertex))
                return false;

            adjacencyList.Remove(vertex);

            ISimpleList<T> vertices =
                adjacencyList.GetKeys();

            for (int i = 0; i < vertices.Count; i++)
            {
                ISimpleList<Edge<T>> edges =
                    adjacencyList[vertices[i]];

                for (int j = edges.Count - 1; j >= 0; j--)
                {
                    if (EqualityComparer<T>.Default.Equals(
                        edges[j].To,
                        vertex))
                    {
                        edges.RemoveAt(j);
                    }
                }
            }

            return true;
        }

        public ISimpleList<T> GetVertices()
        {
            return adjacencyList.GetKeys();
        }

    

        public void Clear()
        {
            adjacencyList.Clear();
        }
    }
}