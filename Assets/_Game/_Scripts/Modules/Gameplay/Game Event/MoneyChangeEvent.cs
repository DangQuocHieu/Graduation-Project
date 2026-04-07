using DQHieu.Framework;
using UnityEngine;

public struct MoneyChangeEvent : IGameEvent
{
    public int currentMoney;

    public MoneyChangeEvent(int currentMoney)
    {
        this.currentMoney = currentMoney;
    } 
}
