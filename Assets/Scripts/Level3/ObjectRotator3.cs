using UnityEngine;
using UnityEngine.InputSystem;

public class ObjectRotator3 : MonoBehaviour
{
    public float rotationSpeed = 0.3f;
    public float moveSpeed = 0.01f;

    // The currently selected object (shared across all instances)
    private static ObjectRotator3 selectedObject = null;

    private bool isDragging = false;
    private float lastMouseX;
    private float lastMouseY;

    void Update()
    {
        HandleSelection();

        // Only the selected object processes input
        if (selectedObject != this)
            return;

        HandleDragStart();
        HandleDragEnd();

        if (isDragging)
            HandleDragMovement();
    }

    private void HandleSelection()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Ray ray = Camera.main.ScreenPointToRay(
                Mouse.current.position.ReadValue()
            );

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                // DEBUG : shows exactly what the ray touched
                Debug.Log("[RAYCAST] Hit: " + hit.collider.gameObject.name);

                // GetComponentInParent walks up the hierarchy, so the script
                // is found even if the Collider sits on a child mesh object
                ObjectRotator3 clicked = hit.collider.GetComponentInParent<ObjectRotator3>();

                if (clicked != null)
                {
                    selectedObject = clicked;
                    Debug.Log("[SELECTED] " + clicked.gameObject.name);
                }
                else
                {
                    Debug.Log("[NO ROTATOR] " + hit.collider.gameObject.name +
                              " has no ObjectRotator3 on itself or its parents.");
                }
            }
            else
            {
                Debug.Log("[RAYCAST] Hit nothing.");
            }
        }
    }

    private void HandleDragStart()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            isDragging = true;
            lastMouseX = Mouse.current.position.x.ReadValue();
            lastMouseY = Mouse.current.position.y.ReadValue();
        }
    }

    private void HandleDragEnd()
    {
        if (Mouse.current.leftButton.wasReleasedThisFrame)
            isDragging = false;
    }

    private void HandleDragMovement()
    {
        float currentMouseX = Mouse.current.position.x.ReadValue();
        float currentMouseY = Mouse.current.position.y.ReadValue();

        float deltaX = currentMouseX - lastMouseX;
        float deltaY = currentMouseY - lastMouseY;

        bool isCtrl  = Keyboard.current != null && Keyboard.current.ctrlKey.isPressed;
        bool isShift = Keyboard.current != null && Keyboard.current.shiftKey.isPressed;

        if (isShift)
        {
            // SHIFT + drag → move on world X and Y only, never on Z (depth locked)
            Vector3 move = new Vector3(deltaX * moveSpeed, deltaY * moveSpeed, 0f);
            transform.position += move;
        }
        else if (isCtrl)
        {
            // CTRL + drag → vertical rotation (X axis)
            transform.Rotate(Vector3.right, deltaY * rotationSpeed, Space.World);
        }
        else
        {
            // Simple drag → horizontal rotation (Y axis)
            transform.Rotate(Vector3.up, -deltaX * rotationSpeed, Space.World);
        }

        lastMouseX = currentMouseX;
        lastMouseY = currentMouseY;
    }
}