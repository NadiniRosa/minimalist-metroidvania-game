using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AreaID
{
    Area01,
    Area02,
    Area03,
    Area04,
    Area05
}

[Serializable]
public class BackgroundAreaData
{
    public AreaID area;
    public Sprite backgroundSprite;
}

public class BackgroundManager : MonoBehaviour
{
    public static BackgroundManager Instance;

    [SerializeField] private SpriteRenderer mainRenderer;
    [SerializeField] private SpriteRenderer fadeRenderer;

    [SerializeField] private float fadeDuration = 1f;

    [SerializeField] private List<BackgroundAreaData> backgroundList = new List<BackgroundAreaData>();

    private Dictionary<AreaID, Sprite> backgroundDict;

    private bool isTransitioning = false;
    private bool hasCurrentArea = false;

    private AreaID currentArea;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        BuildDictionary();
    }

    private void Start()
    {
        if (mainRenderer != null)
        {
            Color c = mainRenderer.color;
            c.a = 1f;
            mainRenderer.color = c;
        }

        if (fadeRenderer != null)
        {
            Color c = fadeRenderer.color;
            c.a = 0f;
            fadeRenderer.color = c;
        }
    }

    private void BuildDictionary()
    {
        backgroundDict = new Dictionary<AreaID, Sprite>();

        foreach (var data in backgroundList)
        {
            if (data == null) continue;
            if (backgroundDict.ContainsKey(data.area)) continue;

            backgroundDict.Add(data.area, data.backgroundSprite);
        }
    }

    public void SetBackground(AreaID area)
    {
        if (mainRenderer == null || fadeRenderer == null || backgroundDict == null) return;
        if (!backgroundDict.TryGetValue(area, out Sprite sprite) || sprite == null) return;

        if (isTransitioning) return;

        if (hasCurrentArea && area.Equals(currentArea) && mainRenderer.sprite == sprite) return;

        StartCoroutine(FadeToArea(area, sprite));
    }

    private IEnumerator FadeToArea(AreaID area, Sprite newSprite)
    {
        isTransitioning = true;

        hasCurrentArea = true;
        currentArea = area;

        Color mainStart = mainRenderer.color;
        Color fadeStart = fadeRenderer.color;

        mainStart.a = 1f;
        fadeStart.a = 0f;

        mainRenderer.color = mainStart;

        fadeRenderer.sprite = newSprite;
        fadeRenderer.color = fadeStart;

        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / fadeDuration);

            float mainAlpha = Mathf.Lerp(1f, 0f, t);
            float fadeAlpha = Mathf.Lerp(0f, 1f, t);

            mainRenderer.color = new Color(mainStart.r, mainStart.g, mainStart.b, mainAlpha);
            fadeRenderer.color = new Color(fadeStart.r, fadeStart.g, fadeStart.b, fadeAlpha);

            yield return null;
        }

        mainRenderer.sprite = newSprite;

        Color mainFinal = mainRenderer.color;
        mainFinal.a = 1f;
        mainRenderer.color = mainFinal;

        Color fadeFinal = fadeRenderer.color;
        fadeFinal.a = 0f;
        fadeRenderer.color = fadeFinal;

        isTransitioning = false;
    }
}
