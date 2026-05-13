using DQHieu.Framework;
using UnityEngine;

public class FoodServedEvent : IGameEvent
{
    public DishScore dishScore;

    public FoodServedEvent(DishScore dishScore)
    {
        this.dishScore = dishScore;
    }
}
