using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightGroupUnlockToggle : MonoBehaviour
{
    [Header("Search Root")]
    [SerializeField] private Transform root;

    private Light2D[] lights;
    private bool lastUnlocked;

    private void Awake()
    {
        CacheLights();
    }

    private void Start()
    {
        bool unlocked = GameManager.Instance != null && GameManager.Instance.LightUnlocked;
        Apply(unlocked);
    }

    private void Update()
    {
        bool unlocked = GameManager.Instance != null && GameManager.Instance.LightUnlocked;

        if (unlocked != lastUnlocked)
            Apply(unlocked);
    }

    private void CacheLights()
    {
        Transform Root = root != null ? root : transform;

        lights = Root.GetComponentsInChildren<Light2D>(true);

        if (lights == null || lights.Length == 0)
            Debug.LogWarning($"LightGroupUnlockToggle: No Light2D found under '{root.name}'.");
    }

    private void Apply(bool unlocked)
    {
        lastUnlocked = unlocked;

        if (lights == null) return;

        for (int i = 0; i < lights.Length; i++)
        {
            if (lights[i] != null)
                lights[i].enabled = unlocked;
        }
    }
}
