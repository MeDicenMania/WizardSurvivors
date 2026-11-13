using UnityEngine;

[CreateAssetMenu(fileName = "PassiveItemsScriptableObjects",menuName = "Scriptableobjects/Passive Item")]
public class PassiveItemScriptableObject : ScriptableObject
{
    [SerializeField]
    float multiplier;
    public float Multiplier { get => multiplier; private set => multiplier = value;  }
}
