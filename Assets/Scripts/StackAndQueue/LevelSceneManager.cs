using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

//LIFO.
public interface ISimpleStack<T>
{
	int Count { get; }
	bool IsEmpty { get; }
	void Push(T item);
	T Pop();
	T Peek();
	void Clear();
	T[] ToArray();
}

//FIFO.
public interface ISimpleQueue<T>
{
	int Count { get; }
	bool IsEmpty { get; }
	void Enqueue(T item);
	T Dequeue();
	T Peek();
	void Clear();
	T[] ToArray();
}

// Implementación estática redimensionable de Stack.
// El último elemento agregado es el primero en salir (LIFO).
public class SimpleArrayStack<T> : ISimpleStack<T>
{
	private T[] internalArray;
	private int count;
	private const int defaultCapacity = 4;

	public int Count => count;
	public bool IsEmpty => count == 0;

	public SimpleArrayStack(int capacity = defaultCapacity)
	{
		ValidateSize(capacity);
		internalArray = new T[capacity];
		count = 0;
	}

	public void Push(T item)
	{
		if (count == internalArray.Length)
			Resize(internalArray.Length * 2);

		internalArray[count] = item;
		count++;
	}

	public T Pop()
	{
		if (IsEmpty)
			throw new InvalidOperationException("No se puede hacer Pop: el Stack está vacío.");

		int topIndex = count - 1;
		T item = internalArray[topIndex];
		internalArray[topIndex] = default;
		count--;

		return item;
	}

	public T Peek()
	{
		if (IsEmpty)
			throw new InvalidOperationException("No se puede hacer Peek: el Stack está vacío.");

		return internalArray[count - 1];
	}

	public void Clear()
	{
		Array.Clear(internalArray, 0, count);
		count = 0;
	}

	public T[] ToArray()
	{
		T[] result = new T[count];

		for (int i = 0; i < count; i++)
			result[i] = internalArray[i];

		return result;
	}

	private void ValidateSize(int size)
	{
		if (size <= 0)
			throw new ArgumentOutOfRangeException(nameof(size), "La capacidad debe ser mayor que cero.");
	}

	private void Resize(int newCapacity)
	{
		T[] newArray = new T[newCapacity];

		for (int i = 0; i < count; i++)
			newArray[i] = internalArray[i];

		internalArray = newArray;
	}
}

// Implementación estática redimensionable de Queue.
// El primer elemento agregado es el primero en salir (FIFO).

public class SimpleArrayQueue<T> : ISimpleQueue<T>
{
	private T[] internalArray;
	private int count;
	private const int defaultCapacity = 4;

	public int Count => count;
	public bool IsEmpty => count == 0;

	public SimpleArrayQueue(int capacity = defaultCapacity)
	{
		ValidateSize(capacity);
		internalArray = new T[capacity];
		count = 0;
	}

	public void Enqueue(T item)
	{
		if (count == internalArray.Length)
			Resize(internalArray.Length * 2);

		internalArray[count] = item;
		count++;
	}

	public T Dequeue()
	{
		if (IsEmpty)
			throw new InvalidOperationException("No se puede hacer Dequeue: la Queue está vacía.");

		T item = internalArray[0];

		for (int i = 1; i < count; i++)
			internalArray[i - 1] = internalArray[i];

		count--;
		internalArray[count] = default;

		return item;
	}

	public T Peek()
	{
		if (IsEmpty)
			throw new InvalidOperationException("No se puede hacer Peek: la Queue está vacía.");

		return internalArray[0];
	}

	public void Clear()
	{
		Array.Clear(internalArray, 0, count);
		count = 0;
	}

	public T[] ToArray()
	{
		T[] result = new T[count];

		for (int i = 0; i < count; i++)
			result[i] = internalArray[i];

		return result;
	}

	private void ValidateSize(int size)
	{
		if (size <= 0)
			throw new ArgumentOutOfRangeException(nameof(size), "La capacidad debe ser mayor que cero.");
	}

	private void Resize(int newCapacity)
	{
		T[] newArray = new T[newCapacity];

		for (int i = 0; i < count; i++)
			newArray[i] = internalArray[i];

		internalArray = newArray;
	}
}


// Administra la selección aleatoria de escenas de nivel.
// Queue contiene los niveles pendientes del ciclo actual.
// Stack conserva el historial para poder volver al nivel anterior.


public class LevelSceneManager : MonoBehaviour
{
	[Header("Niveles disponibles")]
	[Tooltip("Nombres exactos de las escenas de nivel, tal como aparecen en Build Settings.")]
	[SerializeField] private string[] levelSceneNames;

	private ISimpleStack<string> levelStack;
	private ISimpleQueue<string> upcomingLevels;

	private string lastSelectedLevel;

	private static LevelSceneManager instance;

	private void Awake()
	{
		// Evita crear múltiples administradores.
		if (instance != null && instance != this)
		{
			Destroy(gameObject);
			return;
		}

		instance = this;

		// Mantiene el administrador entre cambios de escena.
		DontDestroyOnLoad(gameObject);

		levelStack = new SimpleArrayStack<string>();
		upcomingLevels = new SimpleArrayQueue<string>();

		FillQueueWithShuffledLevels();
	}

	// Carga el siguiente nivel de la Queue.
	public void LoadNextLevel()
	{
		if (upcomingLevels == null || upcomingLevels.IsEmpty)
		{
			FillQueueWithShuffledLevels();
		}

		if (upcomingLevels.IsEmpty)
		{
			Debug.LogError("LevelSceneManager: no hay niveles disponibles.");
			return;
		}

		string nextLevel = upcomingLevels.Dequeue();

		lastSelectedLevel = nextLevel;

		Debug.Log("Cargando siguiente nivel: " + nextLevel);

		SceneManager.LoadScene(nextLevel);
	}

	private void FillQueueWithShuffledLevels()
	{
		upcomingLevels.Clear();
		levelStack.Clear();

		if (levelSceneNames == null || levelSceneNames.Length == 0)
		{
			Debug.LogError("LevelSceneManager: no hay escenas configuradas.");
			return;
		}

		// Copia los nombres válidos y evita duplicados.
		List<string> availableLevels = new List<string>();

		foreach (string sceneName in levelSceneNames)
		{
			if (string.IsNullOrWhiteSpace(sceneName))
				continue;

			if (availableLevels.Contains(sceneName))
			{
				Debug.LogWarning("Nivel duplicado ignorado: " + sceneName);
				continue;
			}

			availableLevels.Add(sceneName);
		}

		if (availableLevels.Count == 0)
		{
			Debug.LogError("LevelSceneManager: no hay nombres de escenas válidos.");
			return;
		}

		// Mezcla aleatoriamente los niveles.
		ShuffleLevels(availableLevels);

		// Evita que el primer nivel del nuevo ciclo
		// sea igual al último nivel del ciclo anterior.
		PreventConsecutiveRepetition(availableLevels);

		// STACK:
		// Insertamos los niveles en orden inverso.
		// Al hacer Pop, recuperamos el orden aleatorio original.
		for (int i = availableLevels.Count - 1; i >= 0; i--)
		{
			levelStack.Push(availableLevels[i]);
		}

		// QUEUE:
		// Transferimos los niveles del Stack a la Queue.
		// La Queue conserva el orden de salida.
		while (!levelStack.IsEmpty)
		{
			upcomingLevels.Enqueue(levelStack.Pop());
		}
	}

	private void ShuffleLevels(List<string> levels)
	{
		// Algoritmo Fisher-Yates.
		for (int i = levels.Count - 1; i > 0; i--)
		{
			int randomIndex = UnityEngine.Random.Range(0, i + 1);

			string temp = levels[i];
			levels[i] = levels[randomIndex];
			levels[randomIndex] = temp;
		}
	}

	private void PreventConsecutiveRepetition(List<string> levels)
	{
		if (levels.Count <= 1 || string.IsNullOrEmpty(lastSelectedLevel))
			return;

		if (levels[0] == lastSelectedLevel)
		{
			// Elegimos una posición distinta de la primera.
			int swapIndex = UnityEngine.Random.Range(1, levels.Count);

			string temp = levels[0];
			levels[0] = levels[swapIndex];
			levels[swapIndex] = temp;
		}
	}
}

