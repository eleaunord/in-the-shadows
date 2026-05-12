using UnityEngine;
using System.Collections;

public class CameraZoom : MonoBehaviour
{
    [Header("Zoom Settings")]
    public Transform shadowTarget;              // target the camera zooms toward
    public float zoomDuration = 2f;             // zoom duration in seconds
    public float zoomFOV = 20f;                 // final FOV (smaller = more zoomed in)
    public Vector3 zoomOffset = new Vector3(0, 0, -3f); // final camera position offset

    private Camera cam;
    private Vector3 originalPosition;           // camera's original position
    private Quaternion originalRotation;        // camera's original rotation
    private float originalFOV;                  // camera's original FOV

    [Header("Letterbox")]
    public Letterbox letterbox;                 // reference to the black bars script

    void Start()
    {
        // Save the camera's initial state to restore it later
        cam = GetComponent<Camera>();
        originalPosition = transform.position;
        originalRotation = transform.rotation;
        originalFOV = cam.fieldOfView;
    }

    // Main coroutine : zoom toward the shadow then show the solved panel
    public IEnumerator ZoomToShadow(System.Action onComplete)
    {
        float elapsed = 0f;

        // Calculate the final camera position in front of the target
        Vector3 targetPosition = shadowTarget.position + zoomOffset;

        // Calculate the rotation so the camera looks toward the target
        Quaternion targetRotation = Quaternion.LookRotation(
            shadowTarget.position - targetPosition);

        // Trigger the letterbox black bars animation at the start of the zoom
        if (letterbox != null)
            letterbox.StartCoroutine(letterbox.ShowBars());

        // Animate the camera toward the target over zoomDuration seconds
        while (elapsed < zoomDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / zoomDuration;

            // Smooth ease in-out curve for fluid movement
            float smoothT = t * t * (3f - 2f * t);

            // Interpolate position, rotation and FOV between initial and final state
            transform.position = Vector3.Lerp(originalPosition, targetPosition, smoothT);
            transform.rotation = Quaternion.Lerp(originalRotation, targetRotation, smoothT);
            cam.fieldOfView = Mathf.Lerp(originalFOV, zoomFOV, smoothT);

            yield return null;
        }

        // Short pause to let the player admire the shadow
        yield return new WaitForSeconds(0.5f);

        // Hide the black bars before showing the victory panel
        if (letterbox != null)
            yield return StartCoroutine(letterbox.HideBars());

        // Invoke the callback → triggers the PuzzleSolvedPanel display
        onComplete?.Invoke();
    }

    // Coroutine to smoothly return the camera to its original state
    public IEnumerator ZoomBack()
    {
        float elapsed = 0f;

        // Save the current state before zooming back
        Vector3 currentPos = transform.position;
        Quaternion currentRot = transform.rotation;
        float currentFOV = cam.fieldOfView;

        // Animate the return in half the time of the forward zoom
        while (elapsed < zoomDuration * 0.5f)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / (zoomDuration * 0.5f);

            // Smooth ease in-out curve for fluid return
            float smoothT = t * t * (3f - 2f * t);

            // Interpolate back to original position, rotation and FOV
            transform.position = Vector3.Lerp(currentPos, originalPosition, smoothT);
            transform.rotation = Quaternion.Lerp(currentRot, originalRotation, smoothT);
            cam.fieldOfView = Mathf.Lerp(currentFOV, originalFOV, smoothT);

            yield return null;
        }
    }
}