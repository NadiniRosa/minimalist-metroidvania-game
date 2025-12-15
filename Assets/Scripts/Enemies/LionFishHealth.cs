using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LionFishHealth : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Image fillImage;

    [Header("Fade")]
    [SerializeField] private float fadeDuration = 0.25f;

    private Coroutine fadeRoutine;

    private void Awake()
    {
        if (canvasGroup == null) canvasGroup = GetComponent<CanvasGroup>();

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
    }

    public void Show()
    {
        StartFade(1f);
    }

    public void Hide()
    {
        StartFade(0f);
    }

    public void SetFill(float current, float max)
    {
        if (fillImage == null) return;

        float pct = (max <= 0f) ? 0f : Mathf.Clamp01(current / max);
        fillImage.fillAmount = pct;
    }

    private void StartFade(float target)
    {
        if (fadeRoutine != null) StopCoroutine(fadeRoutine);
        fadeRoutine = StartCoroutine(FadeTo(target));
    }

    private IEnumerator FadeTo(float target)
    {
        float start = canvasGroup.alpha;
        float t = 0f;

        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Lerp(start, target, t / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = target;
        fadeRoutine = null;
    }
}
