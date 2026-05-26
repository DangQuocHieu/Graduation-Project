
using UnityEngine;
using UnityEngine.UI;

public class TutorialSubScreen : MonoBehaviour
{
    public Button acceptButton;
    public bool acceptButtonClicked = false;
    public RectTransform overlay;

    void OnEnable()
    {
        acceptButton.onClick.AddListener(OnAcceptButtonClicked);
    }

    void OnDisable()
    {
        acceptButton.onClick.RemoveListener(OnAcceptButtonClicked);
    }

    public void ToggleScreen(bool isActive)
    {
        overlay.gameObject.SetActive(isActive);
        gameObject.SetActive(isActive);
    }

    private void OnAcceptButtonClicked()
    {
        acceptButtonClicked = true;
        ToggleScreen(false);
    }
}
