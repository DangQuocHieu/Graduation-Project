using UnityEngine;

public class CustomerOrderController : MonoBehaviour
{
    public bool orderAccepted = false;
    public GameObject takeOrderButton;

    public void HandleOrderAccepted()
    {
        orderAccepted = true;
        takeOrderButton.gameObject.SetActive(false);}
}
