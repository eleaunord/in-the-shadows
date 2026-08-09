using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class PuzzleButtonUI : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private TMP_Text label;
    [SerializeField] private GameObject lockIcon;
    [SerializeField] private GameObject solvedCheckmark;

    [Header("Config")]
    [SerializeField] private string sceneName;       // exact scene name to load
    [SerializeField] private string puzzleClueName;   // e.g. "Meow"

    public void Setup(int puzzleIndex, bool unlocked, bool solved)
    {
        label.text = puzzleClueName;
        button.interactable = unlocked;

        if (lockIcon == null || solvedCheckmark == null)
        {
            Debug.LogError($"PuzzleButtonUI '{name}' (puzzle {puzzleIndex}): lockIcon or solvedCheckmark is not assigned in the Inspector.", this);
            return;
        }

        lockIcon.SetActive(!unlocked);
        solvedCheckmark.SetActive(solved);
    }

    public void OnClicked()
    {
        SceneManager.LoadScene(sceneName);
    }
}