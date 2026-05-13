using System.Collections.Generic;
using DQHieu.Framework;
using UnityEngine;

public struct CustomerOrderComplete : IGameEvent
{
    public CustomerOrderComplete(CustomerOrder order)
    {
        this.order = order;
    }
    public CustomerOrder order;
}
