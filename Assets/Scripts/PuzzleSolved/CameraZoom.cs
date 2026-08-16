using UnityEngine;
using System.Collections;

/*
Coroutine & IEnumerator = methode c# qui peut faire une pause
En C# quand on appelle un méthode elle s'execute dans l'instant
Pour faire un zoom qui dure 2 sec dans une methode normale = pas possible comme tout se passerait dans l'instant
Coroutine = méthode spéciale qui peut dire stop je fais une pause ici et je reprendrai à ce meme endroit à la prochaine frame
IEnumerator = étiquette qui dit à Unity cette méthode va faire des pauses
// lire un livre d'un coup ou en prenant des pauses

Lerp
Lerp(0, 10, 0) → te donne 0 (tu es encore au départ, 0%)
Lerp(0, 10, 0.5) → te donne 5 (tu es à mi-chemin, 50%)
Lerp(0, 10, 1) → te donne 10 (tu es arrivé, 100%)

*/

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

        // Calculate the rotation so the camera looks toward the target, géré par Unity directement
        Quaternion targetRotation = Quaternion.LookRotation(
            shadowTarget.position - targetPosition);

        // Trigger the letterbox black bars animation at the start of the zoom
        if (letterbox != null)
            letterbox.StartCoroutine(letterbox.ShowBars()); // lance l'animation mais continue tt de suite sans attendre qu'elle finisse // en mm temps

        // Animate the camera toward the target over zoomDuration seconds
        while (elapsed < zoomDuration)
        {
            // Time.deltaTime = time since the last frame, permet à elapsed d'avancer à la meme vitesse reelle 
            // evite que le zoom soit plus lent ou non selon le pc
            elapsed += Time.deltaTime;
            float t = elapsed / zoomDuration; // flag pr savoir ou on en est

            // Smooth ease in-out curve for fluid movement sinon mvvmnt robot
            float smoothT = t * t * (3f - 2f * t);

            // Interpolate position, rotation and FOV between initial and final state
            // Lerp = calculer entre 2 points ; donne un point entre A et B selon un %
            // place la cam entre sa position de depart et sa position d'arrivée => cb de % du chemin smoothT on a deja fait
            transform.position = Vector3.Lerp(originalPosition, targetPosition, smoothT);
            transform.rotation = Quaternion.Lerp(originalRotation, targetRotation, smoothT);
            cam.fieldOfView = Mathf.Lerp(originalFOV, zoomFOV, smoothT);

            yield return null; // le marque page du livre, 6O frames per sec game donc la boucle s'écxecute 60 fois par seconde en bougeant la camera un tt ptt peu à chaque passage
        }

        // Short pause to let the player admire the shadow
        yield return new WaitForSeconds(0.5f);

        // Hide the black bars before showing the victory panel
        if (letterbox != null)
            yield return StartCoroutine(letterbox.HideBars()); // lance l'annimation et att qu'elle soit completement terminée avant de continuer

        // Invoke the callback → triggers the PuzzleSolvedPanel display
        // ? fait le seulement si on m'a bien donnée une méthode 
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

            // Smooth ease in-out curve for fluid return ; smoothstep polynomial (3t² - 2t³)
            float smoothT = t * t * (3f - 2f * t);

            // Interpolate back to original position, rotation and FOV
            // Lerp = linear interpolation
            transform.position = Vector3.Lerp(currentPos, originalPosition, smoothT);
            transform.rotation = Quaternion.Lerp(currentRot, originalRotation, smoothT);
            cam.fieldOfView = Mathf.Lerp(currentFOV, originalFOV, smoothT);

            yield return null;
        }
    }
}