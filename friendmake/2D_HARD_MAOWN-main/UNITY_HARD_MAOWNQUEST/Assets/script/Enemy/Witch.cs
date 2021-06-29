using UnityEngine;

public class Witch : MonoBehaviour
{
    [Header("移動速度"), Range(0, 100)]
    public float speed = 1f;
    [Header("攻擊力"), Range(0, 100)]
    public float attack = 10f;
    [Header("攻擊冷卻"), Range(0, 30)]
    public float cd = 3;
    [Header("血量"), Range(0, 1000)]
    public float hp = 100f;
    [Header("偵測範圍"), Range(0, 50)]
    public float radiusTrack = 5;
    [Header("攻擊範圍"), Range(0, 30)]
    public float radiusAttack = 2;
    [Header("偵測地板位移與半徑")]
    public Vector3 groundoffset;
    public float groundRadius = 0.1f;
    [Header("打擊音效"), Tooltip("開槍的音效")]
    public AudioClip bulletSound;
    [Header("子彈生成位置")]
    public Vector3 posBullet;
    [Header("子彈"), Tooltip("角色要發射的子彈物件")]
    public GameObject bullet;
    [Header("子彈速度"), Range(0, 5000)]
    public int bulletspeed = 800;

    private Transform player;
    private Rigidbody2D rig;
    private Animator ani;
    private AudioSource aud;
    private ParticleSystem ps;
    
    /// <summary>
    /// Cd時間
    /// </summary>
    private float timer;
    /// <summary>
    /// 原始速度
    /// </summary>
    private float speedOriginal;

    private void Start()
    {
        ani = GetComponent<Animator>();
        rig = GetComponent<Rigidbody2D>();
        aud = GetComponent<AudioSource>();
        player = GameObject.Find("Player").transform;

        timer = cd;
        speedOriginal = speed;
    }
    private void OnDrawGizmos()
    {
        #region 繪製距離與檢查地板
        Gizmos.color = new Color(0, 1, 0, 0.3f);
        Gizmos.DrawSphere(transform.position, radiusTrack);

        Gizmos.color = new Color(1, 0, 0, 0.3f);
        Gizmos.DrawSphere(transform.position, radiusAttack);

        Gizmos.color = new Color(0.6f, 0.9f, 1, 0.7f);
        Gizmos.DrawSphere(transform.position + transform.right * groundoffset.x + transform.up * groundoffset.y, groundRadius);
        #endregion
        #region 繪製子彈生成區域
        Gizmos.color = new Color(0, 0, 1, 0.5f);
        Gizmos.DrawSphere(transform.position + transform.right * posBullet.x + transform.up * posBullet.y, 0.1f);

        #endregion
    }
    private void Update()
    {
        Move();

    }
    private void Move()
    {
        //如果死亡就跳出
        if (ani.GetBool("死亡")) return;
        float dis = Vector3.Distance(player.position, transform.position);

        if (dis <= radiusAttack)
        {
            Attack();
        }

        else if (dis <= radiusTrack)
        {
            rig.velocity = transform.right * speed * Time.deltaTime;
            ani.SetBool("走路開關", speed != 0);
            LookAtPlayer();
            CheckGround();
        }

        else
        {
            ani.SetBool("走路開關", false);
        }
    }
    private void Attack()
    {
        ani.SetBool("走路開關", false);

        if (timer <= cd)
        {
            timer += Time.deltaTime;
        }
        else
        {
            timer = 0;
            ani.SetTrigger("攻擊觸發");
            ps.Play();
            aud.PlayOneShot(bulletSound, 0.5f);
            GameObject temp = Instantiate(bullet, transform.position + transform.right * posBullet.x + transform.up * posBullet.y, Quaternion.identity); //簡寫
            // 暫存物件.取得元件<2D鋼體>().添加推力(角色的前方 * 子彈速度)
            temp.GetComponent<Rigidbody2D>().AddForce(transform.right * bulletspeed);
            //暫存物件.添加元件<子彈>();
            temp.AddComponent<EnemyBullet>();
            // 刪除(物件 , 延遲秒數) 
            Destroy(temp, 1f);
            ParticleSystemRenderer render = temp.GetComponent<ParticleSystemRenderer>();
            render.flip = new Vector3(transform.eulerAngles.y == 0 ? 0 : 1, 0, 0);

        }
    }
    private void LookAtPlayer()
    {
        if (transform.position.x > player.position.x)
        {
            transform.eulerAngles = new Vector3(0, 180, 0);
        }

        else
        {
            transform.eulerAngles = Vector3.zero;
        }


    }
    private void CheckGround()
    {
        Collider2D hit = Physics2D.OverlapCircle(transform.position + transform.right * groundoffset.x + transform.up * groundoffset.y, groundRadius, 1 << 8);

        if (hit && (hit.name == "地板" || hit.name == "跳台"))
        {
            print("可以向前");
            speed = speedOriginal;
        }

        else
        {
            print("不可以向前");
            speed = 0;
        }

    }
    private void Dead()
    {
        ani.SetBool("死亡", true);
        //死亡後關閉碰撞
        rig.Sleep();
        //死亡後凍結鋼體座標
        rig.constraints = RigidbodyConstraints2D.FreezeAll;
        GetComponent<CapsuleCollider2D>().enabled = false;
        //死亡後兩秒刪除物件
        Destroy(gameObject, 2);
    }
    public void Hit(float damge)
    {
        hp -= damge;

        if (hp <= 0) Dead();
    }
}
