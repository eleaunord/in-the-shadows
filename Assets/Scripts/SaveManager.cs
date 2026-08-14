using UnityEngine;

/*
Progression des puzzles (résolu ou non), stockée de facon persistante avec PlayerPrefs (systeme de sauvegarde simple d'unity)
PlayerPrefs = API native d'Unity pour sauvegarder de petites données clé-valeur sur le disque, persistantes même après fermeture du jeu
Mode test vs normal : istesmode gardé en mémoire
déblocage progressif
*/
public class SaveManager : MonoBehaviour
{
    private static SaveManager _instance;

    // s'éxecute à chaque fois qu'on lit dans SaveManager.Instance
    public static SaveManager Instance // référence statique globale : permet d'écrire instance depuis n'importe quel script 
    {
        get
        {
            // pas besoin d'avoir un save manager dans chaque scene 
            if (_instance == null) // personne n'a encore créé de SaveManager
            {
                GameObject go = new GameObject("SaveManager"); // création d'un gameobject vide 'SaveManager'
                _instance = go.AddComponent<SaveManager>(); // morceau de comportement qu'on attache à un gameobject ; attache le script savemanager
                DontDestroyOnLoad(go); // le rend persistent
            }
            return _instance;
        }
    }

    public bool SkipToLevelSelect { get; set; } = false;
    /*
    private bool _skipToLevelSelect = false; 
    public bool SkipToLevelSelect
    {
        get { return _skipToLevelSelect; } // lit le champs 
        set { _skipToLevelSelect = value; } // écrit
    }
    */
    private const int TOTAL_PUZZLES = 3;
    private const string SOLVED_KEY_PREFIX = "Puzzle_Solved_"; // + index

    // Transient (non persisté) : puzzle qui vient d'être débloqué cette session,
    // consommé une seule fois par MenuManager pour jouer l'animation de déblocage.
    private int? _pendingUnlockedPuzzleIndex = null;

    public bool IsTestMode { get; private set; } = false; // n'importe qu'elle script pt lire mais seul du code à l'interieur de la classe pt modif

    // attribut : ajoute une entrée cliquable dans le menu : du composant inspector
    [ContextMenu("Reset Save Data")]
    public void ResetSaveData()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        Debug.Log("Save data reset.");
    }

    // sécurité complementaire pour que instance soit persistent
    private void Awake()
    {
        if (_instance != null && _instance != this) // evite doublon
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SetTestMode(bool value)
    {
        IsTestMode = value;
    }

    public bool IsPuzzleSolved(int puzzleIndex)
    {
        bool result = PlayerPrefs.GetInt(SOLVED_KEY_PREFIX + puzzleIndex, 0) == 1;
        //Debug.Log($"[SaveManager] IsPuzzleSolved({puzzleIndex}) = {result}");
        // Debug.Log("[SaveManager] IsPuzzleSolved(" + puzzleIndex + ") = " + result);
        return result;
    }

    public void MarkPuzzleSolved(int puzzleIndex)
    {
        //Debug.Log($"[SaveManager] MarkPuzzleSolved({puzzleIndex}) appelé");
        //PlayerPrefs.SetInt(clé, valeur)
        PlayerPrefs.SetInt(SOLVED_KEY_PREFIX + puzzleIndex, 1);
        PlayerPrefs.Save();

        int nextIndex = puzzleIndex + 1;
        if (nextIndex <= TOTAL_PUZZLES)
        {
            _pendingUnlockedPuzzleIndex = nextIndex;
        }
    }

    // Consommé une seule fois par MenuManager.RefreshPuzzleButtons() : renvoie le puzzle
    // fraîchement débloqué (ou null) et remet le flag à null pour ne pas rejouer l'animation.
    public int? ConsumeNewlyUnlockedPuzzle()
    {
        int? p = _pendingUnlockedPuzzleIndex;
        _pendingUnlockedPuzzleIndex = null;
        return p;
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