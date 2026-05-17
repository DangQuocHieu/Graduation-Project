using DQHieu.Framework;
using UnityEngine;
using UnityEngine.UI;
public class OrderScreen : MonoBehaviour
{
    public CustomerOrderButton customerOrderButtonPrefab;
    public OrderTicketUI orderTicketUI;
    public RectTransform customerOrderButtonGroup;

    void OnEnable()
    {
        EventBus.Subcribe<CustomerOrderComplete>(HandleCustomerOrderComplete);
    }

    void OnDisable()
    {
        EventBus.UnSubcribe<CustomerOrderComplete>(HandleCustomerOrderComplete);
    }

    private void HandleCustomerOrderComplete(CustomerOrderComplete evt)
    {
        var btn = Instantiate(customerOrderButtonPrefab, customerOrderButtonGroup);
        btn.Initialize(evt.customer, orderTicketUI);
    }
}
