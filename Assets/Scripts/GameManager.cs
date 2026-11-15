using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class GameManager : MonoBehaviour
{
   public enum GameState  //diferentes estados del juego
    {
        Gameplay,
        Paused,
        GameOver
    }

    public GameState currentState;  //estado actual de la partida

    public GameState previusState; //estado anterior al que ya hicimos

    [Header("UI")]
    public GameObject pauseScreen;
    public GameObject resultScreen;

    public TextMeshProUGUI currentDamageUI;
    public TextMeshProUGUI currentMoveSpeedUI;
    public TextMeshProUGUI currentRegenUI;
    public TextMeshProUGUI currentMagnetUI;
    public TextMeshProUGUI currentProjectileSpeedUI;


    public Image wizardImage;
    public TextMeshProUGUI wizardName;
    public TextMeshProUGUI maxLevelUI;

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


    void DisableMenus()
    {
        pauseScreen.SetActive(false);
        resultScreen.SetActive(false);
    }


    public void GameOver()
    {
        ChangeState(GameState.GameOver);
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

}
