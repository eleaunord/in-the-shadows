using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class PuzzleButtonUI : MonoBehaviour
{
    // dans l'inspecteur
    [SerializeField] private Button button;
    [SerializeField] private TMP_Text label;
    [SerializeField] private GameObject lockIcon;
    [SerializeField] private GameObject solvedCheckmark;
    [SerializeField] private Image grayOverlay; // optionnel : null sur PuzzleButton_1 (toujours débloqué)

    [Header("Config")]
    [SerializeField] private string sceneName;
    [SerializeField] private string puzzleClueName;

    [Header("Animation")]
    [SerializeField] private float unlockFadeDuration = 2f;
    [SerializeField] private int highlightPulseCount = 3;
    [SerializeField] private float highlightPulseInterval = 3f;

    public void Setup(int puzzleIndex, bool unlocked, bool solved, bool justUnlocked = false)
    {
        label.text = puzzleClueName;

        if (lockIcon == null || solvedCheckmark == null)
        {
            Debug.LogError($"PuzzleButtonUI '{name}' (puzzle {puzzleIndex}): lockIcon or solvedCheckmark is not assigned in the Inspector.", this);
            return;
        }

        solvedCheckmark.SetActive(solved); // affiche check si puzzle résolut

        if (justUnlocked && unlocked && grayOverlay != null)
        {
            StartCoroutine(PlayUnlockFadeAnimation());
        }
        else
        {
            // Etat final instantané : puzzle déjà débloqué depuis avant, ou puzzle 1 (toujours débloqué)
            button.interactable = unlocked; // grise/non cliquable ou pas
            lockIcon.SetActive(!unlocked); // affiche cadenas si puzzle non débloqué
            SetGrayOverlayAlpha(unlocked ? 0f : 1f);
        }
    }

    // co routine d'animation
    // Fondu du cadenas + de l'overlay gris quand ce puzzle vient d'être débloqué
    private IEnumerator PlayUnlockFadeAnimation()
    {
        button.interactable = false; // bloque les clics pendant l'animation
        lockIcon.SetActive(true);

        Image lockImage = lockIcon.GetComponent<Image>();
        Color lockColor = lockImage != null ? lockImage.color : Color.white;
        float elapsed = 0f;

        while (elapsed < unlockFadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / unlockFadeDuration);
            SetGrayOverlayAlpha(Mathf.Lerp(1f, 0f, t)); // progression de l'animation de 0 à 1 par t%
            if (lockImage != null)
            {
                lockColor.a = Mathf.Lerp(1f, 0f, t);
                lockImage.color = lockColor;
            }
            yield return null;
        }

        SetGrayOverlayAlpha(0f);
        lockIcon.SetActive(false);
        button.interactable = true;

        // Attire l'œil sur le puzzle qui vient d'être débloqué : flash Normal/Hover
        yield return PlayHighlightPulse();
    }

    // Fait clignoter le fond du bouton entre son sprite Normal et Highlighted,
    // pour attirer l'œil juste après le fondu de déblocage. Se termine sur Normal
    // pour laisser le Sprite Swap natif du Button reprendre la main normalement.
    private IEnumerator PlayHighlightPulse()
    {
        Image backgroundImage = button.targetGraphic as Image;
        Sprite highlightedSprite = button.spriteState.highlightedSprite;
        if (backgroundImage == null || highlightedSprite == null) yield break;

        Sprite normalSprite = backgroundImage.sprite;

        for (int i = 0; i < highlightPulseCount; i++)
        {
            backgroundImage.sprite = highlightedSprite;
            yield return new WaitForSeconds(highlightPulseInterval);
            backgroundImage.sprite = normalSprite;
            yield return new WaitForSeconds(highlightPulseInterval);
        }
    }

    // pour changer transparence du calque gris sans toucher à la couleur
    private void SetGrayOverlayAlpha(float a)
    {
        if (grayOverlay == null) return;
        Color c = grayOverlay.color;
        c.a = a;
        grayOverlay.color = c;
    }

    // event OnClick du composant Button dans inspector
    public void OnClicked()
    {
        SceneManager.LoadScene(sceneName);
    }
}