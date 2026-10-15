using UnityEngine;




[CreateAssetMenu(fileName ="SpellScriptableObject", menuName = "ScriptableObjects/Spell")]
public class SpellsScriptableObjects : ScriptableObject
{
    [SerializeField]
    GameObject prefab;

    public GameObject Prefab {get => prefab; private set => prefab = value; }

    [SerializeField]
    float damage;
    public float Damage {get => damage;private set => damage = value;  }

    [SerializeField]
    float speed;
    public float Speed {get => speed;private set => speed = value; }

    [SerializeField]
    float cooldown;
    public float Cooldown { get => cooldown;private set => cooldown = value; }

    [SerializeField]
    float pierce;
    public float Pierce { get => pierce;private set => pierce = value; }
}
