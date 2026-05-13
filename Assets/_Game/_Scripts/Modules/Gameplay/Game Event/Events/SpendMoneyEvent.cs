using DQHieu.Framework;
using UnityEngine;

public class SpendMoneyEvent : IGameEvent
{
    public int amount;

    public SpendMoneyEvent(int amount)
    {
        this.amount = amount;
    }
}
