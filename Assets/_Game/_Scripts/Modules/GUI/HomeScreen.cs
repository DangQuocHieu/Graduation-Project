using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HomeScreen : MonoBehaviour
{
    public Button startButton;
    public Button settingButton;

    void OnEnable()
    {
        startButton.onClick.AddListener(OnStartButtonClicked);
    }

    void OnDisable()
    {
        startButton.onClick.RemoveListener(OnStartButtonClicked);
    }

    private void OnStartButtonClicked()
    {
        SceneTransitionManager.Instance.LoadScene("GameplayScene");
    }
}
