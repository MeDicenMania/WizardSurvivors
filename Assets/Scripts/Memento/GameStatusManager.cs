using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// PATRÓN MEMENTO (Caretaker / Cuidador): Se encarga de mantener la pila (Stack)
/// de estados guardados y llamar a SaveState() / RestoreState() en el Originador.
/// </summary>
public class GameStatusManager : MonoBehaviour
{
    private Stack<PlayerMemento> savedStates = new Stack<PlayerMemento>();

    [Header("Originador")]
    public PlayerStats player;

    [Header("Teclas de Guardado / Restaurado")]
    [SerializeField] private KeyCode saveKey = KeyCode.G;
    [SerializeField] private KeyCode restoreKey = KeyCode.R;

    private void Awake()
    {
        if (player == null)
        {
            player = FindAnyObjectByType<PlayerStats>();
        }
    }

    private void Update()
    {
        if (player == null) return;

        if (Input.GetKeyDown(saveKey))
        {
            SaveGame();
        }

        if (Input.GetKeyDown(restoreKey) && savedStates.Count > 0)
        {
            RestoreLastSave();
        }
    }

    public void SaveGame()
    {
        if (player == null) return;

        // Guarda el estado actual del jugador en la pila
        savedStates.Push(player.SaveState());
        Debug.Log($"[Memento] Estado guardado (Pos: {player.transform.position}, Vida: {player.currentMaxHealth}, Nivel: {player.level}). Total guardados: {savedStates.Count}");
    }

    public void RestoreLastSave()
    {
        if (player == null || savedStates.Count == 0) return;

        // Restaura el último estado guardado del jugador
        PlayerMemento lastSavedState = savedStates.Pop();
        player.RestoreState(lastSavedState);
        Debug.Log($"[Memento] Estado restaurado (Pos: {lastSavedState.Position}, Vida: {lastSavedState.Health}, Nivel: {lastSavedState.Level}). Guardados restantes: {savedStates.Count}");
    }
}
