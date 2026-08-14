using UnityEngine;
using UnityEngine.InputSystem;

public class ObjectRotator : MonoBehaviour
{
    public float rotationSpeed = 0.3f;      // rotation speed multiplier
    private bool isDragging = false;        // tracks whether the mouse button is held
    private float lastMouseX;              // mouse X position from the previous frame
    private Vector3 pivotPoint;            // visual center of the object used as rotation pivot

    void Start()
    {
        // Automatically calculate the visual center of the object using its renderers
       
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
            // Vector3.up = axe y
            //deltaX = currentMouseX - lastMouseX = de combien la souris a bougé horizontalement depuis la dernière frame
            transform.RotateAround(pivotPoint, Vector3.up, -deltaX * rotationSpeed);

            // Save current mouse position for next frame
            lastMouseX = currentMouseX;
        }
    }
}