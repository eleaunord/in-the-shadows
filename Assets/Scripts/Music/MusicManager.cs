using UnityEngine;
using UnityEngine.Audio;
using System.Collections;

/*
AudioMixer = outil Unity mixer/modif son en temps réel
lowPassParam = "LowPassCutoff" = le nom (en texte) d'un réglage précis à l'intérieur de ce mixer. Un filtre "low pass" (passe-bas) est un effet audio qui coupe les sons aigus et ne laisse passer que les graves
Au lancement de la scène, le son est étouffé (800). Quand le joueur résout un puzzle, RemoveWindEffect() est appelée, ce qui lance une transition douce de 2 secondes qui fait remonter progressivement la fréquence jusqu'à 22000, donnant l'impression que la musique "se révèle" clairement — un petit effet sonore qui accompagne visuellement/auditivement la satisfaction d'avoir résolu le puzzle
*/
public class MusicManager : MonoBehaviour
{
    [Header("Audio")]
    public AudioMixer audioMixer;               // reference to the game's audio mixer
    public string lowPassParam = "LowPassCutoff"; // name of the exposed low pass parameter

    [Header("Settings")]
    public float windFrequency = 800f;          // low pass cutoff frequency with wind effect (muffled)
    public float clearFrequency = 22000f;       // low pass cutoff frequency without effect (clear)
    public float transitionDuration = 2f;       // duration of the transition in seconds


    // on applique l'effet dès le lancement ; 
    void Start() 
    {
        // Start with the wind/muffled effect applied
        // SetFloat(nom, valeur) = méthode native d'Unity qui dit : "va chercher le réglage qui s'appelle lowPassParam (donc "LowPassCutoff") dans le mixer, et donne-lui la valeur windFrequency".
        audioMixer.SetFloat(lowPassParam, windFrequency);
    }


    // Called when the puzzle is solved to smoothly remove the wind effect
    public void RemoveWindEffect()
    {
        StartCoroutine(TransitionFrequency(windFrequency, clearFrequency));
    }

    // Coroutine : smoothly transitions the low pass cutoff frequency from one value to another
    private IEnumerator TransitionFrequency(float from, float to)
    {
        float elapsed = 0f;

        while (elapsed < transitionDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / transitionDuration;

            // Smooth ease in-out curve for a natural sounding transition
            float smoothT = t * t * (3f - 2f * t);

            // Interpolate the cutoff frequency and apply it to the mixer
            float freq = Mathf.Lerp(from, to, smoothT);
            audioMixer.SetFloat(lowPassParam, freq);

            yield return null;
        }

        // Ensure the final frequency is set exactly at the end
        audioMixer.SetFloat(lowPassParam, to);
    }
}