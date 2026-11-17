
using System;
using System.Collections.Generic;

namespace ED262C
{
    public class SimpleDictionary<TKey, TValue> : ISimpleDictionary<TKey, TValue>
    {
        private SimpleArrayList<TKey> keys = new SimpleArrayList<TKey>();
        private SimpleArrayList<TValue> values = new SimpleArrayList<TValue>();

        public int Count => keys.Count;

        // Permite acceder a un valor usando su clave.
        public TValue this[TKey key]
        {
            get
            {
                int index = FindIndex(key);

                if (index == -1)
                    throw new KeyNotFoundException("Key not found.");

                return values[index];
            }

            set
            {
                int index = FindIndex(key);

                if (index == -1)
                    Add(key, value);
                else
                    values[index] = value;
            }
        }

        // Agrega una nueva clave y su valor.
        public void Add(TKey key, TValue value)
        {
            if (ContainsKey(key))
                throw new ArgumentException("The key already exists.");

            keys.Add(key);
            values.Add(value);
        }

        // Busca la posición de una clave.
        private int FindIndex(TKey key)
        {
            for (int i = 0; i < keys.Count; i++)
            {
                if (EqualityComparer<TKey>.Default.Equals(keys[i], key))
                    return i;
            }

            return -1;
        }

        // Comprueba si existe una clave.
        public bool ContainsKey(TKey key)
        {
            return FindIndex(key) != -1;
        }

        // Obtiene un valor sin generar una excepción si no existe.
        public bool TryGetValue(TKey key, out TValue value)
        {
            int index = FindIndex(key);

            if (index == -1)
            {
                value = default(TValue);
                return false;
            }

            value = values[index];
            return true;
        }

        // Elimina una clave y su valor correspondiente.
        public bool Remove(TKey key)
        {
            int index = FindIndex(key);

            if (index == -1)
                return false;

            keys.RemoveAt(index);
            values.RemoveAt(index);

            return true;
        }

        // Devuelve una copia de todas las claves.
        public ISimpleList<TKey> GetKeys()
        {
            SimpleArrayList<TKey> result = new SimpleArrayList<TKey>();
            result.AddRange(keys.ToArray());

            return result;
        }

        // Devuelve una copia de todos los valores.
        public ISimpleList<TValue> GetValues()
        {
            SimpleArrayList<TValue> result = new SimpleArrayList<TValue>();
            result.AddRange(values.ToArray());

            return result;
        }

        // Vacía el diccionario.
        public void Clear()
        {
            keys.Clear();
            values.Clear();
        }
    }
}
