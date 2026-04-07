using DQHieu.Framework;

[System.Serializable]
public class PlayerData
{
    public int money
    {
        get
        {
            return money;
        }
        set
        {
            money = value;
            EventBus.SendMessage<MoneyChangeEvent>(new MoneyChangeEvent());
        }
        
    }
}
