using UnityEngine;
using UnityEngine.Audio;
using System.Collections;

public class MusicManager : MonoBehaviour
{
    [Header("Audio")]
    public AudioMixer audioMixer;               // reference to the game's audio mixer
    public string lowPassParam = "LowPassCutoff"; // name of the exposed low pass parameter

    [Header("Settings")]
    public float windFrequency = 800f;          // low pass cutoff frequency with wind effect (muffled)
    public float clearFrequency = 22000f;       // low pass cutoff frequency without effect (clear)
    public float transitionDuration = 2f;       // duration of the transition in seconds

    void Start()
    {
        // Start with the wind/muffled effect applied
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