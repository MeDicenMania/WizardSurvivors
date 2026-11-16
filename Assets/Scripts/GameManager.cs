using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
public class GameManager : MonoBehaviour
{
   public enum GameState  //diferentes estados del juego
    {
        Gameplay,
        Paused,
        GameOver,
        LevelUp
    }

    public GameState currentState;  //estado actual de la partida

    public GameState previusState; //estado anterior al que ya hicimos

    [Header("UI")]
    //pantallas
    public GameObject pauseScreen;
    public GameObject resultScreen;
    public GameObject levelUpScreen;

    //Variables de resultados y so
    public TextMeshProUGUI currentDamageUI;
    public TextMeshProUGUI currentMoveSpeedUI;
    public TextMeshProUGUI currentRegenUI;
    public TextMeshProUGUI currentMagnetUI;
    public TextMeshProUGUI currentProjectileSpeedUI;
  
    public Image wizardImage;
    public TextMeshProUGUI wizardName;
    public TextMeshProUGUI maxLevelUI;

    public float timeLimit; //tiempo limite nada mas
    float stopWatchTime;  // el tiempo que de verdad paso desde que se activo
    public TextMeshProUGUI stopWatchDisplay;        //tiempo que se muestra arriba
    public TextMeshProUGUI timereachedUI; // tiempo que se muestra al final de la run cuando moris digamos

    public GameObject playerobject;

    //BOOLS
    public bool ischoosingUpgrades = false; //chequea si el jugador esta eligiendo mejoras osea se pausa el tiempo todo

    public bool isGameOver = false;
    public static GameManager instance;
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogError("extra manager");
            Destroy(gameObject);
        }
        DisableMenus();
    }
    void Update()
    {
       
        switch (currentState)
        {
            case GameState.Gameplay:
                CheckForPause();
                UpdateStopWatch();
                break;

            case GameState.Paused:

                CheckForPause();
                break;

            case GameState.GameOver:
                if(!isGameOver)
                {
                    isGameOver = true;
                    Time.timeScale = 0;
                    Debug.Log("Game is over");
                    DisplayResults();
                }
                break;
            case GameState.LevelUp:
                if (!ischoosingUpgrades)
                {
                    ischoosingUpgrades = true;
                    Time.timeScale = 0;
                    levelUpScreen.SetActive(true);
                    Debug.Log("upgrades");
                }
                break;
            default:
                Debug.LogError("invalidstate");
                break;
        }
    }
    public void ChangeState(GameState newState)
    {
        currentState = newState;
    }
    public void PauseGame()
    {
        if( currentState!= GameState.Paused)
        {
            previusState = currentState;
            ChangeState(GameState.Paused);
            Time.timeScale = 0;               //esto pausa el tiempo o sea pausa el juego digamos
            pauseScreen.SetActive(true);
            Debug.Log("en pausa");
        }

       
    }
    public void ResumeGame()
    {
        if( currentState == GameState.Paused)
        {
            ChangeState(previusState);
            Time.timeScale = 1;          //despausa
            pauseScreen.SetActive(false);
            Debug.Log("saliste de pausa");
        }


    }


    void CheckForPause()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if(currentState == GameState.Paused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }
     void UpdateStopWatch()
    {
        stopWatchTime += Time.deltaTime;
        UpdatateStopWatchDisplay();
        if(stopWatchTime >= timeLimit)
        {
            GameOver();
        }

    }

     void UpdatateStopWatchDisplay()
    {
        int minutes = Mathf.FloorToInt(stopWatchTime/ 60);
        int seconds = Mathf.FloorToInt(stopWatchTime% 60);

        stopWatchDisplay.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    void DisableMenus()
    {
        pauseScreen.SetActive(false);
        resultScreen.SetActive(false);
        levelUpScreen.SetActive(false);
    }


    public void GameOver()
    {
        ChangeState(GameState.GameOver);
        timereachedUI.text = stopWatchDisplay.text;
    }

    void DisplayResults()
    {
        resultScreen.SetActive(true);
    }

    public void AssingWizardUI(WizardsScriptableObject wizardChosen)
    {
        wizardImage.sprite = wizardChosen.Icon;
        wizardName.text = wizardChosen.Name;
    }

    public void AssingLevelReachedUI(int levelReached)
    {
        maxLevelUI.text = levelReached.ToString();
    }
    public void StartLevelUp()
    {
        ChangeState(GameState.LevelUp);
        playerobject.SendMessage("RemoveandApllyUI");

    }
    public void EndLevelUp()
    {
        ischoosingUpgrades = false;
        Time.timeScale = 1;
        levelUpScreen.SetActive(false);
        ChangeState(GameState.Gameplay);
    }

}
