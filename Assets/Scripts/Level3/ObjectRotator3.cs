using UnityEngine;
using UnityEngine.InputSystem;

/*
variable static = variable partagée entre toutes les instances du script, pas une copie par objet mais une seule variable pour tout le jeu
4 pivots different ; besoin d'avoir 1 pivot selectionné
*/
public class ObjectRotator3 : MonoBehaviour
{
    public float rotationSpeed = 0.3f;
    public float moveSpeed = 0.01f;

    // The currently selected object (shared across all instances)
    // static appliqué à un champ d'instance ; donné globale
    private static ObjectRotator3 selectedObject = null; // partagee entre tt les instances du script

    private bool isDragging = false;
    private float lastMouseX;
    private float lastMouseY;

    void Update()
    {
        HandleSelection();

        // Only the selected object processes input ; si l'objet actuellement sélectionné n'est pas moi arrete toi ici et ne fais rien de plus
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
            // dès qu'un click vient de démarrer on lance un raycast à traver le point où le joueur a cliaque à l'écran vers l'intérieur de la scène 3D
            // Camera.main.ScreenPointToRay(positionEcran) => conversion d'une position 2D à l'ecran en 1 ligne 3D dans l'espace
            // calcul de perspective automatique grâce à la méthode
            Ray ray = Camera.main.ScreenPointToRay(
                Mouse.current.position.ReadValue()
            );

            // Physics.Raycast(ray, out hit) envoie le rayon dans la scène + check si il touche un collieder sur son chemin
            // OUT = si ca touche qqch remplis la variable HIT avec les détails de ce qui a été touché
            // renvoie true/false si qqch a été touché ou non
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                // DEBUG : shows exactly what the ray touched
                //Debug.Log("[RAYCAST] Hit: " + hit.collider.gameObject.name);

                // le rayon touche un collider mais ce collider pt etre un mesh enfant et pas le pivot parent
                // OR ObjectRotator3 est attaché au pivot parent pas au mesh enfant
                // GetComponentInParent<T>() remonte la hiérarchie vers le haut, du collider clické enfant vers son parent
                ObjectRotator3 clicked = hit.collider.GetComponentInParent<ObjectRotator3>();

                if (clicked != null)
                {
                    selectedObject = clicked; // si un objectrotator a bien été trouvé il devient le nouvel objet sélectionné ; remplace l'ancienne sélection
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

        // check si un keyboard a bien été détecté
        bool isCtrl  = Keyboard.current != null && Keyboard.current.ctrlKey.isPressed;
        bool isShift = Keyboard.current != null && Keyboard.current.shiftKey.isPressed;

        if (isShift)
        {
            // SHIFT + drag → move on world X and Y only, never on Z (depth locked) evite qu'on sorte de la lumière
            Vector3 move = new Vector3(deltaX * moveSpeed, deltaY * moveSpeed, 0f);
            transform.position += move;
        }
        else if (isCtrl)
        {
            // CTRL + drag → vertical rotation (X axis)
            // utilisation de rotate ici et non rotatearound car plus confiance en mes skills unity pour bien placé le pivot parent x)
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