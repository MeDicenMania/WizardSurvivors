using UnityEngine;

[CreateAssetMenu(fileName = "CharacterScriptableObject", menuName = ("ScriptableObjects/Wizards"))]
public class WizardsScriptableObject : ScriptableObject
{
    [SerializeField]
    Sprite  icon;
    public Sprite Icon { get => icon; private set => icon = value; }

    [SerializeField]
    string name;
    public string Name { get => name; private set => name = value; }




    [SerializeField]
    GameObject startingSpell;
    public GameObject StartingSpell { get => startingSpell; private set => startingSpell = value; }

    [SerializeField]
    float maxHealth;

    public float MaxHealth { get => maxHealth; private set => maxHealth = value; }
    
    [SerializeField]
    float regen;

    public float Regen { get => regen; private set => regen = value; }
    
    [SerializeField]
    float moveSpeed;

    public float MoveSpeed { get => moveSpeed; private set => moveSpeed = value; }

    [SerializeField]
    float damage;

    public float Damage { get => damage; private set => damage = value; }

    [SerializeField]
    float projectileSpeed;
    public float ProjectileSpeed { get => projectileSpeed; private set => projectileSpeed = value; }

    [SerializeField]
    float magnet;

    public float Magnet { get => magnet; private set => magnet = value; }

}
