using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Player Abilities Unlocked")]
    public bool DashUnlocked = false;
    public bool DoubleJumpUnlocked = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;

        DontDestroyOnLoad(gameObject);
    }

    public void PlayerDied()
    {
        Debug.Log("Player has died.");
    }
}
