using UnityEngine;
using System.Text;

// Read-only debug utility for Level 3.
// Dumps every full-pose Solution stored on a PuzzleManager3 to the Console as
// clean, copy-pasteable text, so solutions can be shared without screenshots.
//
// Parts are identified by trackedParts[i].objectToTrack.name (there are no
// named pivot fields in PuzzleManager3 — Solution.partTargets[i] lines up
// with trackedParts[i] purely by list index), and each PartTarget always
// carries both a target rotation and a target position.
//
// Output format per solution:
//
//   === Solution 0 ===
//   PartName: rot(X, Y, Z) pos(X, Y, Z)
//   ...
//
// followed by a single-line summary (some Console entries truncate long
// multi-line logs on click, so the summary is a safe copy-paste fallback):
//
//   Solution 0 | PartName(rotX,rotY,rotZ) | ...
//
// Does not read or modify anything else in PuzzleManager3.
public class SolutionExporter3 : MonoBehaviour
{
    [Tooltip("PuzzleManager3 to read solutions from. Auto-found on this GameObject if left empty.")]
    public PuzzleManager3 puzzleManager;

    [ContextMenu("Export All Solutions To Console")]
    public void ExportAllSolutionsToConsole()
    {
        if (puzzleManager == null)
            puzzleManager = GetComponent<PuzzleManager3>();

        if (puzzleManager == null)
        {
            Debug.LogError("SolutionExporter3: no PuzzleManager3 assigned or found on this GameObject.");
            return;
        }

        if (puzzleManager.solutions.Count == 0)
        {
            Debug.Log("SolutionExporter3: PuzzleManager3 has no solutions to export.");
            return;
        }

        for (int s = 0; s < puzzleManager.solutions.Count; s++)
        {
            PuzzleManager3.Solution sol = puzzleManager.solutions[s];

            StringBuilder block = new StringBuilder();
            StringBuilder summary = new StringBuilder();

            block.AppendLine($"=== Solution {s} ===");
            summary.Append($"Solution {s}");

            for (int i = 0; i < sol.partTargets.Count; i++)
            {
                string partName = "<unassigned>";
                if (i < puzzleManager.trackedParts.Count &&
                    puzzleManager.trackedParts[i].objectToTrack != null)
                {
                    partName = puzzleManager.trackedParts[i].objectToTrack.name;
                }

                PuzzleManager3.PartTarget target = sol.partTargets[i];
                Vector3 r = target.targetRotation;
                Vector3 p = target.targetPosition;

                block.AppendLine(
                    $"{partName}: rot({r.x:F3}, {r.y:F3}, {r.z:F3}) pos({p.x:F3}, {p.y:F3}, {p.z:F3})");

                summary.Append($" | {partName}({r.x:F3},{r.y:F3},{r.z:F3})");
            }

            Debug.Log(block.ToString());
            Debug.Log(summary.ToString());
        }
    }
}
