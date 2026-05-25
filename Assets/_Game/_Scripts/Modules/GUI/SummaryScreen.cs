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
    [TabGroup("UI Ref")] public Button homeButton;
    [TabGroup("UI Ref")] public Button tryAgainButton;

    [TabGroup("Text")] public TextMeshProUGUI ordersAmountText;
    [TabGroup("Text")] public TextMeshProUGUI finishedOrdersAmountText;
    [TabGroup("Text")] public TextMeshProUGUI expenseAmountText;
    [TabGroup("Text")] public TextMeshProUGUI mealIncomeText;
    [TabGroup("Text")] public TextMeshProUGUI totalIncomeText;
    [TabGroup("Text")] public TextMeshProUGUI waitingScoreText;
    [TabGroup("Text")] public TextMeshProUGUI tasteScoreText;
    [TabGroup("Text")] public TextMeshProUGUI overallScoreText;
    [TabGroup("Text")] public TextMeshProUGUI levelText;

    [TabGroup("Grade Images")] public List<CanvasGroup> gradeImages = new();
    private Sequence showScreenSeq;
    private LevelStatisticsManager levelStatisticsManager;
    private CanvasGroup displayGradeImage;

    void OnEnable()
    {
        EventBus.Subcribe<LevelComplete>(HandleLevelCompleteEvent);
        nextButton.onClick.AddListener(OnNextButtonClicked);
        homeButton.onClick.AddListener(OnHomeButtonClicked);
        tryAgainButton.onClick.AddListener(OnTryAgainButtonClicked);
    }

    void OnDisable()
    {
        EventBus.UnSubcribe<LevelComplete>(HandleLevelCompleteEvent);
        nextButton.onClick.RemoveListener(OnNextButtonClicked);
        homeButton.onClick.RemoveListener(OnHomeButtonClicked);
        tryAgainButton.onClick.RemoveListener(OnTryAgainButtonClicked);
    }
    void Start()
    {
        if(!DataManager.Instance.playerData.TutorialCompleted)
        {
            levelText.text = "Tutorial";
        }
        else levelText.text = "Level " + (DataManager.Instance.playerData.CurrentLevelIndex + 1);
    }

    public void Initialize(LevelStatisticsManager levelStatisticsManager)
    {
        this.levelStatisticsManager = levelStatisticsManager;
    }

    private void HandleLevelCompleteEvent(LevelComplete evt)
    {
        // 1. Ẩn tất cả các nút điều hướng trước khi chạy tween
        nextButton.gameObject.SetActive(false);
        tryAgainButton.gameObject.SetActive(false);
        homeButton.gameObject.SetActive(false);

        // 2. Xác định điểm số và Grade Image
        DisplayGradeImage();

        showScreenSeq?.Kill();
        showScreenSeq = DOTween.Sequence();

        showScreenSeq.AppendInterval(5f);
        showScreenSeq.AppendCallback(() =>
        {
            overlay.gameObject.SetActive(true);
            CursorHelper.ShowCursor();
        });

        showScreenSeq.Append(popup.SlideIn());

        // Chạy hiệu ứng tăng chỉ số (Text) đồng thời
        showScreenSeq.AppendInterval(1f);
        showScreenSeq.Join(AnimateOrderText(ordersAmountText, levelStatisticsManager.orderAmount));
        showScreenSeq.Join(AnimateOrderText(finishedOrdersAmountText, levelStatisticsManager.finishedOrder));
        showScreenSeq.Join(AnimateMoneyText(expenseAmountText, levelStatisticsManager.expense));
        showScreenSeq.Join(AnimateMoneyText(mealIncomeText, levelStatisticsManager.mealIncome));
        showScreenSeq.Join(AnimateMoneyText(totalIncomeText, levelStatisticsManager.GetTotalIncome()));
        showScreenSeq.Join(AnimateStatisticsText(waitingScoreText, levelStatisticsManager.GetTotalWaitingScore()));
        showScreenSeq.Join(AnimateStatisticsText(tasteScoreText, levelStatisticsManager.GetTotalTasteScore()));
        showScreenSeq.Join(AnimateStatisticsText(overallScoreText, levelStatisticsManager.GetOverallScore()));

        // Hiện Grade đóng dấu (A, B, C...)
        showScreenSeq.AppendInterval(1f);
        showScreenSeq.Append(AnimateGradePop(displayGradeImage));

        // ==========================================
        // THÊM: TWEEN HIỆN CÁC BUTTON SAU CÙNG
        // ==========================================
        showScreenSeq.AppendCallback(() =>
        {
            int score = levelStatisticsManager.GetOverallScore();

            // Hiện nút Home bất kể thắng hay thua
            homeButton.gameObject.SetActive(true);
            AnimateButtonScale(homeButton.transform);

            if (score >= 50)
            {
                // Thắng -> Hiện nút Next
                nextButton.gameObject.SetActive(true);
                AnimateButtonScale(nextButton.transform);
            }
            else
            {
                // Thua -> Hiện nút Try Again
                tryAgainButton.gameObject.SetActive(true);
                AnimateButtonScale(tryAgainButton.transform);
            }
        });

        showScreenSeq.SetLink(gameObject).SetUpdate(true);
    }

    // Hàm phụ trợ tạo hiệu ứng xuất hiện nảy nhẹ (Pop) cho Button giống như Grade Image
    private void AnimateButtonScale(Transform btnTransform)
    {
        btnTransform.localScale = Vector3.zero;
        btnTransform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack).SetUpdate(true);
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
        int score = levelStatisticsManager.GetOverallScore();
        Debug.Log(score);
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
        target.alpha = 0f;
        target.transform.localScale = Vector3.one * 2.5f;
        target.gameObject.SetActive(true);

        Sequence seq = DOTween.Sequence();
        seq.Join(target.DOFade(1f, 0.2f));
        seq.Join(target.transform.DOScale(1f, 0.25f).SetEase(Ease.OutBack));

        return seq;
    }

    private void OnNextButtonClicked() => SceneTransitionManager.Instance.LoadScene("GameplayScene");
    private void OnHomeButtonClicked() => SceneTransitionManager.Instance.LoadScene("HomeScene");
    private void OnTryAgainButtonClicked() => SceneTransitionManager.Instance.LoadScene("GameplayScene");
}