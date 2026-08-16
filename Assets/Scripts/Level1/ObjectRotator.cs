using UnityEngine;
using UnityEngine.InputSystem;

/*
Bounds = type Unity qui rpz une boîte englobante alignée sur les axes

Mouse.current = nouvel Input System (le nouveau GetMouseButtonDown(0))
=> réference statique vers la souris actuellement active
.leftButton : le bouton gauche 
.wasPressedThisFrame : true seulement pendant la frame exacte où le clic vient d'arriver (pas à chaque frame tant qu'il reste appuyé).
.wasReleasedThisFrame : symétrique, true seulement à la frame du relâchement.
.position est ce qu'on appelle une InputAction ou valeur d'entrée composite (un Vector2 représentant x et y de la souris à l'écran). .x en extrait la composante horizontale, et 
.ReadValue() est la méthode qui lit effectivement la valeur actuelle à cet instant
*/
public class ObjectRotator : MonoBehaviour
{
    public float rotationSpeed = 0.3f;      // rotation speed multiplier
    private bool isDragging = false;        // tracks whether the mouse button is held
    private float lastMouseX;              // mouse X position from the previous frame
    private Vector3 pivotPoint;            // visual center of the object used as rotation pivot

    void Start()
    {
        // Automatically calculate the visual center of the object using its renderers
       
       // GEOMETRIE 3D
        // recupère tous les composants renderer (morceaux visibles, maillages) présents sur cet objet et ses enfants
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        if (renderers.Length > 0)
        {
            // Expand bounds to encapsulate all child renderers
            // = boite englobante qui contient tt la mesh
            Bounds bounds = renderers[0].bounds; // demarre avec la boite du premier trouvé
            foreach (Renderer r in renderers)
                bounds.Encapsulate(r.bounds); // agrandit la boite pour qu'elle englobe aussi les autres meshes

            // Use the center of the combined bounds as the pivot point
            pivotPoint = bounds.center; // centre geometrique de la boite 
        }
        else
        {
            // Fallback : use the object's transform position if no renderer is found
            pivotPoint = transform.position;
        }
    }

    void Update()
    {
        // Start dragging when the left mouse button is pressed
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            isDragging = true;
            lastMouseX = Mouse.current.position.x.ReadValue();
        }

        // Stop dragging when the left mouse button is released
        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            isDragging = false;
        }

        // Rotate the object while the mouse button is held
        if (isDragging)
        {
            float currentMouseX = Mouse.current.position.x.ReadValue();

            // Calculate how much the mouse moved horizontally since last frame
            float deltaX = currentMouseX - lastMouseX;

            // Rotate around the object's visual center on the Y axis (horizontal rotation)
            // et non rotate sinon ca risque de faire tourner autour de l'origine du monde
            // Vector3.up = axe y (racourcie de new Vector3(0,1,0))
            //deltaX = currentMouseX - lastMouseX = de combien la souris a bougé horizontalement depuis la dernière frame
            transform.RotateAround(pivotPoint, Vector3.up, -deltaX * rotationSpeed);

            // Save current mouse position for next frame
            lastMouseX = currentMouseX;
        }
    }
}