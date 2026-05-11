using DQHieu.Framework;


public struct InteractWithCashObject : IGameEvent
{
    public Customer customer;
    public int amount;

    public InteractWithCashObject(int amount, Customer customer)
    {
        this.customer = customer;
        this.amount = amount;
    }
}
