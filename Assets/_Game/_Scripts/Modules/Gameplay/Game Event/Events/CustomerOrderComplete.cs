using System.Collections.Generic;
using DQHieu.Framework;
using UnityEngine;

public struct CustomerOrderComplete : IGameEvent
{
    public CustomerOrderComplete(List<IngredientType> order)
    {
        this.order = order;
    }
    public List<IngredientType> order;
}
