using DG.Tweening;
using DQHieu.Framework;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DishStatisticsScreen : MonoBehaviour
{
    [TabGroup("UI Ref")]
    public RectTransform overlay;
    [TabGroup("UI Ref")]
    public UISlideTween popUp;
    [TabGroup("UI Ref")]
    public TextMeshProUGUI guestPaidText;
    [TabGroup("UI Ref")]
    public Button nextButton;

    [TabGroup("Score Slider")]
    public DishScoreSlider waitingSlider;
    [TabGroup("Score Slider")]
    public DishScoreSlider tasteSlider;
    [TabGroup("Score Slider")]
    public DishScoreSlider totalSlider;

    private Sequence showScreenTween;
    public Customer currentCustomer;

    void OnEnable()
    {
        EventBus.Subcribe<InteractWithCashObject>(HandleInteractWithCashObject);
        nextButton.onClick.AddListener(OnNextButtonClicked);
    }

    void OnDisable()
    {
        EventBus.UnSubcribe<InteractWithCashObject>(HandleInteractWithCashObject);
        nextButton.onClick.RemoveListener(OnNextButtonClicked);
    }

    private void HandleInteractWithCashObject(InteractWithCashObject evt)
    {
        currentCustomer = evt.customer;
        var dishScore = currentCustomer.dishScore;

        waitingSlider.SetUp(dishScore.displayWatingScore);
        tasteSlider.SetUp(dishScore.displayTasteScore);     
        totalSlider.SetUp(dishScore.GetTotalScore());
        overlay.gameObject.SetActive(true);
        
        showScreenTween?.Kill();
        showScreenTween = DOTween.Sequence(); 
        showScreenTween.Append(popUp.SlideIn());
        showScreenTween.AppendInterval(1f);

        showScreenTween.Join(waitingSlider.AnimateScore());
        showScreenTween.Join(tasteSlider.AnimateScore());
        showScreenTween.Join(totalSlider.AnimateScore());

        showScreenTween.AppendInterval(1f);
        showScreenTween.Append(AnimateGuestPaidAmount(dishScore.guestPaidAmount, 1f));

        showScreenTween.SetLink(gameObject).SetUpdate(true);
    }

    private Tween AnimateGuestPaidAmount(int targetAmount, float duration)
    {
        int currentPaid = 0;
        UpdateGuestPaidText(0); 

        return DOTween.To(() => currentPaid, x => 
        {
            currentPaid = x;
            UpdateGuestPaidText(currentPaid);
        }, targetAmount, duration).SetEase(Ease.OutQuad);
    }

    private void UpdateGuestPaidText(int amount)
    {
        guestPaidText.text = "GUEST PAID: " +  amount.ToVNDCurrency();
    }

    public void HideScreen()
    {
        overlay.gameObject.SetActive(false);
        popUp.gameObject.SetActive(false);
    }

    public void OnNextButtonClicked()
    {
        EventBus.SendMessage<CustomerPaymentReceived>(new CustomerPaymentReceived(currentCustomer.dishScore.guestPaidAmount));
        HideScreen();
    }
}