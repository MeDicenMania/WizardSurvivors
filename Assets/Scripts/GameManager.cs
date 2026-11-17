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
        LevelUp,
        InShop
    }

    public GameState currentState;  //estado actual de la partida

    public GameState previusState; //estado anterior al que ya hicimos

    [Header("UI")]
    //pantallas
    public GameObject pauseScreen;
    public GameObject resultScreen;
    public GameObject levelUpScreen;
    public GameObject shopScreen;

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
    private InventoryManager inventory;
    //BOOLS
    public bool ischoosingUpgrades = false; //chequea si el jugador esta eligiendo mejoras osea se pausa el tiempo todo

    public bool isGameOver = false;
    public static GameManager instance;

    [Header("Shop")]
    public bool isShoping = false;
    public TextMeshProUGUI cheapestOption;
    public ShopManager shopManager;

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
        inventory = FindAnyObjectByType<InventoryManager>();
        DisableMenus();
    }
    void Update()
    {
        // La tecla P se lee siempre, igual que Escape en la pausa
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (currentState == GameState.InShop)
                EndShoping();
            else if (currentState == GameState.Gameplay)
                StartShoping();
        }

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
                if (!isGameOver)
                {
                    isGameOver = true;
                    Time.timeScale = 0;
                    DisplayResults();
                }
                break;

            case GameState.LevelUp:
                if (!ischoosingUpgrades)
                {
                    ischoosingUpgrades = true;
                    Time.timeScale = 0;
                    levelUpScreen.SetActive(true);
                }
                break;

            case GameState.InShop:
                CheckForPause();
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

            if (inventory != null)
            {
                inventory.ShowLastUpgrade();
            }

            
        }

       
    }
    public void ResumeGame()
    {
        if( currentState == GameState.Paused)
        {
            ChangeState(previusState);
                     //despausa
            pauseScreen.SetActive(false);
            Debug.Log("saliste de pausa");
            bool menuOpen = previusState == GameState.InShop
                    || previusState == GameState.LevelUp;
            Time.timeScale = menuOpen ? 0 : 1;
        }


    }

    public void CheckShop()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (currentState == GameState.InShop)
            {
                EndShoping();
            }
            else
            {
                StartShoping();
            }
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
        shopScreen.SetActive(false);
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
    public void StartShoping()
    {
        ChangeState(GameState.InShop);
        isShoping = true;
        Time.timeScale = 0;
        shopScreen.SetActive(true);
    }

    public void EndShoping()
    {
        isShoping = false;
        Time.timeScale = 1;
        shopScreen.SetActive(false);
        ChangeState(GameState.Gameplay);
    }


}
