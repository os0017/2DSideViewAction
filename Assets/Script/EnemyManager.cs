using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class EnemyManager : EnemyBase
{
    [SerializeField]
    LayerMask blocklayerMask;
    [SerializeField]
    GameObject deathEffect;

    public enum Directiontype
    {
        Stop,
        Right,
        Left
    }

    Directiontype directiontype = Directiontype.Stop;

    Rigidbody2D rigidbody2D;
    float speed;
    private void Start()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        //右へ
        directiontype = Directiontype.Right;
        //directiontype = Directiontype.Stop;
    }

    private void Update()
    {
        if (!IsGround())
        {
            //方向転換する
            ChangeDirection();
        }
    }

    private void FixedUpdate()
    {
        switch (directiontype)
        {
            case Directiontype.Stop:
                speed = 0;
                break;

            case Directiontype.Right:
                speed = 3;
                transform.localScale = new Vector3(1, 1, 1);
                break;

            case Directiontype.Left:
                speed = -3;
                transform.localScale = new Vector3(-1, 1, 1);
                break;
        }
        rigidbody2D.velocity = new Vector2(speed, rigidbody2D.velocity.y);
    }
    bool IsGround()
    {
        Vector3 startVec = transform.position + transform.right * 0.5f * transform.localScale.x;
        Vector3 endVec = startVec - transform.up * 0.5f;
        Debug.DrawLine(startVec, endVec);
        //Debug.Log(Physics2D.Linecast(startVec, endVec, blocklayerMask));
        //RaycastHit2D hit = Physics2D.Linecast(startVec, endVec, blocklayerMask);
        //Debug.Log("ヒットしたオブジェクト: " + hit.collider.name);

        return Physics2D.Linecast(startVec, endVec, blocklayerMask);
    }
    void ChangeDirection()
    {
        if (directiontype == Directiontype.Right)
        {
            directiontype = Directiontype.Left;
        } 
        else if (directiontype == Directiontype.Left)
        {
            directiontype = Directiontype.Right;

        }
    }

    public override void DestroyEnemy()
    {
        //Instantiate:プレハブを発生させる（プレハブ,発生する位置,発生する角度）
        Instantiate(deathEffect, this.transform.position, this.transform.rotation);
        Destroy(this.gameObject);
    }
}
