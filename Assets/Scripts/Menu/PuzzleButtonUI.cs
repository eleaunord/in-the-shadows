using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

/*
Corountine = méthode qui peut s'étaler sur plusieurs frames au lieu de s'éxecuter directement
type de retour => IEnumerator
yield return null; => pause ici, on reprends à la prochaine frame
yiel return new waiForSeconds(x); => pause ici on reprend dans x secs
debut => StartCoroutine(Mamethode()) 

private IEnumerator FunctionName()
    {

        float elapsed = 0f;

        while (elapsed < timerEnds)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / timerEnds);
            toDoBeforeItEnds(Mathf.Lerp(1f, 0f, t)); // progression de l'animation de 0 à 1 par t%
            yield return null;
        }

        if (pblm) yield break;

        yield return null;
    }
*/
public class PuzzleButtonUI : MonoBehaviour
{
    // dans l'inspecteur
    [SerializeField] private Button button;
    [SerializeField] private TMP_Text label;
    [SerializeField] private GameObject lockIcon;
    [SerializeField] private GameObject solvedCheckmark;
    [SerializeField] private Image grayOverlay; //null sur PuzzleButton_1 (toujours débloqué)

    [Header("Config")]
    [SerializeField] private string sceneName;
    [SerializeField] private string puzzleClueName;

    [Header("Animation")]
    [SerializeField] private float unlockFadeDuration = 0.5f;
    [SerializeField] private int highlightPulseCount = 2;
    [SerializeField] private float highlightPulseInterval = 0.25f;


    // Setup recoit un etat depuis MenuManager sans savoir pourquoi cet etat est ce qu'il est!
    // si etat nouveau (justUnlocked) => déclenche une sequence de 2 coroutine (animation)
    public void Setup(int puzzleIndex, bool unlocked, bool solved, bool justUnlocked = false)
    {
        label.text = puzzleClueName;

        if (lockIcon == null || solvedCheckmark == null)
        {
            // $ permet d'inserer direct name & co sans avoir besoind d'un + 
            // this = contecte, quand on clique sur le mess d'erreur dans la console ca selectionne direct le bon gameobject dans la hiérarchie
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


        // GetComponent<Image>() => récupere le composant image attaché au lockIcon
        Image lockImage = lockIcon.GetComponent<Image>();
        Color lockColor = lockImage != null ? lockImage.color : Color.white; // si il n'y a pas de composant lockIcon alors on met du blanc
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

        // Attire l'œil sur le puzzle qui vient d'être débloqué : flash Normal/Hover => attend que cette routine playhighpulse se termine complemetement avant de termine ta coroutine 
        yield return PlayHighlightPulse();
    }

    // Fait clignoter le fond du bouton entre son sprite Normal et Highlighted,
    // pour attirer l'œil juste après le fondu de déblocage. Se termine sur Normal
    // pour laisser le Sprite Swap natif du Button reprendre la main normalement.
    private IEnumerator PlayHighlightPulse()
    {
        SFXManager.Instance.PlaySFX(SFXManager.Instance.unlockClip);

        // as cast sur, on convertit graphic (classe base pr ui) en image, si ca ne marche pas ca renvoie null direct
        Image backgroundImage = button.targetGraphic as Image;

        // on cherche direct le sprite hover (highlighted) dans spriteState, un espace ou Unity stock chaque etats des boutons
        Sprite highlightedSprite = button.spriteState.highlightedSprite;

        if (backgroundImage == null || highlightedSprite == null) yield break; // pblm avec l'image = on arrete immediatement la couroutine

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
    // color est un struct(type valeur)
    private void SetGrayOverlayAlpha(float a)
    {
        if (grayOverlay == null) return;
        Color c = grayOverlay.color; // renvoie copie de la couleur pas une ref à l'originale
        c.a = a; // on modifie la copie 
        grayOverlay.color = c; // on reassigne la couleur complete pr que ca prenne effet
    }

    // event OnClick du composant Button dans inspector
    public void OnClicked()
    {
        SceneManager.LoadScene(sceneName);
    }
}