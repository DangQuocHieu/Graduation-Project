using UnityEngine;

public class CustomerOrderController : MonoBehaviour, IInteractable
{
    public bool orderAccepted = false;
    public GameObject takeOrderButton;

    public void OnInteract()
    {
        HandleOrderAccepted();
    }

    public void HandleOrderAccepted()
    {
        orderAccepted = true;
        takeOrderButton.gameObject.SetActive(false);}
}
