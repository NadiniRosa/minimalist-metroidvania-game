using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button startGameButton;
    [SerializeField] private Button creditsButton;

    [SerializeField] private CanvasGroup creditsGroup;
    [SerializeField] private Button backButton;
    private bool isCreditsEnabled = false;

    private void Awake()
    {
        startGameButton.onClick.AddListener(StartGame);
        creditsButton.onClick.AddListener(ShowCredits);
        backButton.onClick.AddListener(ShowCredits);
    }

    private void StartGame()
    {
        SceneManager.LoadScene("Testing");
    }
    
    private void ShowCredits()
    {
        isCreditsEnabled = !isCreditsEnabled;

        if (isCreditsEnabled)
        {
            creditsGroup.alpha = 1f;
            creditsGroup.interactable = true;
            creditsGroup.blocksRaycasts = true;
        }
        else
        {
            creditsGroup.alpha = 0f;
            creditsGroup.interactable = false;
            creditsGroup.blocksRaycasts = false;
        }
    }
}
