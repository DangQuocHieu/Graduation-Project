using UnityEngine.UI;
using UnityEngine;
using TMPro;
using UnityEngine.AI;
using DQHieu.Framework;

public class CustomerOrderButton : MonoBehaviour, IInteractable
{
    public Image customerImage;
    public Customer attachedCustomer;
    public OrderTicketUI orderTicketUI;
    public TextMeshProUGUI timerText;

    void OnEnable()
    {
        EventBus.Subcribe<FoodServedEvent>(HandleFoodServedEvent);
    }

    void OnDisable()
    {
        EventBus.UnSubcribe<FoodServedEvent>(HandleFoodServedEvent);
    }

    void Update()
    {
        if(attachedCustomer == null)
        {
            return;
        }
        if(attachedCustomer.currentState is CustomerState.WaitingForFood || attachedCustomer.currentState is CustomerState.ServiceDelayed)
        timerText.text = attachedCustomer.stateTimer.ToMMSS();
    }

    public void OnInteract()
    {
        orderTicketUI.ShowIngredients(attachedCustomer.customerOrder);
    }

    public void Initialize(Customer customer, OrderTicketUI orderTicketUI)
    {
        customerImage.sprite = customer.customerSO.avatarSprite;
        attachedCustomer = customer;
        this.orderTicketUI = orderTicketUI;
        timerText.text = attachedCustomer.stateTimer.ToMMSS();
    }

    private void HandleFoodServedEvent(FoodServedEvent evt)
    {
        if(attachedCustomer == evt.customer)
        {
            if(orderTicketUI.currentOrder == evt.customer.customerOrder)
            {
                orderTicketUI.Hide();
                orderTicketUI.currentOrder = null;
            }
            Destroy(gameObject);
        }
    }
}
