using UnityEngine;

public class Boss : Enemy
{
    [Header("進入第二階段的血量")]
    public float secondHP = 300;
    [Header("魔王攻擊: 第二階段特效")]
    public ParticleSystem psAttackSecond;
    [Header("第二階段攻擊力"), Range(0, 1000)]
    public float attackSecond = 10;
    public StateBoss stateBoss;
    
  
    public override void Hit(float damage)
    {
        base.Hit(damage);  // 指父類別原本的程式區塊
        //print("我是子類別 BOSS");
        if (hp <= secondHP)
        {
            radiusAttack = 7;
            stateBoss = StateBoss.second;

        }
    }
    protected override void AttackState()
    {


        switch (stateBoss)
        {
            case StateBoss.first:
                base.AttackState();
                break;
            case StateBoss.second:
                timer = 0;
                ani.SetTrigger("攻擊觸發");
                
                psAttackSecond.transform.position = transform.position + transform.right * 7f + transform.up * -0.5f;
                psAttackSecond.transform.eulerAngles = transform.eulerAngles;

                psAttackSecond.Play();
                break;
        }
    }
    protected override void Start()
    {
        base.Start();
        psAttackSecond.GetComponent<ParticleSystemData>().attack = attackSecond;
    }
    protected override void Dead()
    {
        base.Dead();
        
        StartCoroutine(player.GetComponent<Player>().GameOver("YOU WIN"));
    }

}
public enum StateBoss
{
    first, second
}
