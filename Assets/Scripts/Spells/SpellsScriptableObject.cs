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


    [SerializeField]
    int level;              //no lo cambies in game
    public int Level { get => level; private set => level = value; }
    [SerializeField]
    GameObject nextLevelPrefab;              //no lo cambies in game
    public GameObject NextLevelPrefab { get => nextLevelPrefab; private set => nextLevelPrefab = value; }

    [SerializeField]
    Sprite icon;        //no lo cambies en game/runtime
    public Sprite Icon {get => icon;private set => icon = value; }

    [SerializeField]
    string name;        //no lo cambies en game/runtime
    public string Name { get => name; private set => name = value; }

    [SerializeField]
    string description;        //no lo cambies en game/runtime           // !Que es el hechizo tipo como funciona pero si es una mejora pone que mejora
    public string Description { get => description; private set => description = value; }



}
