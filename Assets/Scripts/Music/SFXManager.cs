using UnityEngine;


public class SFXManager : MonoBehaviour
{
    // tout le monde peut lire Instance mais seule la classe SFXManager peut la modif
    public static SFXManager Instance { get; private set; } // singleton simplifié comparé à SaveManager, pas de création automatique (vu qu'il existe deja en dure dans la scène)

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
        // le clip demandé n'existe pas
        // le composant audio lui mm n'est pas config
        // le joeur a desactivé le son dans les options
        if (clip == null || sfxSource == null || !soundEnabled) return;
        
        sfxSource.PlayOneShot(clip); // méthode Unity qui joue un son en une fois sans de chevauchement
    }

    public void SetSoundEnabled(bool value)
    {
        soundEnabled = value;
    }
}
