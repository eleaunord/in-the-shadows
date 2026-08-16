using UnityEngine;
using UnityEngine.InputSystem;

/*
Cache : optimisation vs calcul à chaque frame

ObjectRotator (niveau 1) : calcule du pivotPoint une seule fois dans Start() via Bounds.Encapsulate
puis stockage dans un Vector3

ObjectRotator2 (niveau 2) : stock la reference vers le composant (cachedRenderer), puis on recalcules son centre à chaque frame dans Update()
référence vers le composant — c'est-à-dire "où se trouve le Renderer". GetComponentInChildren<Renderer>() fait un parcours de la hiérarchie du GameObject pour le trouver — une opération relativement coûteuse si elle est répétée à chaque frame, mais dont le résultat (le composant lui-même) ne change jamais pendant toute la vie de l'objet. Un objet ne change pas de Renderer en cours de partie. Donc chercher une fois, garder la référence, c'est logique : la réponse à "quel est ton Renderer ?" est une donnée stable.
// adresse d'un ami
vs bounds.center ou se trouve ton ami a un instant précis

*/
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
            bool isCtrlHeld = Keyboard.current != null && Keyboard.current.ctrlKey.isPressed;

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