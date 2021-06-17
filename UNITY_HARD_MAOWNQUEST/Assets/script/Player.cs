using UnityEngine;
using UnityEditor.Animations;

public class Player : MonoBehaviour
{
    #region 欄位
    [Header("移動速度"), Range(0, 2000)]
    public float Speed = 200;
    [Header("跳躍高度"), Range(0, 2000)]
    public float JumpHeight = 200;
    [Header("血量"), Range(0, 200)]
    public float HP = 100;
    [Header("角色是否在地板上"), Tooltip("儲存角色在地板上")]
    public bool isGround;
    [Header("子彈"), Tooltip("角色要發射的子彈物件")]
    public GameObject bullet;
    [Header("子彈速度"), Range(0, 5000)]
    public int bulletspeed = 800;
    [Header("開槍音效"), Tooltip("開槍的音效")]
    public AudioClip bulletSound;
    [Header("判斷地板碰撞的位移與半徑")]
    public Vector3 groundoffest;
    public float groundRadius = 0.2f;
    [Header("子彈生成位置")]
    public Vector3 posBullet;

    private AudioSource aud;
    private Rigidbody2D rig;
    private Animator ani;
    private ParticleSystem ps;

    #endregion

    #region 事件
    private void Start()
    {
        rig = GetComponent<Rigidbody2D>();
        ani = GetComponent<Animator>();
        aud = GetComponent<AudioSource>();
        ps = transform.Find("集氣").GetComponent<ParticleSystem>();
    }
    public void Update()
    {
        Move();
        Jump();
        Fire();
        Physics2D.IgnoreLayerCollision(9, 10, true);

    }
    private void OnDrawGizmos()
    {
        //1.指定顏色
        Gizmos.color = new Color(1, 0, 0, 0.5f);
        //2.繪製圖形
        // trasform 可以抓到此腳本同一層的變形元件
        Gizmos.DrawSphere(transform.position + transform.right * groundoffest.x + transform.up * groundoffest.y + groundoffest, groundRadius);
        // 先指定顏色在畫圖型
        Gizmos.color = new Color(0, 0, 1, 0.5f);
        Gizmos.DrawSphere(transform.position + transform.right * posBullet.x + transform.up * posBullet.y, 0.1f);
    }
    public void Move()
    {
        float h = Input.GetAxis("Horizontal");
        rig.velocity = new Vector2(h * Speed * Time.deltaTime, rig.velocity.y);
        if ((Input.GetKeyDown(KeyCode.D)) || (Input.GetKeyDown(KeyCode.RightArrow)))
        {
            transform.eulerAngles = Vector3.zero;
        }
        // 否則如果 按下a鍵時面相左邊 -0, 180, 0
        else if ((Input.GetKeyDown(KeyCode.A)) || (Input.GetKeyDown(KeyCode.LeftArrow)))
        {
            transform.eulerAngles = new Vector3(0, 180, 0);
        }
        ani.SetBool("走路開關", h != 0);
    }
    public void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGround)
        {
            float J = Input.GetAxis("Vertical");
            rig.AddForce(new Vector2(0, JumpHeight));
        }
        Collider2D hit = Physics2D.OverlapCircle(transform.position + transform.right * groundoffest.x + transform.up * groundoffest.y, groundRadius, 1 << 8);
        if (hit && (hit.name == "地板" || hit.name == "跳台"))
        {
            //print("角色在地板上");
            isGround = true;
        }
        else
        {
            isGround = false;
        }

    }
    private float timer;
    public void Fire()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            ani.SetTrigger("攻擊觸發");
            ps.Play();
        }
        else if (Input.GetKey(KeyCode.Mouse0))
        {
            // 累加 +=
            timer += Time.deltaTime;
            //print("按住左鍵的時間" + timer);
        }
        else if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            ps.Stop();  //停止集氣
            aud.PlayOneShot(bulletSound, 0.5f);
            // Object.Instantiate(bullet); //原始寫法
            // Instantiate //簡寫
            // 暫存物件 = 生成(物件.座標.角度)
            // Quaternion 四位元-角度
            // Quaternion.identity
            GameObject temp = Instantiate(bullet, transform.position + transform.right * posBullet.x + transform.up * posBullet.y, Quaternion.identity); //簡寫
            // 暫存物件.取得元件<2D鋼體>().添加推力(角色的前方 * 子彈速度)
            temp.GetComponent<Rigidbody2D>().AddForce(transform.right * bulletspeed);
            // 刪除(物件 , 延遲秒數) 
            Destroy(temp, 1f);

            // 讓子彈的角度根玩家目前的角度相同 - 子彈角度問題
            //temp.transform.eulerAngles = transform.eulerAngles;
            //ParticleSystem.MainModule main = temp.GetComponent<ParticleSystem>().main;
            //main.flipRotation = transform.eulerAngles.y == 0 ? 0 : 1;
            ParticleSystemRenderer render = temp.GetComponent<ParticleSystemRenderer>();
            // 渲染的翻面 = 角色的角度 - ? : 三元運算子
            render.flip = new Vector3(transform.eulerAngles.y == 0 ? 0 : 1, 0, 0);
            // 計時器 = 數學.夾住(計時器,最小,最大); 
            timer = Mathf.Clamp(timer, 0, 5);
            //集氣:調整子彈尺寸
            // temp.transform.lossyScale = Vector3.one; //為唯獨 read only - 不能指定值 - 此行為錯誤示範會出現紅色錯誤標示
            temp.transform.localScale = Vector3.one + Vector3.one * timer;

            //計時器歸零
            timer = 0;
        }
        #endregion

        #region 方法
        #endregion


    }
}
