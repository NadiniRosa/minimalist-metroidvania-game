using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField] private Transform pointA;
    [SerializeField] private Transform pointB;
    [SerializeField] private float speed = 2f;

    private Vector3 nextPosition;
    private Vector3 lastPosition;

    public float CurrentVelocityX { get; private set; }

    private void Start()
    {
        nextPosition = pointB.position;
        lastPosition = transform.position;
    }

    private void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, nextPosition, speed * Time.deltaTime);

        CurrentVelocityX = (transform.position.x - lastPosition.x) / Time.deltaTime;
        lastPosition = transform.position;

        if (transform.position == nextPosition)
        {
            nextPosition = (nextPosition == pointA.position) ? pointB.position : pointA.position;
            FlipTowards(nextPosition);
        }
    }

    private void FlipTowards(Vector3 targetPos)
    {
        Vector3 scale = transform.localScale;

        if (targetPos.x > transform.position.x)
            scale.x = -Mathf.Abs(scale.x);
        else if (targetPos.x < transform.position.x)
            scale.x = Mathf.Abs(scale.x);

        transform.localScale = scale;
    }
}
