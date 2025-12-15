using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Player Abilities Unlocked")]
    public bool DashUnlocked = false;
    public bool DoubleJumpUnlocked = false;
    public bool LightUnlocked = false;
    
    [Header("Runes")]
    public int Runes = 0;
    private int savedRunes;

    private bool hasSave = false;
    public bool HasCheckpoint => hasSave;

    private Vector3 savedPosition;
    
    private int savedHealth;
    private int savedMaxHealth;

    private bool savedDashUnlocked;
    private bool savedDoubleJumpUnlocked;
    private bool savedLightUnlocked;

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

    public void PlayerDied()
    {
        Debug.Log("Player has died.");

        if (UIManager.Instance != null)
            UIManager.Instance.ShowGameOver();
    }
    public void SaveCheckpoint(Transform playerTransform)
    {
        if (playerTransform == null || PlayerController.Instance == null)
            return;

        hasSave = true;

        savedPosition = playerTransform.position;
        savedHealth = PlayerController.Instance.health;
        savedMaxHealth = PlayerController.Instance.maxHealth;

        savedDashUnlocked = DashUnlocked;
        savedDoubleJumpUnlocked = DoubleJumpUnlocked;
        savedLightUnlocked = LightUnlocked;
        savedRunes = Runes;

        Debug.Log("Checkpoint saved at position " + savedPosition);

        if (UIManager.Instance != null)
            UIManager.Instance.RefreshGameOverButtons();

        if (PlayerHealthHUD.Instance != null)
            PlayerHealthHUD.Instance.UpdateHealthBars();
    }

    public void LoadFromCheckpoint()
    {
        if (!hasSave || PlayerController.Instance == null)
        {
            Debug.Log("No checkpoint to load.");
            return;
        }

        DashUnlocked = savedDashUnlocked;
        DoubleJumpUnlocked = savedDoubleJumpUnlocked;
        LightUnlocked = savedLightUnlocked;
        Runes = savedRunes;

        PlayerController.Instance.RespawnFromCheckpoint(savedPosition, savedHealth, savedMaxHealth);

        foreach (var boss in FindObjectsOfType<LionFish>(true)) 
            boss.ResetToPhase1();
    }
}
