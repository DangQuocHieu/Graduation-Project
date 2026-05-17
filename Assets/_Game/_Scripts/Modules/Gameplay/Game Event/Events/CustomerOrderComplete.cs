using System.Collections.Generic;
using DQHieu.Framework;
using UnityEngine;

public struct CustomerOrderComplete : IGameEvent
{
    public CustomerOrderComplete(Customer customer)
    {
        this.customer = customer;
    }
    public Customer customer;
}
