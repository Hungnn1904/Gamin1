using UnityEngine;
using UnityEngine.InputSystem;

public class Interactivedetecter : MonoBehaviour
{
    private IInteractable interactableInRange = null;
    public GameObject interactableIcon;
    void Start(){
        interactableIcon.SetActive(false);
    }
    public void OnInteract(InputAction.CallbackContext context) {
        if (context.performed)
        {
            interactableInRange?.Interact();
        }
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out IInteractable interactable) && interactable.CanInteract())
        {
            interactableInRange = interactable;
            interactableIcon.SetActive(true);
        }
    }
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent(out IInteractable interactable) && interactable == interactableInRange)
        {
            interactableInRange = null;
            interactableIcon.SetActive(false);
        }
    }
}
