using System;
using UnityEngine;

[CreateAssetMenu(fileName = "PassiveItemsScriptableObjects",menuName = "Scriptableobjects/Passive Item")]
public class PassiveItemScriptableObject : ScriptableObject
{
    [SerializeField]
    float multiplier;
    public float Multiplier { get => multiplier; private set => multiplier = value;  }
    [SerializeField]
    int level;              //no lo cambies in game
    public int Level { get => level; private set => level = value; }
    [SerializeField]
    GameObject nextLevelPrefab;              //no lo cambies in game
    public GameObject NextLevelPrefab { get => nextLevelPrefab; private set => nextLevelPrefab = value; }

    [SerializeField]
    Sprite icon;        //no lo cambies en game/runtime

    public Sprite Icon { get => icon; private set => icon = value; }
}
