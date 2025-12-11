using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private GameObject panelRoot;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Button closeButton;

    public bool TutorialOpen { get; private set; }

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
        if (panelRoot != null)
            panelRoot.SetActive(false);

        TutorialOpen = false;

        if (closeButton != null)
            closeButton.onClick.AddListener(CloseTutorial);
    }

    public void ShowTutorial(Sprite sprite)
    {
        if (panelRoot == null || backgroundImage == null)
        {
            Debug.LogWarning("TutorialManager: panelRoot or backgroundImage not assigned!");
            return;
        }

        backgroundImage.sprite = sprite;
        backgroundImage.enabled = (sprite != null);

        panelRoot.SetActive(true);

        TutorialOpen = true;

        Debug.Log("TutorialManager: ShowTutorial called with sprite: "
                  + (sprite != null ? sprite.name : "NULL"));
    }

    public void CloseTutorial()
    {
        if (panelRoot == null)
            return;

        panelRoot.SetActive(false);

        TutorialOpen = false;

        Debug.Log("TutorialManager: CloseTutorial called");
    }
}
