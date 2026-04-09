using DQHieu.Framework;
using TMPro;
using UnityEngine;

public class MoneyUIPanel : MonoBehaviour
{
    public TextMeshProUGUI moneyText;

    void OnEnable()
    {
        EventBus.Subcribe<MoneyChangeEvent>(HandleMoneyChangeEvent);
    }

    void OnDisable()
    {
        EventBus.UnSubcribe<MoneyChangeEvent>(HandleMoneyChangeEvent);
    }
    public void Initialize(DataManager dataManager)
    {
        UpdateUI(dataManager.playerData.Money);
    }

    private void HandleMoneyChangeEvent(MoneyChangeEvent evt)
    {
        UpdateUI(evt.currentMoney);
    }

    public void UpdateUI(int money)
    {
        moneyText.text = CurrencyFormatter.ToGameCurrency(money);
    }
}
