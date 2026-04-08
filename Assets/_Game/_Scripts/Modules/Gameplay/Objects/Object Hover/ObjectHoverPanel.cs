using UnityEngine;
using UnityEngine.UI;
using TMPro;

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

    private Camera mainCamera;
    private Vector3 currentTargetPosition;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void LateUpdate()
    {
        if (!isPanelVisible || mainCamera == null) return;

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
            weightText.text = $"{ingredientSO.maxWeight}g";
        }
        else
        {
            weightRect.gameObject.SetActive(false);
            weightText.text = "";
        }
    }

    public void ShowPanel(Vector3 position)
    {
        currentTargetPosition = position; 
        
        isPanelVisible = true;
        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    public void HidePanel()
    {
        if(!isPanelVisible) return;
        isPanelVisible = false;
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }
}