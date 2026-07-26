using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class PuzzleManager3 : MonoBehaviour
{
    // Target transform for one part within a solution
    [System.Serializable]
    public class PartTarget
    {
        public Vector3 targetRotation;  // target local euler angles for this part
        public Vector3 targetPosition;  // target local position for this part
    }

    // A complete set of target transforms covering all tracked parts at once
    [System.Serializable]
    public class Solution
    {
        public List<PartTarget> partTargets = new List<PartTarget>(); // one entry per trackedPart, same order
    }

    // Holds a reference to each movable part; solutions reference parts by index
    [System.Serializable]
    public class TrackedPart
    {
        public Transform objectToTrack;  // the object the player manipulates
    }

    [Header("Puzzle Setup")]
    public List<TrackedPart> trackedParts = new List<TrackedPart>(); // master list of all parts
    public List<Solution> solutions = new List<Solution>();           // all valid full-pose solutions
    public float toleranceDegrees = 15f;   // how close each rotation needs to be (per axis)
    public float tolerancePosition = 0.5f; // how close each position needs to be
    public float validationDelay = 1f;     // seconds ALL parts must stay on the same solution before solving

    [Header("UI")]
    public GameObject puzzleSolvedPanel;            // panel displayed when the puzzle is solved
    public TextMeshProUGUI solvedText;              // text component showing the solved message
    public string solvedMessage = "Puzzle Solved!"; // message displayed on solve
    public string mainMenuSceneName = "MainMenu";   // name of the main menu scene to load

    [Header("Cinematic")]
    public CameraZoom cameraZoom;          // handles the zoom cinematic on solve

    [Header("Events")]
    public UnityEvent onPuzzleSolved;      // Unity event triggered when puzzle is solved

    [Header("Audio")]
    public MusicManager musicManager;      // handles music transition on solve

    private bool isSolved = false;              // prevents solving the puzzle multiple times
    private float validationTimer = 0f;        // tracks how long ALL parts have held the same solution
    private int lastMatchedSolutionIndex = -1; // solution index matched on the previous frame

    void Update()
    {
        if (isSolved) return;

        int matched = FindMatchingSolutionIndex();

        // LOG TEMPORAIRE — which solution is currently aligned (or none)
        Debug.Log("Currently matching solution: " + (matched >= 0 ? matched.ToString() : "none"));

        if (matched >= 0)
        {
            // LOG TEMPORAIRE — per-part detail for the matched solution
            LogSolutionMatch(matched);

            if (matched != lastMatchedSolutionIndex)
            {
                // A different solution started matching: reset the timer before counting this frame
                validationTimer = 0f;
                lastMatchedSolutionIndex = matched;
            }

            // Increment the timer while every part holds the same solution
            validationTimer += Time.deltaTime;

            // Solve the puzzle only after all parts have been held in position long enough
            if (validationTimer >= validationDelay)
            {
                isSolved = true;
                DisableAllRotators();
                ShowSolvedPanel();
                onPuzzleSolved.Invoke();
            }
        }
        else
        {
            // Reset the timer as soon as no solution is fully matched
            validationTimer = 0f;
            lastMatchedSolutionIndex = -1;
        }
    }

    // Returns the index of the first solution where every part simultaneously matches, or -1
    private int FindMatchingSolutionIndex()
    {
        if (trackedParts.Count == 0 || solutions.Count == 0) return -1;

        for (int s = 0; s < solutions.Count; s++)
        {
            Solution sol = solutions[s];

            // Skip solutions that don't cover all tracked parts
            if (sol.partTargets.Count != trackedParts.Count) continue;

            bool allMatch = true;
            for (int i = 0; i < trackedParts.Count; i++)
            {
                if (trackedParts[i].objectToTrack == null) { allMatch = false; break; }
                if (!IsPartAlignedToTarget(trackedParts[i].objectToTrack, sol.partTargets[i]))
                {
                    allMatch = false;
                    break; // stop checking this solution as soon as one part fails
                }
            }

            if (allMatch) return s;
        }

        return -1;
    }

    // Checks all 3 rotation axes and position distance for one part against one target
    private bool IsPartAlignedToTarget(Transform obj, PartTarget target)
    {
        Vector3 current = obj.localEulerAngles;

        float diffX = Mathf.Abs(Mathf.DeltaAngle(current.x, target.targetRotation.x));
        float diffY = Mathf.Abs(Mathf.DeltaAngle(current.y, target.targetRotation.y));
        float diffZ = Mathf.Abs(Mathf.DeltaAngle(current.z, target.targetRotation.z));

        bool rotationOk = diffX <= toleranceDegrees &&
                          diffY <= toleranceDegrees &&
                          diffZ <= toleranceDegrees;

        float distPos = Vector3.Distance(obj.localPosition, target.targetPosition);
        bool positionOk = distPos <= tolerancePosition;

        return rotationOk && positionOk;
    }

    // LOG TEMPORAIRE — logs per-part detail for a matched solution
    private void LogSolutionMatch(int solutionIndex)
    {
        Solution sol = solutions[solutionIndex];
        for (int i = 0; i < trackedParts.Count; i++)
        {
            Transform obj = trackedParts[i].objectToTrack;
            if (obj == null) continue;

            PartTarget target = sol.partTargets[i];
            Vector3 current = obj.localEulerAngles;

            float diffX = Mathf.Abs(Mathf.DeltaAngle(current.x, target.targetRotation.x));
            float diffY = Mathf.Abs(Mathf.DeltaAngle(current.y, target.targetRotation.y));
            float diffZ = Mathf.Abs(Mathf.DeltaAngle(current.z, target.targetRotation.z));

            bool rotationOk = diffX <= toleranceDegrees &&
                              diffY <= toleranceDegrees &&
                              diffZ <= toleranceDegrees;

            float distPos = Vector3.Distance(obj.localPosition, target.targetPosition);
            bool positionOk = distPos <= tolerancePosition;

            Debug.Log("[Sol " + solutionIndex + "] " + obj.name +
                      " | rotDiff: " + diffX.ToString("F1") + "/" + diffY.ToString("F1") + "/" + diffZ.ToString("F1") +
                      " | posDist: " + distPos.ToString("F2") +
                      " | rotOK: " + rotationOk + " | posOK: " + positionOk);
        }
    }

    // Disables ObjectRotator3 on each part (checks pivot parents) so the player can no longer move them after solving
    private void DisableAllRotators()
    {
        foreach (TrackedPart part in trackedParts)
        {
            if (part.objectToTrack == null) continue;
            ObjectRotator3 rotator = part.objectToTrack.GetComponentInParent<ObjectRotator3>();
            if (rotator != null)
                rotator.enabled = false;
        }
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
        musicManager?.RemoveWindEffect();

        if (cameraZoom != null)
        {
            // Wait for the zoom cinematic to finish before showing the panel
            yield return StartCoroutine(cameraZoom.ZoomToShadow(() =>
            {
                // Show the solved panel once the zoom is complete
                if (puzzleSolvedPanel != null)
                {
                    puzzleSolvedPanel.SetActive(true);
                    solvedText?.SetText(solvedMessage);
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
                solvedText?.SetText(solvedMessage);
            }
        }
    }

    // Loads the main menu scene
    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SaveManager.Instance.MarkPuzzleSolved(3);
        SaveManager.Instance.SkipToLevelSelect = true;
        SceneManager.LoadScene("MainMenu");
    }

    // Quits the application
    public void QuitGame()
    {
        Debug.Log("Quit!");
        Application.Quit();
    }

    // Editor utility : right-click the script in the Inspector to snapshot all parts' current transforms and APPEND them as a new solution
    [ContextMenu("Capture Current Pose As New Solution")]
    public void CaptureCurrentPoseAsNewSolution()
    {
        // Require all parts to be assigned before capturing
        foreach (TrackedPart part in trackedParts)
        {
            if (part.objectToTrack == null)
            {
                Debug.LogError("PuzzleManager3: a TrackedPart has no objectToTrack assigned. Assign all parts before capturing a solution.");
                return;
            }
        }

        Solution newSolution = new Solution();

        foreach (TrackedPart part in trackedParts)
        {
            PartTarget pt = new PartTarget
            {
                targetRotation = part.objectToTrack.localEulerAngles,
                targetPosition = part.objectToTrack.localPosition
            };

            newSolution.partTargets.Add(pt);

            Debug.Log("[" + part.objectToTrack.name + "] Captured rotation: " +
                      pt.targetRotation + " | position: " + pt.targetPosition);
        }

        solutions.Add(newSolution);
        Debug.Log("Solution " + (solutions.Count - 1) + " captured (" + newSolution.partTargets.Count + " parts).");
    }
}
