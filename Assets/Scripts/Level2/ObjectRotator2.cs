using UnityEngine;
using UnityEngine.InputSystem;

public class ObjectRotator2 : MonoBehaviour
{

    // public
    public float rotationSpeed = 0.3f;

    // private
    private bool isDragging = false; // is the mouse slidding? 
    private float lastMouseX; 
    private float lastMouseY; // en plus pr ce niveau
    private Renderer cachedRenderer; // stock the renderer so we do not have to look for it at each frame

    void Start()
    {
        cachedRenderer = GetComponentInChildren<Renderer>();
        // look for the renderer on the object once at startup
    }

    void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame) // we just clicked
        {
            isDragging = true;
            lastMouseX = Mouse.current.position.x.ReadValue();
            lastMouseY = Mouse.current.position.y.ReadValue();
        }

        if (Mouse.current.leftButton.wasReleasedThisFrame) // we just released
        {
            isDragging = false;
        }

        if (isDragging && cachedRenderer != null)
        {
            float currentMouseX = Mouse.current.position.x.ReadValue(); // horizontal movement
            float currentMouseY = Mouse.current.position.y.ReadValue(); // vertical movement
            
            //déplacement de la souris depuis la dernière frame, sur les deux axes cette fois (horizontal et vertical)
            float deltaX = currentMouseX - lastMouseX;
            float deltaY = currentMouseY - lastMouseY;

            Vector3 center = cachedRenderer.bounds.center; // center of the 3D mesh

            // lit l'état de la touche CTRL via le nouveau Input System
            bool isCtrlHeld = Keyboard.current.ctrlKey.isPressed;

            if (isCtrlHeld)
                // CTRL + drag = vertical rotation (around x axis)
                transform.RotateAround(center, Vector3.right, deltaY * rotationSpeed);
            else
                // simple drag = horizontal rotation (around y axis)
                transform.RotateAround(center, Vector3.up, -deltaX * rotationSpeed);

            lastMouseX = currentMouseX;
            lastMouseY = currentMouseY;
        }
    }
}