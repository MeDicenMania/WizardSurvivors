using UnityEngine;
using UnityEngine.SceneManagement;
public class SceneController : MonoBehaviour
{
    [SerializeField] private LevelSceneManager levelSceneManager;

    public void SceneChange(string name)
    {
        SceneManager.LoadScene(name);
        Time.timeScale = 1;
    }
    public void Quit()
    {
        Application.Quit();
    }

    public void SceneChangeNextLevel()
    {
        levelSceneManager.LoadNextLevel();
    }

}
