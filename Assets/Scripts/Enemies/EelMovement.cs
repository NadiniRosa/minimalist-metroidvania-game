using UnityEngine;

public class EelEnemy : Enemy
{
    [Header("Points")]
    [SerializeField] private Transform pointA;
    [SerializeField] private Transform pointB;

    [Header("Movement")]
    [SerializeField] private float maxSpeed = 10f;
    [SerializeField] private float acceleration = 20f;

    private Vector3 nextPosition;
    private Vector3 lastPosition;
    private float currentSpeed;

    public float CurrentVelocityX { get; private set; }

    protected override void Start()
    {
        base.Start();

        transform.position = pointA.position;
        nextPosition = pointB.position;

        ForceFaceLeft();
        FlipTowards(nextPosition);

        lastPosition = transform.position;
        currentSpeed = (acceleration <= 0f) ? maxSpeed : 0f;
    }

    protected override void Update()
    {
        if (acceleration > 0f)
            currentSpeed = Mathf.MoveTowards(currentSpeed, maxSpeed, acceleration * Time.deltaTime);
        else
            currentSpeed = maxSpeed;

        transform.position = Vector3.MoveTowards(transform.position, nextPosition, currentSpeed * Time.deltaTime);

        CurrentVelocityX = (transform.position.x - lastPosition.x) / Time.deltaTime;
        lastPosition = transform.position;

        if (transform.position == nextPosition)
        {
            nextPosition = (nextPosition == pointA.position) ? pointB.position : pointA.position;
            FlipTowards(nextPosition);

            if (acceleration > 0f) currentSpeed = 0f;
        }
    }

    public virtual void EnemyHit()
    {

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

    private void ForceFaceLeft()
    {
        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x);
        transform.localScale = scale;
    }

    protected override void Attack()
    {
        base.Attack();

        if (AudioService.Instance != null)
            AudioService.Instance.PlaySFX(SFXType.EelBiting);
    }
}
