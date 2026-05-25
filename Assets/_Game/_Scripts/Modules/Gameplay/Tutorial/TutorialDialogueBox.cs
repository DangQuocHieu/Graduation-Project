using System.Collections;
using TMPro;
using UnityEngine;
using DG.Tweening;

public class TutorialDialogueBox : MonoBehaviour
{
    public TextMeshProUGUI tutorialText;
    public UISlideTween uiSlideTween;
    
    [Header("Animation Settings")]
    public float timePerCharacter = 0.04f;
    [Tooltip("Thời gian trễ để chờ popup slide xuống hẳn (khớp với thời gian chạy slide của UISlideTween)")]
    public float popupSlideDuration = 0.3f; 

    private Coroutine tutorialSequenceCoroutine;
    private Tween typingTween;

    public void ShowDialogueBox(string text, float displayDuration = 3f)
    {
        // Kiểm tra xem có cần chạy hiệu ứng SlideIn không:
        // Cần SlideIn nếu GameObject của UI chưa active, HOẶC nếu coroutine hiện tại là null
        // (nghĩa là dialogue box đang không hiển thị thoại nào, có thể đang ở trạng thái ẩn hoặc đang chạy SlideOut).
        bool needsSlideIn = !uiSlideTween.gameObject.activeSelf || (tutorialSequenceCoroutine == null);

        // 1. Dập toàn bộ Coroutine và Tween cũ nếu người chơi đổi câu thoại liên tục
        if (tutorialSequenceCoroutine != null)
        {
            StopCoroutine(tutorialSequenceCoroutine);
            tutorialSequenceCoroutine = null;
        }
        typingTween?.Kill();

        // 2. Chạy luồng xử lý tuần tự qua Coroutine mới
        tutorialSequenceCoroutine = StartCoroutine(DialogueSequence(text, displayDuration, needsSlideIn));
    }

    private IEnumerator DialogueSequence(string text, float displayDuration, bool needsSlideIn)
    {
        // Bước 1: Clear text cũ về rỗng trước khi popup xuất hiện
        tutorialText.text = "";

        // Bước 2: Kích hoạt hiệu ứng slide xuống của popup nếu cần thiết
        if (needsSlideIn)
        {
            uiSlideTween.SlideIn();
            // CHỜ: Đợi popup trượt xuống hoàn toàn rồi mới chạy dòng lệnh tiếp theo
            yield return new WaitForSeconds(popupSlideDuration);
        }

        // Bước 3: Sau khi popup đã đứng yên ➔ Bắt đầu chạy hiệu ứng gõ chữ
        float totalTypingDuration = text.Length * timePerCharacter;
        typingTween = tutorialText.DOText(text, totalTypingDuration).SetEase(Ease.Linear);

        // CHỜ: Đợi cho chữ gõ xong hoàn toàn
        yield return typingTween.WaitForCompletion();

        // Bước 4: Chờ thêm một khoảng thời gian tĩnh để người chơi đọc xong câu thoại
        yield return new WaitForSeconds(displayDuration);

        // Bước 5: Tự động ẩn bảng thoại đi
        HideDialogueBox();
    }

    public void HideDialogueBox()
    {
        if (tutorialSequenceCoroutine != null)
        {
            StopCoroutine(tutorialSequenceCoroutine);
            tutorialSequenceCoroutine = null;
        }
        typingTween?.Kill();
        
        uiSlideTween.SlideOut();
    }

    private void OnDestroy()
    {
        typingTween?.Kill();
    }
}