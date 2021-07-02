using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime : MonoBehaviour
{
    #region 欄位
    [Header("移動速度"), Range(0, 100)]
    public float speed = 1f;
    [Header("攻擊力"), Range(0, 100)]
    public float attack = 10f;
    [Header("血量"), Range(0, 1000)]
    public float hp = 100f;
    [Header("偵測範圍"), Range(0, 50)]
    public float radiusTrack = 5;
    [Header("攻擊範圍"), Range(0, 30)]
    public float radiusAttack = 2;

    
    private Transform player;
    private Rigidbody2D rig;
    private Animator ani;
    #endregion

    #region 事件
    private void Start()
    {
        ani = GetComponent<Animator>();
        rig = GetComponent<Rigidbody2D>();

        player = GameObject.Find("Player").transform;

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0, 1, 0, 0.3f);
        Gizmos.DrawSphere(transform.position, radiusTrack);

        Gizmos.color = new Color(1, 0, 0, 0.3f);
        Gizmos.DrawSphere(transform.position, radiusAttack);
    }

    private void Update()
    {
        Move();
    }
    #endregion

    #region 方法
    private void Move()
    {
        float dis = Vector3.Distance(player.position, transform.position);
        
        if (dis <= radiusTrack)
        {
            rig.velocity = transform.right * speed * Time.deltaTime;
        }
    }
    #endregion
}
