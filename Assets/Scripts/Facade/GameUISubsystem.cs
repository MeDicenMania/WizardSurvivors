using UnityEngine;
using TMPro;

public class GameUISubsystem : MonoBehaviour
{
    [Header("Paneles Opcionales de UI")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private TextMeshProUGUI statusText;

    public void ShowGameOverUI()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        if (statusText != null)
            statusText.text = "GAME OVER";

        Debug.Log("[GameUISubsystem] Mostrando pantalla de Game Over.");
    }

    public void SetPauseUI(bool isPaused)
    {
        if (pausePanel != null)
            pausePanel.SetActive(isPaused);

        if (statusText != null)
            statusText.text = isPaused ? "PAUSA" : "";

        Debug.Log($"[GameUISubsystem] UI de Pausa: {(isPaused ? "Activada" : "Desactivada")}");
    }

    public void ShowLevelUpNotification(int newLevel)
    {
        if (statusText != null)
            statusText.text = $"¡NIVEL {newLevel}!";

        Debug.Log($"[GameUISubsystem] ¡Subiste al Nivel {newLevel}!");
    }
}
