using DQHieu.Framework;
using UnityEngine;

public class FoodServedEvent : IGameEvent
{
    public Customer customer;

    public FoodServedEvent(Customer customer)
    {
        this.customer = customer;
    }
}
