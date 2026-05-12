using UnityEngine;
using System.Collections;

public class Letterbox : MonoBehaviour
{
    public GameObject topBar;           // top black bar GameObject
    public GameObject bottomBar;        // bottom black bar GameObject
    public float animDuration = 0.8f;   // animation duration in seconds

    void Start()
    {
        // Hide both bars at the start of the scene
        if (topBar != null) topBar.SetActive(false);
        if (bottomBar != null) bottomBar.SetActive(false);
    }

    // Coroutine : animates the bars sliding in from top and bottom
    public IEnumerator ShowBars()
    {
        // Activate both bars before animating them
        if (topBar != null) topBar.SetActive(true);
        if (bottomBar != null) bottomBar.SetActive(true);

        float elapsed = 0f;
        RectTransform top = topBar.GetComponent<RectTransform>();
        RectTransform bot = bottomBar.GetComponent<RectTransform>();

        // Animate scale from 0 to 1 on Y axis to slide bars in
        while (elapsed < animDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / animDuration);
            float scale = Mathf.Lerp(0f, 1f, t);
            top.localScale = new Vector3(1, scale, 1);
            bot.localScale = new Vector3(1, scale, 1);
            yield return null;
        }

        // Ensure bars are fully visible at the end
        top.localScale = Vector3.one;
        bot.localScale = Vector3.one;
    }

    // Coroutine : animates the bars sliding out then hides them
    public IEnumerator HideBars()
    {
        RectTransform top = topBar.GetComponent<RectTransform>();
        RectTransform bot = bottomBar.GetComponent<RectTransform>();

        float elapsed = 0f;

        // Animate scale from 1 to 0 on Y axis to slide bars out
        while (elapsed < animDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / animDuration);
            float scale = Mathf.Lerp(1f, 0f, t);
            top.localScale = new Vector3(1, scale, 1);
            bot.localScale = new Vector3(1, scale, 1);
            yield return null;
        }

        // Deactivate both bars once animation is complete
        if (topBar != null) topBar.SetActive(false);
        if (bottomBar != null) bottomBar.SetActive(false);
    }
}