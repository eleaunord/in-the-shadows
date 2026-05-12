using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class PuzzleManager : MonoBehaviour
{
    // Stores a target rotation and position the object must match to solve the puzzle
    [System.Serializable]
    public class TargetTransform
    {
        public Vector3 rotation;
        public Vector3 position;
    }

    [Header("Puzzle Setup")]
    public Transform objectToTrack;         // the object the player rotates
    public float toleranceDegrees = 15f;    // how close the rotation needs to be to validate

    [Header("UI")]
    public GameObject puzzleSolvedPanel;    // panel displayed when the puzzle is solved
    public TextMeshProUGUI solvedText;      // text component showing the solved message
    public string solvedMessage = "Puzzle Solved!"; // message displayed on solve
    public string mainMenuSceneName = "MainMenu";   // name of the main menu scene to load

    [Header("Cinematic")]
    public CameraZoom cameraZoom;           // handles the zoom cinematic on solve

    [Header("Events")]
    public UnityEvent onPuzzleSolved;       // Unity event triggered when puzzle is solved

    [Header("Audio")]
    public MusicManager musicManager;       // handles music transition on solve

    [Header("Targets")]
    public TargetTransform[] targets;       // list of valid target rotations/positions
    public float tolerancePosition = 0.5f; // how close the position needs to be to validate
    public float validationDelay = 1f;     // time the object must stay in position before solving

    private bool isSolved = false;          // prevents solving the puzzle multiple times
    private float validationTimer = 0f;    // tracks how long the object has been in the correct position

    void Update()
    {
        // Do nothing if already solved or no object to track
        if (isSolved) return;
        if (objectToTrack == null) return;

        if (IsTransformClose())
        {
            // Increment the timer while the object is in the correct position
            validationTimer += Time.deltaTime;

            // Solve the puzzle only after the object has been held in position long enough
            if (validationTimer >= validationDelay)
            {
                isSolved = true;
                ShowSolvedPanel();
                onPuzzleSolved.Invoke();
            }
        }
        else
        {
            // Reset the timer if the object leaves the correct position
            validationTimer = 0f;
        }
    }

    // Checks if the tracked object's rotation matches any of the target rotations
    private bool IsTransformClose()
    {
        foreach (TargetTransform target in targets)
        {
            // Level 1 : only check Y rotation (horizontal rotation only)
            float diffY = Mathf.Abs(Mathf.DeltaAngle(
                objectToTrack.localEulerAngles.y, target.rotation.y));

            bool rotationOk = diffY <= toleranceDegrees;

            if (rotationOk)
                return true;
        }
        return false;
    }

    // Triggers the cinematic then shows the solved panel
    private void ShowSolvedPanel()
    {
        StartCoroutine(PlayCinematicThenShow());
    }

    // Coroutine : plays the zoom cinematic then displays the puzzle solved panel
    private IEnumerator PlayCinematicThenShow()
    {
        // Start the music transition immediately when the puzzle is solved
        if (musicManager != null)
            musicManager.RemoveWindEffect();

        if (cameraZoom != null)
        {
            // Wait for the zoom cinematic to finish before showing the panel
            yield return StartCoroutine(cameraZoom.ZoomToShadow(() =>
            {
                // Show the solved panel once the zoom is complete
                if (puzzleSolvedPanel != null)
                {
                    puzzleSolvedPanel.SetActive(true);
                    if (solvedText != null)
                        solvedText.text = solvedMessage;
                }
            }));
        }
        else
        {
            // Fallback if no camera zoom is assigned : short delay then show panel
            yield return new WaitForSeconds(0.5f);
            if (puzzleSolvedPanel != null)
            {
                puzzleSolvedPanel.SetActive(true);
                if (solvedText != null)
                    solvedText.text = solvedMessage;
            }
        }
    }

    // Loads the main menu scene
    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }

    // Quits the application
    public void QuitGame()
    {
        Debug.Log("Quit!");
        Application.Quit();
    }

    // Editor utility : right click the script in the Inspector to capture the current transform as a target
    [ContextMenu("Capture Current Transform As Target")]
    public void CaptureTargetTransform()
    {
        if (objectToTrack != null)
        {
            // Add a new entry to the targets array with the current local rotation and position
            System.Array.Resize(ref targets, targets.Length + 1);
            targets[targets.Length - 1] = new TargetTransform
            {
                rotation = objectToTrack.localEulerAngles,
                position = objectToTrack.localPosition
            };
            Debug.Log("Captured local rotation: " + objectToTrack.localEulerAngles +
                      " | local position: " + objectToTrack.localPosition);
        }
    }
}