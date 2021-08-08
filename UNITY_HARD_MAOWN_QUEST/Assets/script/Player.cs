using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;


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
    //靜態 static
    //1.靜態欄位不會在重新仔路後還原為預設值
    //2.靜態欄位不會顯示在屬性面板上
    //3.生命值數量
    public static int life = 2;
    //判定地板尖刺
    public float hitBoxCDtime;

    private AudioSource aud;
    private Rigidbody2D rig;
    protected Animator ani;
    protected Transform player;
    private ParticleSystem ps;
    /// <summary>
    /// 血量
    /// </summary>
    private Image imgHp;
    //生命值
    private Text textHp;
    /// <summary>
    /// 血量最大值
    /// </summary>
    private float hpmax;
    /// <summary>
    /// 攻擊力
    /// </summary>
    private float attack = 10;
    /// <summary>
    /// 判定尖刺傷害範圍
    /// </summary>
    private PolygonCollider2D polygonCollider2D;
    private Text textFinaleTitle;

    private Inventory inventory;
    [SerializeField] private UI_Inventory uiInventory;


    #endregion

    #region 事件
    private CanvasGroup groupFinal;
    //private void Awake()
    //{
        //inventory = new Inventory();
        //uiInventory.SetInventory(inventory);
        //ItemWorld.SpawnItemWorld(new Vector3(20, 20), new Item { itemType = Item.ItemType.fire, amount = 1 });
        //ItemWorld.SpawnItemWorld(new Vector3(-20, 20), new Item { itemType = Item.ItemType.health, amount = 1 });
        //ItemWorld.SpawnItemWorld(new Vector3(20, -20), new Item { itemType = Item.ItemType.poison, amount = 1 });
    //}
    private void Start()
    {
        rig = GetComponent<Rigidbody2D>();
        ani = GetComponent<Animator>();
        aud = GetComponent<AudioSource>();
        ps = transform.Find("集氣").GetComponent<ParticleSystem>();
        imgHp = GameObject.Find("血條").GetComponent<Image>();
        textHp = GameObject.Find("生命").GetComponent<Text>();
        textHp.text = life.ToString();
        hpmax = HP; //抓到初始玩家血量
        Physics2D.IgnoreLayerCollision(9, 10, true);
        groupFinal = GameObject.Find("結束畫面").GetComponent<CanvasGroup>();
        textFinaleTitle = GameObject.Find("結束標題").GetComponent<Text>();
        polygonCollider2D = GetComponent<PolygonCollider2D>();
        
        inventory = new Inventory(UseItem);
        
        uiInventory.SetPlayer(this);
        uiInventory.SetInventory(inventory);
        
    }
    
    public void FixedUpdate()
    {
        MoveFixed();
    }
    
    public void MoveFixed()
    {
        rig.velocity = new Vector2(h * Speed * Time.deltaTime, rig.velocity.y);
    }
    public void Update()
    {
        if (Dead()) return;
        Move();
        Jump();
        Fire();
        
        if(useFire && Input.GetKeyDown(KeyCode.E))
        {
            inventory.RemoveItem(new Item { itemType = Item.ItemType.fire, amount = 1 });
            
            useFire = false;
        }



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
    private void OnParticleCollision(GameObject other)
    {
        Hit(other.GetComponent<ParticleSystemData>().attack);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (win) return;
        if (collision.name == "死亡區域") HP = 0;
        //吃道具
        ItemWorld itemWorld = collision.GetComponent<ItemWorld>();
        if (itemWorld != null)
        {
            //觸碰道具
            inventory.AddItem(itemWorld.GetItem());
            itemWorld.DestroySelf();
        }
            
    }

    #endregion

    #region 方法
    private float h;
    public void Move()
    {
        h = Input.GetAxis("Horizontal");
        
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
            //暫存物件.添加元件<子彈>();
            temp.AddComponent<Bullet>();
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
            //攻擊力 累加 四捨五入(計時器) * 2
            temp.GetComponent<Bullet>().attack = attack + (Mathf.Round(timer) + 5) * 4;
            //集氣:調整子彈尺寸
            // temp.transform.lossyScale = Vector3.one; //為唯獨 read only - 不能指定值 - 此行為錯誤示範會出現紅色錯誤標示
            temp.transform.localScale = Vector3.one + Vector3.one * timer;

            //計時器歸零
            timer = 0;

            // 子彈攻擊力
        }
        #endregion


    }
    bool useFire;
    public void UseItem(Item item)
    {
        switch (item.itemType)
        {
            case Item.ItemType.health: // 補血道具使用
                HP += 20;
                HP = Mathf.Clamp(HP, 0, hpmax);
                imgHp.fillAmount = HP / hpmax;
                inventory.RemoveItem(new Item { itemType = Item.ItemType.health, amount = 1 });
                break;
            case Item.ItemType.fire:
                useFire = true;
                inventory.RemoveItem(new Item { itemType = Item.ItemType.fire, amount = 1 });
                break;
            case Item.ItemType.poison:
                HP -= 20;
                HP = Mathf.Clamp(HP, 0, hpmax);
                imgHp.fillAmount = HP / hpmax;
                inventory.RemoveItem(new Item { itemType = Item.ItemType.poison, amount = 1 });
                break;
        }
    }
    public void Hit(float damage)
    {
        if (win) return;
        ani.SetTrigger("受傷觸發");
        HP -= damage;
        imgHp.fillAmount = HP / hpmax;
        if (HP <= 0) Dead();
        polygonCollider2D.enabled = false;
        StartCoroutine(ShowPlayerHitBox());

    }
    /// <summary>
    /// 地刺傷害判定
    /// </summary>
    /// <returns></returns>
    IEnumerator ShowPlayerHitBox()
    {
        yield return new WaitForSeconds(hitBoxCDtime);
        polygonCollider2D.enabled = true;
    }
    private bool Dead()
    {
        // 如果尚未死亡 並且血量低於等於零 才可以執行死亡
        if (!ani.GetBool("死亡開關") && HP <= 0)
        {
            ani.SetBool("死亡開關", HP <= 0);
            life--;
            textHp.text = life.ToString();
            if (life > 0) Invoke("Replay", 2f); // 如果生命值大於0就重新遊戲

            else StartCoroutine(GameOver());  //啟動協同程序
        }
        return HP <= 0;
    }
    private bool win;
    public IEnumerator GameOver(string finalTitle = "GameOver")
    {
        if(!win)
        {
            if (finalTitle == "YOU WIN") win = true;
            textFinaleTitle.text = finalTitle;
        }
        
        while (groupFinal.alpha <1)
        {
            groupFinal.alpha += 0.05f;
            yield return new WaitForSeconds(0.02f);
        }
        groupFinal.interactable = true;
        groupFinal.blocksRaycasts = true;
    }
    private void Replay()
    {
        SceneManager.LoadScene("遊戲畫面");
    }
}
