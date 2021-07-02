using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("移動速度"), Range(0, 1000)]
    public float speed = 1f;
    [Header("攻擊力"), Range(0, 100)]
    public float attack = 10f;
    [Header("血量"), Range(0, 500)]
    public float hp = 200f;
    [Header("偵測範圍"), Range(0, 50)]
    public float radiusTrack = 5f;
    [Header("攻擊範圍"), Range(0, 50)]
    public float radiusAttack = 2;
    [Header("攻擊冷卻"), Range(0, 30)]
    public float cd = 1;
    [Header("偵測地板的位移與半徑")]
    public Vector3 groundoffest;
    public float groundRadius = 0.1f;
    [Header("開槍音效"), Tooltip("開槍的音效")]
    public AudioClip bulletSound;

    public float flashTime;


    protected Transform player;
    private Rigidbody2D rig;
    protected Animator ani;
    private AudioSource aud;
    protected float timer;
    private float speedOringinal;
    private SpriteRenderer sr;
    private Color originalColor;

    #region 事件
    protected virtual void Start()
    {
        ani = GetComponent<Animator>();
        rig = GetComponent<Rigidbody2D>();
        aud = GetComponent<AudioSource>();
        //玩家 = 遊戲物件.尋找("物件名稱") - 搜尋場境內所有物件
        //transform.Find("子物件名稱") - 搜尋此物件的子物件
        player = GameObject.Find("Player").transform;
        //讓敵人一碰到玩家就開始攻擊
        timer = cd;
        speedOringinal = speed;
        sr = GetComponent<SpriteRenderer>();
        originalColor = sr.color;
    }
    [Header("攻擊區域位移與尺寸")]
    public Vector3 attackoffest;
    public Vector3 attackSize;
    private void Update()
    {
        Move();
        
    }
    private void OnDrawGizmos()
    {
        #region 繪製距離與檢查地板
        //追蹤判定球
        Gizmos.color = new Color(0, 1, 0, 0.5f);
        Gizmos.DrawSphere(transform.position, radiusTrack);

        //攻擊判定球
        Gizmos.color = new Color(1, 0, 0, 0.3f);
        Gizmos.DrawSphere(transform.position, radiusAttack);

        //防止掉落判定球
        Gizmos.color = new Color(0.6f, 0.9f, 1, 0.7f);
        Gizmos.DrawSphere(transform.position + transform.right * groundoffest.x + transform.up * groundoffest.y, groundRadius);
        #endregion
        Gizmos.color = new Color(0.3f, 0.3f, 1, 0.8f);
        Gizmos.DrawCube(transform.position + transform.right * attackoffest.x + transform.up * attackoffest.y, attackSize);

    }
    #endregion
    #region 方法
    private void Move()
    {
        //如果死亡就跳出
        if (ani.GetBool("死亡")) return;
        //如果玩家 跟敵人的 距離 小於等於 追蹤範圍 就移動

        // 距離 = 三維向量.距離(a點,b點)
        float dis = Vector3.Distance(player.position, transform.position);

        if (dis <= radiusAttack)
        {
            Attack();
            LookAtPlayer();
        }
        else if (dis <= radiusTrack)
        {
            rig.velocity = transform.right * speed * Time.deltaTime;
            ani.SetBool("走路開關", speed != 0);   //速度不等於零時 走路 否則 等待
            LookAtPlayer();
            CheckGround();
            //timer = cd;

        }
        else
        {
            ani.SetBool("走路開關", false);
            //timer = cd;
        }
    }
    private void Attack()
    {
        if (timer <= cd)
        {
            timer += Time.deltaTime;
        }
        else AttackState();

    }
    protected virtual void AttackState()
    {
        timer = 0;
        ani.SetTrigger("攻擊觸發");
        aud.PlayOneShot(bulletSound, 0.5f);
        Collider2D hit = Physics2D.OverlapBox(transform.position + transform.right * attackoffest.x + transform.up * attackoffest.y, attackSize, 0);
        if (hit && hit.name == "Player") hit.GetComponent<Player>().Hit(attack);
    }
    /// <summary>
    /// 面相玩家
    /// </summary>
    private void LookAtPlayer()
    {
        // 如果敵人 x 大於玩家x就代表玩家在左邊 角度180
        if (transform.position.x > player.position.x)
        {
            transform.eulerAngles = new Vector3(0, 180, 0);
        }
        // 否則敵人 x 小於玩家 x 就代表玩家在右邊 角度0
        else
        {
            transform.eulerAngles = Vector3.zero;
        }
    }
    private void CheckGround()
    {
        Collider2D hit = Physics2D.OverlapCircle(transform.position + transform.right * groundoffest.x + transform.up * groundoffest.y, groundRadius, 1 << 8);

        // 判斷式 程式只有一句(一個分號) 可以省略大括號
        if (hit && (hit.name == "地板" || hit.name == "跳台"))
        {
            speed = speedOringinal;
        }
        else
        {
            speed = 0;
        }

    }
    
    protected virtual void Dead()
    {
        ani.SetBool("死亡", true);
        rig.Sleep();                                                  //剛體 睡著 避免飄移
        rig.constraints = RigidbodyConstraints2D.FreezeAll;           //剛體凍結全部
        GetComponent<CapsuleCollider2D>().enabled = false;            //碰撞器關閉
        Destroy(gameObject, 2);                                       //兩秒後刪除物件
        
    }

    public virtual void Hit(float damage)
    {
        hp -= damage;
        FlashColor(flashTime);
        //判斷式 只有一個分號 可以省略大括號
        if (hp <= 0) Dead();
    }
    void FlashColor(float time)
    {
        sr.color = Color.red;
        Invoke("ResetColor", time);
    }
    void ResetColor()
    {
        sr.color = originalColor;
    }
    #endregion
}
