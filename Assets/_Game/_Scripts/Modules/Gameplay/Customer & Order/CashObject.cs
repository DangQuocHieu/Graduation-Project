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
        attachedCustomer.ChangeState(CustomerState.Leaving);
        EventBus.Raise<InteractWithCashObject>(new InteractWithCashObject(amount, attachedCustomer));
        gameObject.SetActive(false);
    }
}
