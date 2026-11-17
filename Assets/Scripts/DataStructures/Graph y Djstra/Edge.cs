public class Edge<T>
{
    public T From { get; private set; }
    public T To { get; private set; }
    public int Cost { get; private set; }

    public Edge(T from, T to, int cost)
    {
        From = from;
        To = to;
        Cost = cost;
    }
}