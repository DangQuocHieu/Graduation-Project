using DQHieu.Framework;
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
        if (!DataManager.Instance.playerData.TutorialCompleted)
        {
            SceneTransitionManager.Instance.LoadScene("Tutorial Scene");
        }
        else
        {
            SceneTransitionManager.Instance.LoadScene("GameplayScene");
        }

    }
}
