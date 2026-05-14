using DQHieu.Framework;
using UnityEngine;

public struct CustomerLeaveEvent : IGameEvent
{
    public Customer customer;

    public CustomerLeaveEvent(Customer customer)
    {
        this.customer = customer;
    }
}
