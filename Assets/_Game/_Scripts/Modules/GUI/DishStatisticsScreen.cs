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
    public GameLoopManager gameLoopManager;

    void OnEnable()
    {
        EventBus.Subcribe<InteractWithCashObject>(HandleInteractWithCashObject);
        EventBus.Subcribe<LevelComplete>(HandleLevelCompleteEvent);
        nextButton.onClick.AddListener(OnNextButtonClicked);
    }

    void OnDisable()
    {
        EventBus.UnSubcribe<InteractWithCashObject>(HandleInteractWithCashObject);
        EventBus.UnSubcribe<LevelComplete>(HandleLevelCompleteEvent);
        nextButton.onClick.RemoveListener(OnNextButtonClicked);
    }

    public void Initialize(GameLoopManager gameLoopManager)
    {
        this.gameLoopManager = gameLoopManager;
    }

    private void HandleInteractWithCashObject(InteractWithCashObject evt)
    {
        if (kccManager != null) kccManager.SetAllInputBlocked(true);

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
        if (kccManager != null) kccManager.SetAllInputBlocked(false);
        EventBus.Raise<CustomerPaymentReceived>(new CustomerPaymentReceived(currentCustomer.dishScore.guestPaidAmount));
        HideScreen();
        CursorHelper.HideCursor();
    }

    private void HandleLevelCompleteEvent(LevelComplete evt)
    {
        nextButton.gameObject.SetActive(false);
    }
}