using System.Collections.Generic;
using DG.Tweening;
using DQHieu.Framework;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SummaryScreen : MonoBehaviour
{
    [TabGroup("UI Ref")] public RectTransform overlay;
    [TabGroup("UI Ref")] public UISlideTween popup;
    [TabGroup("UI Ref")] public Button nextButton;

    [TabGroup("Text")] public TextMeshProUGUI ordersAmountText;
    [TabGroup("Text")] public TextMeshProUGUI finishedOrdersAmountText;
    [TabGroup("Text")] public TextMeshProUGUI expenseAmountText;
    [TabGroup("Text")] public TextMeshProUGUI mealIncomeText;
    [TabGroup("Text")] public TextMeshProUGUI totalIncomeText;
    [TabGroup("Text")] public TextMeshProUGUI waitingScoreText;
    [TabGroup("Text")] public TextMeshProUGUI tasteScoreText;
    [TabGroup("Text")] public TextMeshProUGUI overallScoreText;

    [TabGroup("Grade Images")] public List<CanvasGroup> gradeImages = new();
    private Sequence showScreenSeq;
    private LevelStatisticsManager levelStatisticsManager;
    private CanvasGroup displayGradeImage;

    void OnEnable()
    {
        EventBus.Subcribe<LevelComplete>(HandleLevelCompleteEvent);
        nextButton.onClick.AddListener(OnNextButtonClicked);
    }

    void OnDisable()
    {
        EventBus.UnSubcribe<LevelComplete>(HandleLevelCompleteEvent);
        nextButton.onClick.RemoveListener(OnNextButtonClicked);
    }

    public void Initialize(LevelStatisticsManager levelStatisticsManager)
    {
        this.levelStatisticsManager = levelStatisticsManager;
    }

    private void HandleLevelCompleteEvent(LevelComplete evt)
    {
        showScreenSeq?.Kill();
        showScreenSeq = DOTween.Sequence();
        showScreenSeq.Append(popup.SlideIn());

        showScreenSeq.AppendInterval(1f);
        showScreenSeq.Join(AnimateOrderText(ordersAmountText, levelStatisticsManager.orderAmount));
        showScreenSeq.Join(AnimateOrderText(finishedOrdersAmountText, levelStatisticsManager.finishedOrder));
        showScreenSeq.Join(AnimateMoneyText(expenseAmountText, levelStatisticsManager.expense));
        showScreenSeq.Join(AnimateMoneyText(mealIncomeText, levelStatisticsManager.mealIncome));
        showScreenSeq.Join(AnimateMoneyText(totalIncomeText, levelStatisticsManager.GetTotalIncome()));
        showScreenSeq.Join(AnimateStatisticsText(waitingScoreText, levelStatisticsManager.GetTotalWaitingScore()));
        showScreenSeq.Join(AnimateStatisticsText(tasteScoreText, levelStatisticsManager.GetTotalTasteScore()));
        showScreenSeq.Join(AnimateStatisticsText(overallScoreText, levelStatisticsManager.GetOverallScore()));

        showScreenSeq.AppendInterval(1f);
        DisplayGradeImage();
        showScreenSeq.Append(AnimateGradePop(displayGradeImage));

        showScreenSeq.SetLink(gameObject).SetUpdate(true);
        nextButton.gameObject.SetActive(true);



    }

    private Tween AnimateStatisticsText(TextMeshProUGUI text, int value)
    {
        text.text = "0%";
        return TweenHelper.AnimateInt(0, value, 1f, onUpdate: (currentValue) =>
        {
            text.text = currentValue.ToString() + "%";
        });
    }

    public Tween AnimateOrderText(TextMeshProUGUI text, int value)
    {
        return TweenHelper.AnimateInt(0, value, 1f, onUpdate: (currentValue) =>
        {
            text.text = currentValue.ToString();
        });
    }
    private Tween AnimateMoneyText(TextMeshProUGUI text, int value)
    {
        return TweenHelper.AnimateInt(0, value, 1f, onUpdate: (currentValue) =>
        {
            text.text = currentValue.ToVNDCurrency();
        });

    }

    public void DisplayGradeImage()
    {
        float score = levelStatisticsManager.GetOverallScore();
        int[] scoreThresholds = { 100, 94, 87, 80, 74, 67, 60, 50, 0 };

        for (int i = 0; i < scoreThresholds.Length; i++)
        {
            if (score >= scoreThresholds[i])
            {
                displayGradeImage = gradeImages[i];
                displayGradeImage.gameObject.SetActive(true);
                return;
            }
        }
    }

    private Tween AnimateGradePop(CanvasGroup target)
    {
        // Bắt đầu từ rất lớn và mờ
        target.alpha = 0f;
        target.transform.localScale = Vector3.one * 2.5f;
        target.gameObject.SetActive(true);

        Sequence seq = DOTween.Sequence();

        // Thời gian nhanh (0.2s - 0.25s) tạo độ gắt
        seq.Join(target.DOFade(1f, 0.2f));
        // Dùng OutBack để có độ nảy nhẹ khi chạm đích
        seq.Join(target.transform.DOScale(1f, 0.25f).SetEase(Ease.OutBack));

        return seq;
    }
    private void OnNextButtonClicked()
    {

    }
}
