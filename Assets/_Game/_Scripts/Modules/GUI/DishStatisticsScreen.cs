using CoreGame.Movement;
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
    public KCCManager kccManager;

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
        if (kccManager != null) kccManager.BlockInput();

        CursorHelper.ShowCursor();
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
        Tween moneyTween = TweenHelper.AnimateInt(
            startValue: 0, 
            endValue: dishScore.guestPaidAmount, 
            duration: 1f, 
            onUpdate: (currentValue) => 
            {
                guestPaidText.text = "GUEST PAID: " + currentValue.ToVNDCurrency();
            }
        );

        showScreenTween.Append(moneyTween);

        showScreenTween.SetLink(gameObject).SetUpdate(true);
    }

    public void HideScreen()
    {
        overlay.gameObject.SetActive(false);
        popUp.gameObject.SetActive(false);
    }

    public void OnNextButtonClicked()
    {
        if (kccManager != null) kccManager.UnblockInput();

        EventBus.SendMessage<CustomerPaymentReceived>(new CustomerPaymentReceived(currentCustomer.dishScore.guestPaidAmount));
        HideScreen();
        CursorHelper.HideCursor();
        if(currentCustomer.isLastCustomer)
        {
            CursorHelper.ShowCursor();
            EventBus.SendMessage<LevelComplete>(new LevelComplete());
        }
    }
}