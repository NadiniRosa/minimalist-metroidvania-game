using UnityEngine;

public class LionFish : Enemy
{
    [Header("Boss Arena")]
    [SerializeField] private bool useArena = true;
    private bool playerInArena = false;

    [Header("Return To Spawn")]
    [SerializeField] private float returnSpeedMultiplier = 0.75f; // slower return than chase
    [SerializeField] private float arriveDistance = 0.1f;

    [Header("Boss Follow")]
    [SerializeField] private float followTime = 5f;

    [Header("Thorns Attack")]
    [SerializeField] private float thornsLockTime = 1.2f;
    [SerializeField] private Collider2D bodyCollider;
    [SerializeField] private Vector2 thornsBoxSize = new Vector2(2f, 2f);
    [SerializeField] private float thornsCircleRadius = 1.0f;

    [Header("Bubble Spit")]
    [SerializeField] private float phase2HealthPercent = 0.5f;
    [SerializeField] private float bubbleInterval = 2f;
    [SerializeField] private GameObject bubblePrefab;
    [SerializeField] private Transform bubbleSpawnPoint;

    private bool bubbleLoopRunning = false;
    private bool inPhase2 = false;

    [Header("Health")]
    [SerializeField] private LionFishHealth bossHealthUI;
    private float maxHealth;

    [Header("Thanks UI")]
    [SerializeField] private CanvasGroup thanksCanvasGroup;
    [SerializeField] private float thanksFadeDuration = 0.25f;

    private Coroutine thanksFadeRoutine;

    [Header("Animator")]
    [SerializeField] private Animator animator;

    private float followTimer = 0f;
    private bool canMove = true;

    private BoxCollider2D box;
    private CircleCollider2D circle;

    private Vector2 normalBoxSize;
    private float normalCircleRadius;

    private Vector3 spawnPos;
    private bool returningToSpawn = false;

    [SerializeField] private Collider2D thornsHitbox;
    private bool thornsActive = false;

    protected override void Start()
    {
        base.Start();

        maxHealth = health;

        if (bossHealthUI != null)
            bossHealthUI.SetFill(health, maxHealth);

        spawnPos = transform.position;

        if (animator == null) animator = GetComponent<Animator>();
        if (bodyCollider == null) bodyCollider = GetComponent<Collider2D>();

        box = bodyCollider as BoxCollider2D;
        circle = bodyCollider as CircleCollider2D;

        if (box != null) normalBoxSize = box.size;
        if (circle != null) normalCircleRadius = circle.radius;
    }

    protected override void Update()
    {
        base.Update();
        if (isDead) return;
        if (player == null) return;

        if (useArena && !playerInArena)
        {
            CancelInvoke(nameof(EndThorns));
            SetThornsCollider(false);
            canMove = true;
            followTimer = 0f;

            ReturnToSpawn();
            return;
        }

        if (inPhase2)
        {
            ReturnToSpawn();

            if (!returningToSpawn)
            {
                rb.linearVelocity = Vector2.zero;
                transform.position = spawnPos;

                if ((!useArena || playerInArena) && !bubbleLoopRunning)
                    StartBubbleLoop();
            }

            return;
        }

        returningToSpawn = false;


        if (canMove)
        {
            followTimer += Time.deltaTime;
            if (followTimer >= followTime)
            {
                followTimer = 0f;
                StartThorns();
            }
        }

        if (canMove && !isRecoiling)
        {
            Vector2 dir = (player.transform.position - transform.position).normalized;
            rb.linearVelocity = dir * speed;
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    public override void EnemyHit(float _damageDone, Vector2 _hitDirection, float _hitForce)
    {
        base.EnemyHit(_damageDone, _hitDirection, _hitForce);

        if (bossHealthUI != null)
            bossHealthUI.SetFill(health, maxHealth);
        
        if (!inPhase2 && maxHealth > 0f && (health / maxHealth) <= phase2HealthPercent)
        {
            EnterPhase2();
        }
    }

    private void ReturnToSpawn()
    {
        returningToSpawn = true;

        Vector2 toSpawn = (spawnPos - transform.position);
        float dist = toSpawn.magnitude;

        if (dist <= arriveDistance)
        {
            rb.linearVelocity = Vector2.zero;
            transform.position = spawnPos;
            returningToSpawn = false;

            return;
        }

        if (!isRecoiling)
        {
            Vector2 dir = toSpawn.normalized;
            rb.linearVelocity = dir * (speed * returnSpeedMultiplier);
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    protected override void Attack()
    {
        // Only damage player during arena fight
        if (useArena && !playerInArena) return;

        base.Attack();
    }

    private void StartThorns()
    {
        if (animator != null)
            animator.SetTrigger("Thorns");

        canMove = false;
        rb.linearVelocity = Vector2.zero;

        CancelInvoke(nameof(EndThorns));
        Invoke(nameof(EndThorns), thornsLockTime);
    }

    private void EndThorns()
    {
        SetThornsCollider(false);
        canMove = true;
    }

    private void SetThornsCollider(bool thornsOn)
    {
        thornsActive = thornsOn;

        if (box != null)
            box.size = thornsOn ? thornsBoxSize : normalBoxSize;

        if (circle != null)
            circle.radius = thornsOn ? thornsCircleRadius : normalCircleRadius;

        if (thornsHitbox != null)
            thornsHitbox.enabled = thornsOn;
    }

    public void Thorns_Start() => SetThornsCollider(true);
    public void Thorns_End() => SetThornsCollider(false);

    public void SetPlayerInArena(bool inside)
    {
        playerInArena = inside;

        if (bossHealthUI != null)
        {
            if (inside)
            {
                bossHealthUI.Show();
                bossHealthUI.SetFill(health, maxHealth);
            }
            else
            {
                bossHealthUI.Hide();
            }
        }

        if (!inside)
        {
            canMove = true;
            followTimer = 0f;
            CancelInvoke(nameof(EndThorns));
            SetThornsCollider(false);
        }

        if (inPhase2)
        {
            if (inside && !returningToSpawn) StartBubbleLoop();
            else StopBubbleLoop();
        }
    }

    protected override void Die()
    {
        if (isDead) return;
        isDead = true;

        CancelInvoke();
        canMove = false;

        if (animator != null)
            animator.SetTrigger("Death");

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.simulated = false;
        }

        Collider2D[] cols = GetComponents<Collider2D>();
        for (int i = 0; i < cols.Length; i++)
            cols[i].enabled = false;
        
        bossHealthUI?.Hide();

        ShowThanksPanel();
    }

    private void ShowThanksPanel()
    {
        if (thanksCanvasGroup == null) return;

        if (thanksFadeRoutine != null)
            StopCoroutine(thanksFadeRoutine);

        thanksFadeRoutine = StartCoroutine(ShowThanksDelayed());
    }

    private System.Collections.IEnumerator ShowThanksDelayed()
    {
        yield return new WaitForSeconds(2f);

        yield return FadeThanksTo(1f);
    }

    private System.Collections.IEnumerator FadeThanksTo(float target)
    {
        float start = thanksCanvasGroup.alpha;
        float t = 0f;

        while (t < thanksFadeDuration)
        {
            t += Time.unscaledDeltaTime;
            thanksCanvasGroup.alpha = Mathf.Lerp(start, target, t / thanksFadeDuration);
            yield return null;
        }

        thanksCanvasGroup.alpha = target;
        thanksCanvasGroup.interactable = true;
        thanksCanvasGroup.blocksRaycasts = true;
        thanksFadeRoutine = null;

        Destroy(gameObject, 1f);
    }

    private void EnterPhase2()
    {
        inPhase2 = true;

        CancelInvoke(nameof(EndThorns));

        SetThornsCollider(false);
        canMove = true; 
        followTimer = 0f;

        StopBubbleLoop();

        returningToSpawn = true;
    }


    private void StartBubbleLoop()
    {
        if (bubbleLoopRunning) return;

        bubbleLoopRunning = true;
        CancelInvoke(nameof(DoBubble));
        InvokeRepeating(nameof(DoBubble), 0f, bubbleInterval);
    }

    private void StopBubbleLoop()
    {
        bubbleLoopRunning = false;
        CancelInvoke(nameof(DoBubble));
    }

    private void DoBubble()
    {
        if (isDead) return;
        if (useArena && !playerInArena) return;
        if (returningToSpawn) return;

        if (animator != null)
            animator.SetTrigger("Bubble");
    }

    public void BubbleSpawn()
    {
        if (isDead) return;
        if (useArena && !playerInArena) return;
        if (bubblePrefab == null) return;

        Vector3 pos = (bubbleSpawnPoint != null) ? bubbleSpawnPoint.position : transform.position;

        var b = Instantiate(bubblePrefab, pos, Quaternion.identity);

        Transform target = (PlayerController.Instance != null) ? PlayerController.Instance.transform : null;
        b.GetComponent<BouncingBubbleProjectile>()?.Initialize(target);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (isDead) return;
        if (!thornsActive) return;

        if (other.CompareTag("Player") &&
            !PlayerController.Instance.playerState.Invincible &&
            !PlayerController.Instance.IsDead)
        {
            Attack();
        }
    }

    public void ResetToPhase1()
    {
        isDead = false;
        health = maxHealth;
        inPhase2 = false;
        bubbleLoopRunning = false;

        CancelInvoke();
        StopBubbleLoop();

        canMove = true;
        followTimer = 0f;
        returningToSpawn = false;
        playerInArena = false;

        SetThornsCollider(false);

        if (rb != null)
        {
            rb.simulated = true;
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }

        foreach (var col in GetComponents<Collider2D>())
            col.enabled = true;

        transform.position = spawnPos;

        bossHealthUI?.Hide();
        bossHealthUI?.SetFill(health, maxHealth);

        if (thanksFadeRoutine != null) StopCoroutine(thanksFadeRoutine);
        if (thanksCanvasGroup != null)
        {
            thanksCanvasGroup.alpha = 0f;
            thanksCanvasGroup.interactable = false;
            thanksCanvasGroup.blocksRaycasts = false;
        }

        if (animator != null)
        {
            animator.Rebind();
            animator.Update(0f);
        }
    }
}
