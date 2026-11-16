using UnityEngine;
using UnityEngine.SceneManagement;

// Loads a random level scene while preventing two consecutive selections
// from loading the same scene.

public class RandomLevelSceneController : MonoBehaviour
{
    [Tooltip("Scene names included in the random level selection. Add each level to Build Settings too.")]
    [SerializeField] private string[] levelSceneNames;

    // Static state survives the transition to the selected scene.
    private static string previousSceneName;

    // Selects and loads a level from the configured scene names.
    // Connect this method to the Play button's OnClick event.
    
    public void LoadRandomLevel()
    {
        if (levelSceneNames == null || levelSceneNames.Length == 0)
        {
            Debug.LogWarning("No hay escenas de nivel configuradas.", this);
            return;
        }

        SimpleArraySet<string> candidates = new SimpleArraySet<string>(levelSceneNames.Length);
        for (int i = 0; i < levelSceneNames.Length; i++)
        {
            string sceneName = levelSceneNames[i];
            if (string.IsNullOrWhiteSpace(sceneName))
                continue;

            if (!Application.CanStreamedLevelBeLoaded(sceneName))
            {
                Debug.LogWarning("La escena '" + sceneName + "' no está incluida en Build Settings.", this);
                continue;
            }

            if (sceneName != previousSceneName)
                candidates.Add(sceneName);
        }

        // If only the previous level is configured, there is no valid next choice.
        if (candidates.IsEmpty)
        {
            Debug.LogWarning("No hay una escena de nivel alternativa a '" + previousSceneName + "'. Agrega al menos otra escena válida.", this);
            return;
        }

        string[] availableScenes = candidates.ToArray();
        string selectedScene = availableScenes[Random.Range(0, availableScenes.Length)];
        previousSceneName = selectedScene;
        SceneManager.LoadScene(selectedScene);
    }
}
