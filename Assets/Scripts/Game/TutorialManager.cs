using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private GameObject panelRoot;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Button closeButton;

    [Header("Fade Settings")]
    [SerializeField] private float fadeDuration = 0.25f;

    public bool TutorialOpen { get; private set; }

    private Coroutine fadeRoutine;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }


        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        TutorialOpen = false;

        if (closeButton != null)
            closeButton.onClick.AddListener(CloseTutorial);

        if (panelRoot != null)
            panelRoot.SetActive(false);

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
    }

    public void ShowTutorial(Sprite sprite)
    {
        if (panelRoot == null || canvasGroup == null || backgroundImage == null)
        {
            Debug.LogWarning("TutorialManager: panelRoot / canvasGroup / backgroundImage not assigned!");
            return;
        }

        backgroundImage.sprite = sprite;
        backgroundImage.enabled = (sprite != null);

        panelRoot.SetActive(true);
        TutorialOpen = true;

        StartFade(1f, makeInteractiveAtEnd: true, disableRootAtEnd: false);
    }

    public void CloseTutorial()
    {
        if (panelRoot == null || canvasGroup == null) return;
        if (!TutorialOpen) return;

        if (AudioService.Instance != null)
            AudioService.Instance.PlaySFX(SFXType.Button);

        TutorialOpen = false;

        StartFade(0f, makeInteractiveAtEnd: false, disableRootAtEnd: true);
    }

    private void StartFade(float targetAlpha, bool makeInteractiveAtEnd, bool disableRootAtEnd)
    {
        if (fadeRoutine != null)
            StopCoroutine(fadeRoutine);

        fadeRoutine = StartCoroutine(FadeRoutine(targetAlpha, makeInteractiveAtEnd, disableRootAtEnd));
    }

    private IEnumerator FadeRoutine(float targetAlpha, bool makeInteractiveAtEnd, bool disableRootAtEnd)
    {
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        float startAlpha = canvasGroup.alpha;
        float t = 0f;

        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, t / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = targetAlpha;
        canvasGroup.interactable = makeInteractiveAtEnd;
        canvasGroup.blocksRaycasts = makeInteractiveAtEnd;

        fadeRoutine = null;

        if (disableRootAtEnd && panelRoot != null)
            panelRoot.SetActive(false);
    }
}
