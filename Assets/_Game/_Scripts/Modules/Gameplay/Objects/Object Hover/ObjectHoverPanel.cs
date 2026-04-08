using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

[RequireComponent(typeof(CanvasGroup))]
public class ObjectHoverPanel : MonoBehaviour
{
    public TextMeshProUGUI displayNameText;
    public TextMeshProUGUI weightText;
    public RectTransform weightRect;
    public CanvasGroup canvasGroup;
    private bool isPanelVisible;

    [Tooltip("X: Trái/Phải, Y: Lên/Xuống, Z: Gần/Xa so với Camera")]
    public Vector3 positionOffset;
    [Header("Constant Size Settings")]
    public bool keepConstantSize = true;
    [Tooltip("Độ lớn của UI. Hãy điều chỉnh số này trên Inspector cho vừa mắt (vd: 0.01 - 0.05)")]
    public float sizeMultiplier = 0.05f;
    [Header("Animation Settings")]
    public float fadeDuration = 0.15f;
    public float moveDuration = 0.15f;

    private Camera mainCamera;
    private Vector3 currentTargetPosition;
    private Tween fadeTween;
    private Tween moveTween;

    private void Start()
    {
        mainCamera = Camera.main;
        
        // Đảm bảo khởi tạo trong trạng thái ẩn hoàn toàn
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    private void LateUpdate()
    {
        if (mainCamera == null) return;

        // Vẫn cập nhật vị trí nếu đang ở trong quá trình fade out (alpha > 0)
        if (!isPanelVisible && canvasGroup.alpha <= 0f) return;

        UpdatePositionAndScale();
    }

    private void UpdatePositionAndScale()
    {
        float distance = Vector3.Distance(currentTargetPosition, mainCamera.transform.position);
        float currentScale = keepConstantSize ? (distance * sizeMultiplier) : 1f;
        Vector3 scaledOffset = positionOffset * currentScale;
        transform.position = currentTargetPosition + (mainCamera.transform.rotation * scaledOffset);
        if (keepConstantSize)
        {
            transform.localScale = Vector3.one * currentScale;
        }
    }

    public void SetUpUI(GrabbableObjectSO grabbableObjectSO)
    {
        if(grabbableObjectSO == null)
        {
            displayNameText.text = "";
            weightText.text = "";
            return;
        }
        
        displayNameText.text = grabbableObjectSO.displayName;
        if(grabbableObjectSO is IngredientSO ingredientSO)
        {
            weightRect.gameObject.SetActive(true);
            weightText.text = $"{ingredientSO.initialAmount}g";
        }
        else
        {
            weightRect.gameObject.SetActive(false);
            weightText.text = "";
        }
    }

    public void ShowPanel(Vector3 position)
    {
        isPanelVisible = true;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        fadeTween?.Kill();
        fadeTween = canvasGroup.DOFade(1f, fadeDuration).SetEase(Ease.OutQuad);

        moveTween?.Kill();
        if (canvasGroup.alpha > 0f)
        {
            moveTween = DOTween.To(() => currentTargetPosition, x => currentTargetPosition = x, position, moveDuration).SetEase(Ease.OutQuad);
        }
        else
        {
            currentTargetPosition = position; 
            UpdatePositionAndScale();
        }
    }

    public void HidePanel()
    {
        if(!isPanelVisible) return;
        isPanelVisible = false;
        
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        fadeTween?.Kill();

        // Mờ dần đi
        fadeTween = canvasGroup.DOFade(0f, fadeDuration).SetEase(Ease.InQuad);
    }
    
    private void OnDestroy()
    {
        fadeTween?.Kill();
        moveTween?.Kill();
    }
}