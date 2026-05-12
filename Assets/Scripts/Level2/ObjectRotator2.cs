using UnityEngine;
using UnityEngine.InputSystem;

public class ObjectRotator2 : MonoBehaviour
{

    // public
    public float rotationSpeed = 0.3f;

    // private
    private bool isDragging = false; // is the mouse slidding? 
    private bool wasPressed = false; // click state at previous frame
    private float lastMouseX; 
    private float lastMouseY;
    private Renderer cachedRenderer; // stock the renderer so we do not have to look for it at each frame

    void Start()
    {
        cachedRenderer = GetComponentInChildren<Renderer>();
        // look for the renderer on the object once at startup
    }

    void Update()
    {
        bool isPressed = Mouse.current.leftButton.isPressed;

        if (isPressed && !wasPressed) // we just clicked
        {
            isDragging = true;
            lastMouseX = Mouse.current.position.x.ReadValue();
            lastMouseY = Mouse.current.position.y.ReadValue();
        }
        if (!isPressed && wasPressed) // we just realeased
            isDragging = false;

        wasPressed = isPressed; // saving current state for next frame

        if (isDragging && cachedRenderer != null)
        {
            float currentMouseX = Mouse.current.position.x.ReadValue(); // horizontal movement
            float currentMouseY = Mouse.current.position.y.ReadValue(); // vertical movement
            float deltaX = currentMouseX - lastMouseX;
            float deltaY = currentMouseY - lastMouseY;

            Vector3 center = cachedRenderer.bounds.center; // center of the 3D mesh

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