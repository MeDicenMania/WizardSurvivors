using UnityEngine;

public class GameAudioSubsystem : MonoBehaviour
{
    [Header("Audio Opcional")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioClip gameOverClip;
    [SerializeField] private AudioClip levelUpClip;

    public void PlayGameOverSound()
    {
        if (musicSource != null)
            musicSource.Stop();

        if (sfxSource != null && gameOverClip != null)
            sfxSource.PlayOneShot(gameOverClip);

        Debug.Log("[GameAudioSubsystem] Reproduciendo sonido de Game Over y deteniendo música.");
    }

    public void SetMusicPaused(bool isPaused)
    {
        if (musicSource != null)
        {
            if (isPaused) musicSource.Pause();
            else musicSource.UnPause();
        }

        Debug.Log($"[GameAudioSubsystem] Música {(isPaused ? "pausada" : "reanudada")}.");
    }

    public void PlayLevelUpSound()
    {
        if (sfxSource != null && levelUpClip != null)
            sfxSource.PlayOneShot(levelUpClip);

        Debug.Log("[GameAudioSubsystem] Reproduciendo sonido de Level Up.");
    }
}
