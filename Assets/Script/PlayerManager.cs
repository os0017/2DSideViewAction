using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField]
    LayerMask blocklayerMask;
    [SerializeField]
    GameManager gameManager;

    public enum Directiontype
    {
        Stop,
        Right,
        Left
    }

    Rigidbody2D rigidbody2D;
    float speed;
    Animator animator;
    //SEの設定
    public AudioClip getItemSE;
    public AudioClip jumpSE;
    public AudioClip stampSE;
    AudioSource audioSource;

    public float Jumppower = 400;
    bool isDead = false;
    //登り判定
    bool isClimbing = false;
    bool canClimb = false;
    public float climbSpeed = 3f;
    private void Start()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    Directiontype directiontype = Directiontype.Stop;

    // Update is called once per frame
    void Update()
    {
        if(isDead)
        {
            Debug.Log("リターン");
            return;
        }
        float x = Input.GetAxis("Horizontal");
        //Mathf.Abs(x):引数に絶対値を付ける
        animator.SetFloat("speed",Mathf.Abs(x));

        if (x == 0)
        {
            //止まっている
            directiontype = Directiontype.Stop;
        }
        else if(x > 0)
        {
            //右へ
            directiontype = Directiontype.Right;
        }
        else if (x < 0)
        {
            //左へ
            directiontype = Directiontype.Left;
        }
        //spaceでジャンプ
        //GetKeyDown:押し込んだ時に一回だけ反応する
        if (IsGroung() )
        {
            //地上にいる場合ジャンプ直後に地面との当たり判定を消す
            animator.SetBool("IsJumping", false);


            if (Input.GetKeyDown("space"))
            {
                if (isClimbing)
                {
                    isClimbing = false;
                }
                Jump();
            }
        }
        else
        {
            //空中にいる場合trueにしてジャンプアニメーションを表示する
            animator.SetBool("IsJumping", true);
        }

        // ↑キーでツタにつかまる
        if (canClimb && Input.GetAxis("Vertical") > 0)
        {
            isClimbing = true;
            PlayerClimb();
        }

        //ツタと重なっている場合、登りアクションを実行化可能にする
        if (isClimbing)
        {
            float v = Input.GetAxis("Vertical");
            rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, v * climbSpeed);
        }
    }

    private void FixedUpdate()
    {
        if (isDead)
        {
            Debug.Log("リターン");
            return;
        }
        switch (directiontype)
        {
            case Directiontype.Stop:
                speed = 0;
                break;

            case Directiontype.Right:
                speed = 3;
                transform.localScale = new Vector3(1,1,1);
                break;

            case Directiontype.Left:
                speed = -3;
                transform.localScale = new Vector3(-1, 1, 1);
                break;
        }
        rigidbody2D.velocity = new Vector2(speed,rigidbody2D.velocity.y);
    }

    void Jump()
    {

        rigidbody2D.gravityScale = 2;
        audioSource.PlayOneShot(jumpSE);
        //上方向に力を加える
        rigidbody2D.AddForce(Vector2.up * Jumppower);
        animator.SetBool("IsJumping", true);
    }

    bool IsGroung()
    {
        //始点と終点を作成
        //Vector3.right:Playerの中心から左側に位置を引き算する
        //Vector3 leftStartPoint = transform.position - Vector3.right * 0.2f;
        //Vector3 rightStartPoint = transform.position + Vector3.right * 0.2f;
        Vector3 leftStartPoint = transform.position - transform.right * 0.3f + Vector3.up * 0.1f;
        Vector3 rightStartPoint = transform.position + transform.right * 0.3f + Vector3.up * 0.1f;
        Vector3 endPoint = transform.position - Vector3.up * 0.1f;
        Debug.DrawLine(leftStartPoint, endPoint);
        Debug.DrawLine(rightStartPoint, endPoint);
        //Debug.Log(Physics2D.Linecast(leftStartPoint, endPoint, blocklayerMask) || Physics2D.Linecast(rightStartPoint, endPoint, blocklayerMask));
        return  Physics2D.Linecast(leftStartPoint, endPoint, blocklayerMask) ||
                Physics2D.Linecast(rightStartPoint, endPoint, blocklayerMask);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isDead)
        {
            Debug.Log("リターン");
            return;
        }
        if (collision.gameObject.tag == "Trap")
        {
            PlayerDie();
        }
        else if(collision.gameObject.tag == "Finish")
        {
            gameManager.GameClear();
        }
        else if (collision.gameObject.tag == "Item")
        {
            audioSource.PlayOneShot(getItemSE);
            collision.gameObject.GetComponent<ItemManager>().GetItem();
        }
        else if(collision.gameObject.tag == "Enemy")
        {
            EnemyBase enemy = collision.gameObject.GetComponent<EnemyBase>();
            //上から踏んだ場合撃破
            if(this.transform.position.y + 0.2f > enemy.transform.position.y)
            {
                rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x,0);
                Jump();
                audioSource.PlayOneShot(stampSE);
                enemy.DestroyEnemy();
            }
            //横からぶつかった場合、ゲームオーバー
            else
            {
                PlayerDie();
            }
        }
        else if(collision.gameObject.tag == "Climb")
        {
            canClimb = true;            
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Climb"))
        {
            canClimb = false;
            isClimbing = false;

            rigidbody2D.gravityScale = 2;
            animator.SetBool("IsClimbing", false); 
        }
    }

    void PlayerDie()
    {
        isDead = true;
        rigidbody2D.velocity = new Vector2(0, 0);
        rigidbody2D.AddForce(Vector2.up * Jumppower);
        //animator.Play:直接Animatorを呼ぶ
        //animator.Play("PlayerDie");
        animator.SetBool("IsDie", true);
        BoxCollider2D boxCollider2D = GetComponent<BoxCollider2D>();
        Destroy(boxCollider2D);
        gameManager.GameOver();
    }

    void PlayerClimb()
    {
        animator.SetBool("IsClimbing", true);
        //重力を無効にする
        isClimbing = true;
        rigidbody2D.gravityScale = 0;
    }

}
