using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Checkpoint UI")]
    [SerializeField] private CanvasGroup checkpointGroup;
    [SerializeField] private float fadeDuration = 0.3f;
    [SerializeField] private float visibleDuration = 1.2f;

    private Coroutine checkpointRoutine;

    [Header("Game Over UI")]
    [SerializeField] private CanvasGroup gameOverGroup;
    [SerializeField] private Button loadCheckpointButton;
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (checkpointGroup != null)
        {
            checkpointGroup.alpha = 0f;
            checkpointGroup.interactable = false;
            checkpointGroup.blocksRaycasts = false;
        }

        if (gameOverGroup != null)
        {
            gameOverGroup.alpha = 0f;
            gameOverGroup.interactable = false;
            gameOverGroup.blocksRaycasts = false;
        }

        RefreshGameOverButtons();
    }

    public void ShowCheckpointSaved()
    {
        if (checkpointGroup == null)
            return;

        if (checkpointRoutine != null)
            StopCoroutine(checkpointRoutine);

        checkpointGroup.gameObject.SetActive(true);
        checkpointRoutine = StartCoroutine(ShowCheckpointRoutine());
    }

    private IEnumerator ShowCheckpointRoutine()
    {
        yield return FadeCanvasGroup(checkpointGroup, 0f, 1f, fadeDuration);
        yield return new WaitForSeconds(visibleDuration);
        yield return FadeCanvasGroup(checkpointGroup, 1f, 0f, fadeDuration);
    }

    public void ShowGameOver()
    {
        if (gameOverGroup == null)
            return;

        RefreshGameOverButtons();

        gameOverGroup.gameObject.SetActive(true);
        gameOverGroup.alpha = 1f;
        gameOverGroup.interactable = true;
        gameOverGroup.blocksRaycasts = true;
    }

    public void HideGameOver()
    {
        if (gameOverGroup == null) return;

        gameOverGroup.alpha = 0f;
        gameOverGroup.interactable = false;
        gameOverGroup.blocksRaycasts = false;
        gameOverGroup.gameObject.SetActive(false);
    }

    public void OnClickLoadLastCheckpoint()
    {
        HideGameOver();
        GameManager.Instance.LoadFromCheckpoint();
    }

    public void OnClickQuitGame()
    {
        if (!string.IsNullOrEmpty(mainMenuSceneName))
            SceneManager.LoadScene(mainMenuSceneName);
        else
            Application.Quit();
    }

    public void RefreshGameOverButtons()
    {
        if (loadCheckpointButton == null) return;

        bool canLoad = (GameManager.Instance != null && GameManager.Instance.HasCheckpoint);
        loadCheckpointButton.interactable = canLoad;
    }

    private IEnumerator FadeCanvasGroup(CanvasGroup group, float from, float to, float duration)
    {
        float time = 0f;
        group.alpha = from;
        group.interactable = false;
        group.blocksRaycasts = false;

        while (time < duration)
        {
            time += Time.deltaTime;
            group.alpha = Mathf.Lerp(from, to, time / duration);
            yield return null;
        }

        group.alpha = to;
    }
}
