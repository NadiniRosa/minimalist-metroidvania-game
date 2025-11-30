using UnityEngine;

public class BlockMover : MonoBehaviour
{
    [Header("Mover Horizontalmente?")]
    public bool moveHorizontal = false;
    public float horizontalDistance = 2f;
    public float horizontalSpeed = 2f;

    [Header("Mover Verticalmente?")]
    public bool moveVertical = false;
    public float verticalDistance = 2f;
    public float verticalSpeed = 2f;

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        float x = transform.position.x;
        float y = transform.position.y;

        if (moveHorizontal)
        {
            x = startPos.x + Mathf.Sin(Time.time * horizontalSpeed) * horizontalDistance;
        }

        if (moveVertical)
        {
            y = startPos.y + Mathf.Sin(Time.time * verticalSpeed) * verticalDistance;
        }

        transform.position = new Vector3(x, y, transform.position.z);
    }
}
