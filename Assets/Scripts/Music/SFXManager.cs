using UnityEngine;

public class SFXManager : MonoBehaviour
{
    public static SFXManager Instance { get; private set; }

    [Header("Audio")]
    public AudioSource sfxSource;

    [Header("Clips")]
    public AudioClip hoverClip;
    public AudioClip clickClip;
    public AudioClip unlockClip;

    [Header("Settings")]
    public bool soundEnabled = true;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip == null || sfxSource == null || !soundEnabled) return;
        sfxSource.PlayOneShot(clip);
    }

    public void SetSoundEnabled(bool value)
    {
        soundEnabled = value;
    }
}
