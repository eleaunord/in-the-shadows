using UnityEngine;

/// <summary>
/// Persistent singleton tracking puzzle progress and the current game mode.
/// Survives scene loads via DontDestroyOnLoad.
/// </summary>
public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }
    public bool SkipToLevelSelect { get; set; } = false;
    private const int TOTAL_PUZZLES = 3;
    private const string SOLVED_KEY_PREFIX = "Puzzle_Solved_"; // + index

    public bool IsTestMode { get; private set; } = false;

    [ContextMenu("Reset Save Data")]
    public void ResetSaveData()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        Debug.Log("Save data reset.");
    }

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

    public void SetTestMode(bool value)
    {
        IsTestMode = value;
    }

    public bool IsPuzzleSolved(int puzzleIndex)
    {
        bool result = PlayerPrefs.GetInt(SOLVED_KEY_PREFIX + puzzleIndex, 0) == 1;
        Debug.Log($"[SaveManager] IsPuzzleSolved({puzzleIndex}) = {result}");
        return result;
    }

    public void MarkPuzzleSolved(int puzzleIndex)
    {
        Debug.Log($"[SaveManager] MarkPuzzleSolved({puzzleIndex}) appelé");
        PlayerPrefs.SetInt(SOLVED_KEY_PREFIX + puzzleIndex, 1);
        PlayerPrefs.Save();
    }
    public bool IsPuzzleUnlocked(int puzzleIndex)
    {
        if (IsTestMode) return true;
        if (puzzleIndex == 1) return true; // first puzzle always unlocked
        return IsPuzzleSolved(puzzleIndex - 1);
    }

    // Handy during development to re-test the progression from scratch
    [ContextMenu("Reset All Progress")]
    public void ResetProgress()
    {
        for (int i = 1; i <= TOTAL_PUZZLES; i++)
        {
            PlayerPrefs.DeleteKey(SOLVED_KEY_PREFIX + i);
        }
        PlayerPrefs.Save();
    }

}