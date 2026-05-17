using System.Collections.Generic;
using DQHieu.Framework;
using UnityEngine;
using UnityEngine.UI;
public class OrderScreen : MonoBehaviour
{
    public CustomerOrderButton customerOrderButtonPrefab;
    public OrderTicketUI orderTicketUI;
    public RectTransform customerOrderButtonGroup;
    public List<CustomerOrderButton> customerOrderButtons = new();

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
        btn.Initialize(evt.customer, orderTicketUI, this);
        customerOrderButtons.Add(btn);
    }

    public void OnCustomerOrderButtonClicked(CustomerOrderButton btn)
    {
        foreach(var customerOrderButton in customerOrderButtons)
        {
            customerOrderButton.selectedOutline.gameObject.SetActive(false);
        }

        btn.selectedOutline.gameObject.SetActive(true);
    }
}
