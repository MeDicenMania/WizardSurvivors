using UnityEngine;

public class CharacterSelector : MonoBehaviour
{
    public static CharacterSelector instance;

    public WizardsScriptableObject wizardData;

    private void Awake()
    {
        if( instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Debug.Log("extra+", this);
        }
    }
    public static WizardsScriptableObject GetData()
    {
        return instance.wizardData;
    }

    public void SelectCharacter(WizardsScriptableObject character)
    {
        wizardData = character;
    }

    public void DestroySingleton()
    {
        instance = null;
        Destroy(gameObject);
    }
}
