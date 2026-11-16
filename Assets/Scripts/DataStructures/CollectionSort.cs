using System;

public static class CollectionSort
{
    // Ordena un array usando IComparable
    public static void SortArray<T>(T[] array, bool ascending = true) where T : IComparable<T>
    {
        SortArray(array, (a, b) => a.CompareTo(b), ascending);
    }

    // Ordena un array usando Comparison
    public static void SortArray<T>(T[] array, Comparison<T> comparison, bool ascending = true)
    {
        if (array == null || array.Length <= 1 || comparison == null)
        {
            return;
        }

        QuickSort(array, 0, array.Length - 1, comparison, ascending);
    }

    static void QuickSort<T>(T[] array, int low, int high, Comparison<T> comparison, bool ascending)
    {
        if (low < high)
        {
            int pivot = Partition(array, low, high, comparison, ascending);
            QuickSort(array, low, pivot - 1, comparison, ascending);
            QuickSort(array, pivot + 1, high, comparison, ascending);
        }
    }

    static int Partition<T>(T[] array, int low, int high, Comparison<T> comparison, bool ascending)
    {
        T pivot = array[high];
        int i = low - 1;

        for (int j = low; j < high; j++)
        {
            int cmp = comparison(array[j], pivot);
            bool beforePivot = ascending ? cmp < 0 : cmp > 0;

            if (beforePivot)
            {
                i++;
                Swap(array, i, j);
            }
        }

        Swap(array, i + 1, high);
        return i + 1;
    }

    static void Swap<T>(T[] array, int a, int b)
    {
        T aux = array[a];
        array[a] = array[b];
        array[b] = aux;
    }
}
