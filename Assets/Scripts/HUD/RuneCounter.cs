using UnityEngine;
using TMPro;

public class RuneCounter : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI qtyText;
    [SerializeField] private string prefix = "x";

    private void Start()
    {
        Refresh();
    }

    public void Refresh()
    {
        if (GameManager.Instance == null) return;
        if (qtyText == null) return;

        qtyText.text = $"{prefix}{GameManager.Instance.Runes}";
    }

    public void Add(int amount)
    {
        if (GameManager.Instance == null) return;

        GameManager.Instance.Runes = Mathf.Max(0, GameManager.Instance.Runes + amount);
        Refresh();
    }

    public void Set(int value)
    {
        if (GameManager.Instance == null) return;

        GameManager.Instance.Runes = Mathf.Max(0, value);
        Refresh();
    }

    public int Get()
    {
        if (GameManager.Instance == null) return 0;
        return GameManager.Instance.Runes;
    }
}
