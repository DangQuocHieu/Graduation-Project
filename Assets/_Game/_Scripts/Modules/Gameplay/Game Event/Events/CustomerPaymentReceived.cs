using DQHieu.Framework;
using UnityEngine;

public struct CustomerPaymentReceived : IGameEvent
{
    public int amount;
    public CustomerPaymentReceived(int amount)
    {
        this.amount = amount;
    }
}
