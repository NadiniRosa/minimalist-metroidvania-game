using UnityEngine;

public class BaseEnemyController : MonoBehaviour
{
    [SerializeField] private float health;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (health <= 0)
            Destroy(gameObject);
    }

    public void EnemyHit(float damage)
    {
        health -= damage;
    }
}
