using UnityEngine;
using UnityEngine.SceneManagement;
public class SceneController : MonoBehaviour
{
    [SerializeField] private LevelSceneManager levelSceneManager;

    public void SceneChange(string name)
    {
        SceneManager.LoadScene(name);
    }

    public void SceneChangeNextLevel()
    {
        levelSceneManager.LoadNextLevel();
    }

}
