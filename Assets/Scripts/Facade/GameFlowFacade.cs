using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// PATRÓN FACADE: Proporciona una interfaz única y simple para coordinar múltiples
/// subsistemas complejos del juego (Jugador, Spawner, UI, Audio y Escenas/Tiempo).
/// </summary>
public class GameFlowFacade : MonoBehaviour
{
    [Header("Subsistemas a coordinar")]
    [SerializeField] private PlayerController playerController;
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private Spawner enemySpawner;
    [SerializeField] private SceneController sceneController;
    [SerializeField] private GameUISubsystem uiSubsystem;
    [SerializeField] private GameAudioSubsystem audioSubsystem;

    private bool isPaused = false;
    private bool isGameOver = false;

    private void Awake()
    {
        // Auto-búsqueda de subsistemas por si no fueron arrastrados en el Inspector
        if (playerController == null) playerController = FindAnyObjectByType<PlayerController>();
        if (playerStats == null) playerStats = FindAnyObjectByType<PlayerStats>();
        if (enemySpawner == null) enemySpawner = FindAnyObjectByType<Spawner>();
        if (sceneController == null) sceneController = FindAnyObjectByType<SceneController>();

        if (uiSubsystem == null)
        {
            uiSubsystem = GetComponent<GameUISubsystem>();
            if (uiSubsystem == null) uiSubsystem = FindAnyObjectByType<GameUISubsystem>();
            if (uiSubsystem == null) uiSubsystem = gameObject.AddComponent<GameUISubsystem>();
        }

        if (audioSubsystem == null)
        {
            audioSubsystem = GetComponent<GameAudioSubsystem>();
            if (audioSubsystem == null) audioSubsystem = FindAnyObjectByType<GameAudioSubsystem>();
            if (audioSubsystem == null) audioSubsystem = gameObject.AddComponent<GameAudioSubsystem>();
        }
    }

    private void Update()
    {
        // Si GameManager ya maneja Escape, evitamos doble toggle
        if (GameManager.instance == null && !isGameOver && Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    /// <summary>
    /// Método unificado de la Fachada para ejecutar el Game Over en todos los subsistemas.
    /// </summary>
    public void TriggerGameOver()
    {
        if (isGameOver) return;
        isGameOver = true;

        // 0. Subsistema GameManager (si existe en la escena)
        if (GameManager.instance != null && !GameManager.instance.isGameOver)
        {
            if (playerStats != null)
                GameManager.instance.AssingLevelReachedUI(playerStats.level);
            GameManager.instance.GameOver();
        }

        // 1. Subsistema del Jugador: frenar movimiento y apagar hechizos
        if (playerController != null)
        {
            playerController.enabled = false;
            Rigidbody2D rb = playerController.GetComponent<Rigidbody2D>();
            if (rb != null) rb.linearVelocity = Vector2.zero;
        }

        if (playerStats != null && playerStats.spawnedSpeells != null)
        {
            foreach (GameObject spell in playerStats.spawnedSpeells)
            {
                if (spell != null) spell.SetActive(false);
            }
        }

        // 2. Subsistema de Enemigos: frenar oleadas del Spawner
        if (enemySpawner != null)
        {
            enemySpawner.StopAllCoroutines();
            enemySpawner.enabled = false;
        }

        // 3. Subsistema de Audio
        if (audioSubsystem != null)
            audioSubsystem.PlayGameOverSound();

        // 4. Subsistema de UI
        if (uiSubsystem != null)
            uiSubsystem.ShowGameOverUI();

        // 5. Subsistema de Tiempo
        Time.timeScale = 0f;
    }

    /// <summary>
    /// Método unificado de la Fachada para pausar o reanudar la partida.
    /// </summary>
    public void TogglePause()
    {
        if (isPaused) ResumeGame();
        else PauseGame();
    }

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;

        if (playerController != null) playerController.enabled = false;
        if (audioSubsystem != null) audioSubsystem.SetMusicPaused(true);
        if (uiSubsystem != null) uiSubsystem.SetPauseUI(true);
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;

        if (playerController != null) playerController.enabled = true;
        if (audioSubsystem != null) audioSubsystem.SetMusicPaused(false);
        if (uiSubsystem != null) uiSubsystem.SetPauseUI(false);
    }

    /// <summary>
    /// Método unificado de la Fachada cuando el jugador sube de nivel.
    /// </summary>
    public void NotifyLevelUp(int newLevel)
    {
        // 1. Subsistema del Jugador: pequeña curación al subir de nivel
        if (playerStats != null)
            playerStats.RestoreHealth(10f);

        // 2. Subsistema de Audio
        if (audioSubsystem != null)
            audioSubsystem.PlayLevelUpSound();

        // 3. Subsistema de UI
        if (uiSubsystem != null)
            uiSubsystem.ShowLevelUpNotification(newLevel);
    }

    /// <summary>
    /// Método unificado de la Fachada para reiniciar la escena actual.
    /// </summary>
    public void RestartGame()
    {
        Time.timeScale = 1f;
        string currentScene = SceneManager.GetActiveScene().name;

        if (sceneController != null)
            sceneController.SceneChange(currentScene);
        else
            SceneManager.LoadScene(currentScene);
    }
}
