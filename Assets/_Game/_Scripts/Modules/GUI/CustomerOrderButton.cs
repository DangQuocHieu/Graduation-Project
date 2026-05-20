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
    public OrderScreen orderScreen;
    public RectTransform selectedOutline;

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
        orderTicketUI.gameObject.SetActive(true);
        orderScreen.OnCustomerOrderButtonClicked(this);
    }

    public void Initialize(Customer customer, OrderTicketUI orderTicketUI, OrderScreen orderScreen)
    {
        customerImage.sprite = customer.customerSO.avatarSprite;
        attachedCustomer = customer;
        this.orderTicketUI = orderTicketUI;
        timerText.text = attachedCustomer.stateTimer.ToMMSS();
        this.orderScreen = orderScreen;
    }

    private void HandleFoodServedEvent(FoodServedEvent evt)
    {
        if(attachedCustomer == evt.customer)
        {
            if(orderTicketUI.currentOrder == evt.customer.customerOrder)
            {
                orderTicketUI.Hide();
                orderTicketUI.currentOrder = null;
                orderScreen.customerOrderButtons.Remove(this);
            }
            Destroy(gameObject);
        }
    }
}
