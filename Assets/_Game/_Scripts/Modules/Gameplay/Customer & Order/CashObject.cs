using DQHieu.Framework;
using TMPro;
using UnityEngine;

public class CashObject : MonoBehaviour, IInteractable
{
    public int amount = 20000;
    public TextMeshPro amountText;
    public Customer attachedCustomer;

    public void OnInteract()
    {
        EventBus.Raise<InteractWithCashObject>(new InteractWithCashObject(amount, attachedCustomer));
        attachedCustomer.ChangeState(CustomerState.Leaving);
        gameObject.SetActive(false);
    }
}
