using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelRandomizer : MonoBehaviour
{
    [SerializeField] private List<string> levelScenes = new List<string>();

    private readonly Queue<string> levelQueue = new Queue<string>();
    private string lastLoadedScene;

    private void Awake()
    {
        QueueLevels();
    }

    public void QueueLevels()
    {
        levelQueue.Clear();

        HashSet<string> queuedScenes = new HashSet<string>();
        foreach (string sceneName in levelScenes)
        {
            if (string.IsNullOrWhiteSpace(sceneName))
            {
                continue;
            }

            if (!Application.CanStreamedLevelBeLoaded(sceneName))
            {
                Debug.LogError("LevelRandomizer: Scene '" + sceneName + "' is not included in the build settings.", this);
                continue;
            }

            if (queuedScenes.Add(sceneName))
            {
                levelQueue.Enqueue(sceneName);
            }
        }

        if (levelQueue.Count == 0)
        {
            Debug.LogError("LevelRandomizer: Add at least one valid scene name to Level Scenes.", this);
        }
    }

    public void LoadRandomLevel()
    {
        if (levelQueue.Count == 0)
        {
            QueueLevels();
        }

        if (levelQueue.Count == 0)
        {
            return;
        }

        List<string> queuedScenes = new List<string>(levelQueue);
        List<string> eligibleScenes = queuedScenes.FindAll(sceneName => sceneName != lastLoadedScene);
        if (eligibleScenes.Count == 0)
        {
            eligibleScenes = queuedScenes;
        }

        string selectedScene = eligibleScenes[Random.Range(0, eligibleScenes.Count)];
        levelQueue.Clear();

        foreach (string sceneName in queuedScenes)
        {
            if (sceneName != selectedScene)
            {
                levelQueue.Enqueue(sceneName);
            }
        }

        lastLoadedScene = selectedScene;
        SceneManager.LoadScene(selectedScene);
    }
}
