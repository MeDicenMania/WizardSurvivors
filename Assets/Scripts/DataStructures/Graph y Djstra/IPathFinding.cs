namespace ED262C
{
    public interface IPathfindingSolver<T>
    {
        PathResult<T> FindPath(
            Graph<T> graph,
            T start,
            T destination);
    }

    public class PathResult<T>
    {
        public bool Found { get; private set; }
        public ISimpleList<T> Path { get; private set; }
        public int TotalCost { get; private set; }

        public PathResult(
            bool found,
            ISimpleList<T> path,
            int totalCost)
        {
            Found = found;
            Path = path;
            TotalCost = totalCost;
        }
    }
}