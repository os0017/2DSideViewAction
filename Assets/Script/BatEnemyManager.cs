using UnityEngine;

public class BatEnemyManager : EnemyBase
{
    Rigidbody2D rigidbody2D;

    [SerializeField]
    Transform player;
    [SerializeField]
    GameObject deathEffect;

    public float findDistance = 5f;
    public float moveSpeed = 2f;

    void Start()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        float distance = Vector2.Distance(transform.position, player.position);

        if (distance < findDistance)
        {
            ChasePlayer();
        }
        else
        {
            Patrol();
        }
    }

    void ChasePlayer()
    {
        Vector2 dir = (player.position - transform.position).normalized;
        rigidbody2D.velocity = dir * moveSpeed;
    }

    void Patrol()
    {
        float x = Mathf.Sin(Time.time) * moveSpeed;
        float y = Mathf.Cos(Time.time) * moveSpeed;

        rigidbody2D.velocity = new Vector2(x, y);
    }

    public override void DestroyEnemy()
    {
        //Instantiate:プレハブを発生させる（プレハブ,発生する位置,発生する角度）
        Instantiate(deathEffect, this.transform.position, this.transform.rotation);
        Destroy(this.gameObject);
    }
}