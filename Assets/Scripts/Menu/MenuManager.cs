using UnityEngine;

/*
MenuManager.cs gère navigation entre écran titre et écran de sélection niveau + Normal/Test mode
On a 2 panels gérés en show/hide : titlePanel + levelSelectPanel
tableau puzzleButtons 
cycle de vie : start() on décide de quel panel on affiche au lancement

MonoBehavior => permet à Unity d'executer le script
-> permet d'accéder à des méthodes appeles automatiquement par Unity comme start()
-> propriétés comme enable, gameObject

SaveManager.instance = pattern singleton => existe partout sans avoir besoin d'une référence explicite, systeme globale de sauvegarde
*/

public class MenuManager : MonoBehaviour
{
    // [Header(...)] et [SerializeField] => attributs, donnent des instructions à unity pas au compilateur c# lui mm
    [Header("Panels")]

    // private pr les autres scripts mais on pt l'afficher dans l'editeur unity
    [SerializeField] private GameObject titlePanel;
    [SerializeField] private GameObject levelSelectPanel;

    [Header("Puzzle buttons (index 0 = Puzzle 1)")]
    [SerializeField] private PuzzleButtonUI[] puzzleButtons; // tableau pour les boutons

    private bool _hasStarted = false; // _ pour variable private 


    // Unity appelle automatiquement OnEnable() et Start(), méthodes héritées de MonoBehaviour

    // OnEnable() : à chaque fois qu'un GameObject devient actif, avant Start()
    private void OnEnable()
    {
        // Ignore le tout premier OnEnable() du chargement de scène : Start() s'en
        // charge juste après et c'est lui qui doit consommer le flag "justUnlocked".
        if (_hasStarted && levelSelectPanel != null && levelSelectPanel.activeSelf) // on a deja demarre, le panel exsite et il est actuellement affiche 
        {
            RefreshPuzzleButtons(); // update les bouttons une fois qu'on a demarré 
        }
    }

    // appel une seul fois, ap OnEnable()
    private void Start()
    {
        _hasStarted = true;

        // quand on revient sur la scene menu : est ce qu'on doit revoir l'écran titre ou aller direct à la selection des puzzles?
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

    // branche sur inspector, le bouton connait la méthode
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
        titlePanel.SetActive(true); // le gameobject (et ses enfants) redevient actif, s'affiche à l'écran, ses scripts recoivent à nouveau Update()
        levelSelectPanel.SetActive(false); // game object des
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

        // Consommé une seule fois par refresh : le puzzle qui vient d'être débloqué cette session (ou null si aucun / déjà consommé / mode test).

        // nullable value type : un int ne peut jamais être nulle => ? = cet int pt contenir soit un nb soit r du tt
        // condition ? valeurSiVrai : valeurSiFaux <=> si on est en mode test, pas de flag "just unlocked", on ne touche pas à ConsumeNewlyUnlockedPuzzle()

        int? justUnlockedIndex = testMode ? null : SaveManager.Instance.ConsumeNewlyUnlockedPuzzle();

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

            bool justUnlocked = justUnlockedIndex.HasValue && justUnlockedIndex.Value == puzzleIndex; // bouton qui vient juste d'etre débloqué vrai /fauxet son index est puzzle index
            puzzleButtons[i].Setup(puzzleIndex, unlocked, solved, justUnlocked); // puzzlebuttonui.cs
        }
    }
}