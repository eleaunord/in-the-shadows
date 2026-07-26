using UnityEngine;

public class MenuManager : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject titlePanel;
    [SerializeField] private GameObject levelSelectPanel;

    [Header("Puzzle buttons (index 0 = Puzzle 1)")]
    [SerializeField] private PuzzleButtonUI[] puzzleButtons;

    private void OnEnable()
    {
        if (levelSelectPanel != null && levelSelectPanel.activeSelf)
        {
            RefreshPuzzleButtons();
        }
    }

    private void Start()
    {

        if (SaveManager.Instance.SkipToLevelSelect)
        {
            SaveManager.Instance.SkipToLevelSelect = false; // reset pour la prochaine fois
            ShowLevelSelectPanel();
        }
        else
        {
            ShowTitlePanel();
        }
    }
    public void OnNormalModeClicked()
    {
        SaveManager.Instance.SetTestMode(false);
        ShowLevelSelectPanel();
    }

    public void OnTestModeClicked()
    {
        SaveManager.Instance.SetTestMode(true);
        ShowLevelSelectPanel();
    }

    public void OnBackToTitleClicked()
    {
        SaveManager.Instance.SetTestMode(false);
        ShowTitlePanel();
    }

    public void OnQuitClicked()
    {
        Application.Quit();
    }

    private void ShowTitlePanel()
    {
        titlePanel.SetActive(true);
        levelSelectPanel.SetActive(false);
    }

    private void ShowLevelSelectPanel()
    {
        titlePanel.SetActive(false);
        levelSelectPanel.SetActive(true);
        RefreshPuzzleButtons();
    }

    private void RefreshPuzzleButtons()
    {
        bool testMode = SaveManager.Instance.IsTestMode;

        for (int i = 0; i < puzzleButtons.Length; i++)
        {
            int puzzleIndex = i + 1; // puzzles are 1-indexed
            bool unlocked;
            bool solved;

            if (testMode)
            {
                // Test Mode: display every level as unlocked/completed for
                // evaluation purposes, without touching real progress data.
                unlocked = true;
                solved = true;
            }
            else
            {
                unlocked = SaveManager.Instance.IsPuzzleUnlocked(puzzleIndex);
                solved = SaveManager.Instance.IsPuzzleSolved(puzzleIndex);
            }

            puzzleButtons[i].Setup(puzzleIndex, unlocked, solved);
        }
    }
}