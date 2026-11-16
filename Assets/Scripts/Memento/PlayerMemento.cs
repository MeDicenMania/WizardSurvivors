using UnityEngine;

/// <summary>
/// PATRÓN MEMENTO (Memento): Clase que no hereda de MonoBehaviour y almacena
/// el estado interno del Originador (PlayerStats) sin violar su encapsulación.
/// </summary>
public class PlayerMemento
{
    public Vector3 Position { get; private set; }
    public float Health { get; private set; }
    public int Experience { get; private set; }
    public int Level { get; private set; }
    public int ExperienceCap { get; private set; }

    public PlayerMemento(Vector3 position, float health, int experience, int level, int experienceCap)
    {
        this.Position = position;
        this.Health = health;
        this.Experience = experience;
        this.Level = level;
        this.ExperienceCap = experienceCap;
    }
}
