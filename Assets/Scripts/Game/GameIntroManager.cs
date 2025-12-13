using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameIntroManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject panelRoot;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Button continueButton;

    [Header("Intro Slides")]
    [SerializeField] private Sprite[] slides;

    [Header("Fade Settings (Image Only)")]
    [SerializeField] private float fadeDuration = 0.25f;

    private int index = 0;
    private Coroutine transitionRoutine;

    private PlayerController player;
    private Rigidbody2D playerRb;
    private bool introOpen = false;

    private void Start()
    {
        if (continueButton != null)
            continueButton.onClick.AddListener(Continue);

        if (slides == null || slides.Length == 0)
        {
            Debug.LogWarning("GameIntroManager: No slides assigned.");
            if (panelRoot != null) panelRoot.SetActive(false);
            return;
        }

        OpenIntro();

        index = 0;
        ShowSlide(index);

        SetImageAlpha(0f);
        StartCoroutine(FadeImageTo(1f));
    }

    private void OpenIntro()
    {
        introOpen = true;

        if (panelRoot != null)
            panelRoot.SetActive(true);

        player = PlayerController.Instance != null ? PlayerController.Instance : FindFirstObjectByType<PlayerController>();
        if (player != null)
            playerRb = player.GetComponent<Rigidbody2D>();

        if (player != null) player.enabled = false;
        if (playerRb != null)
        {
            playerRb.linearVelocity = Vector2.zero;
            playerRb.angularVelocity = 0f;
        }
    }

    private void CloseIntro()
    {
        introOpen = false;

        if (panelRoot != null)
            panelRoot.SetActive(false);
    
        if (player != null) player.enabled = true;
    }

    private void ShowSlide(int i)
    {
        if (backgroundImage == null) return;

        backgroundImage.sprite = slides[i];
        backgroundImage.enabled = true;
    }

    public void Continue()
    {
        if (!introOpen) return;
        if (slides == null || slides.Length == 0) return;
        if (transitionRoutine != null) return;

        if (AudioService.Instance != null)
            AudioService.Instance.PlaySFX(SFXType.Button);

        if (index >= slides.Length - 1)
        {
            transitionRoutine = StartCoroutine(FadeOutAndClose());
            return;
        }

        transitionRoutine = StartCoroutine(TransitionToNextSlide());
    }

    private IEnumerator TransitionToNextSlide()
    {
        SetInteractive(false);

        yield return FadeImageTo(0f);

        index++;
        ShowSlide(index);

        yield return FadeImageTo(1f);

        SetInteractive(true);
        transitionRoutine = null;
    }

    private IEnumerator FadeOutAndClose()
    {
        SetInteractive(false);

        yield return FadeImageTo(0f);

        CloseIntro();

        SetInteractive(false);
        transitionRoutine = null;
    }

    private IEnumerator FadeImageTo(float targetAlpha)
    {
        if (backgroundImage == null) yield break;

        float startAlpha = backgroundImage.color.a;
        float t = 0f;

        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            float a = Mathf.Lerp(startAlpha, targetAlpha, t / fadeDuration);
            SetImageAlpha(a);
            yield return null;
        }

        SetImageAlpha(targetAlpha);
    }

    private void SetImageAlpha(float a)
    {
        if (backgroundImage == null) return;
        Color c = backgroundImage.color;
        c.a = a;
        backgroundImage.color = c;
    }

    private void SetInteractive(bool state)
    {
        if (canvasGroup == null) return;
        canvasGroup.interactable = state;
        canvasGroup.blocksRaycasts = state;
    }
}
