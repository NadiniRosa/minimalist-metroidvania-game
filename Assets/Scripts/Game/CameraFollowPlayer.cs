using UnityEngine;

public class CameraFollowPlayer : MonoBehaviour
{
    [SerializeField] private float followSpeed = 0.1f;
    [SerializeField] private Vector3 offset;

    [Header("Look Up/Down Settings")]
    [SerializeField] private float lookAmount = 2f;
    [SerializeField] private float lerpSpeed = 5f;

    private float currentLookOffsetY = 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float v = Input.GetAxisRaw("Vertical");
        float targetLookOffsetY = 0f;

        if (v > 0.1f)
            targetLookOffsetY = lookAmount;
        else if (v < -0.1f)
            targetLookOffsetY = -lookAmount;

        currentLookOffsetY = Mathf.Lerp(
            currentLookOffsetY,
            targetLookOffsetY,
            lerpSpeed * Time.deltaTime
        );

        Vector3 targetPos = PlayerController.Instance.transform.position + offset + new Vector3(0f, currentLookOffsetY, 0f);

        transform.position = Vector3.Lerp(transform.position, targetPos, followSpeed);
    }
}
