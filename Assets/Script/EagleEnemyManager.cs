using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EagleEnemyManager : EnemyBase
{
    Rigidbody2D rigidbody2D;

    [SerializeField]
    Transform player;
    [SerializeField]
    GameObject deathEffect;

    Animator animator;
    public float findDistance = 5f;
    public float moveSpeed = 2f;
    public int damageCount = 0;
    bool isHurt = false;
    bool isInvincible = false;
    SpriteRenderer sr;
    Vector2 attackDir;
    bool isDashing = false;

    void Start()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (isHurt) return;
        if (isDashing) return;

        float distance = Vector2.Distance(transform.position, player.position);


        if (distance < findDistance)
        {
            ChasePlayer();
        }
        else
        {
            animator.SetBool("isAttack", false);
            Patrol();
        }
    }

    void ChasePlayer()
    {
        if (!isDashing)
        {
            attackDir = (player.position - transform.position).normalized;
            isDashing = true;

            animator.SetBool("isAttack", true);
            StartCoroutine(DashCoroutine());
        }
    }

    void Patrol()
    {
        float x = Mathf.Sin(Time.time) * moveSpeed;
        float y = Mathf.Cos(Time.time) * moveSpeed;

        if (x > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else if (x < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);

        }

        rigidbody2D.velocity = new Vector2(x, y);
    }

    public override void DestroyEnemy()
    {
        if (isInvincible) return; 

        damageCount++;
        if (damageCount < 3)
        {
            StartCoroutine(HurtCoroutine());
            return;
        }
        //Instantiate:プレハブを発生させる（プレハブ,発生する位置,発生する角度）
        Instantiate(deathEffect, this.transform.position, this.transform.rotation);
        Destroy(this.gameObject);
    }

    IEnumerator HurtCoroutine()
    {
        isHurt = true;
        isInvincible = true;

        animator.SetBool("isAttack", false);
        animator.SetTrigger("isDamage");

        //点滅処理
        for(int i = 0; i < 5; i++)
        {
            sr.enabled = false;
            yield return new WaitForSeconds(0.1f);
            sr.enabled = true;
            yield return new WaitForSeconds(0.1f);
        }

        animator.SetBool("isHurt", false);
        isHurt = false;
        isInvincible = false;

    }

    IEnumerator DashCoroutine()
    {
        //突進時間
        float dashtime = 1.0f;
        float timer = 0;

        while(timer < dashtime)
        {
            rigidbody2D.velocity = attackDir * moveSpeed * 3f;
            timer += Time.deltaTime;
            yield return null;
        }

        //終了
        isDashing = false;
        rigidbody2D.velocity = Vector2.zero;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDashing)
        {
            isDashing = false;
            rigidbody2D.velocity = Vector2.zero;
        }
    }
}
